using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static ICBFApp.Pages.Jardin.IndexModel;
using System.Data.SqlClient;

namespace ICBFApp.Pages.Jardin
{
    public class CreateModel : PageModel
    {

        private readonly string _connectionString;

        public CreateModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionSQLServer");
        }

        public JardinInfo jardinInfo = new JardinInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            jardinInfo.nombre = Request.Form["nombreJardin"];
            jardinInfo.direccion = Request.Form["direccionJardin"];

            if (jardinInfo.nombre.Length == 0 || jardinInfo.direccion.Length == 0)
            {
                errorMessage = "Debe completar todos los campos";
                OnGet();
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    String sqlExists = "SELECT COUNT(*) FROM Jardines WHERE Nombre = @NombreJardin";
                    using (SqlCommand commandCheck = new SqlCommand(sqlExists, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@NombreJardin", jardinInfo.nombre);

                        int count = (int)commandCheck.ExecuteScalar();

                        if (count > 0)
                        {
                            errorMessage = "El Jardín '" + jardinInfo.nombre + "' ya existe. Verifique la información e intente de nuevo.";
                            OnGet();
                            return Page();
                        }
                    }

                    // Espacio para validar que el jadin no exista
                    String sqlInsert = "INSERT INTO Jardines (Nombre, Direccion, Estado)" +
                        "VALUES (@NombreJardin, @DireccionJardin, @Estado);";

                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@NombreJardin", jardinInfo.nombre);
                        command.Parameters.AddWithValue("@DireccionJardin", jardinInfo.direccion);
                        command.Parameters.AddWithValue("@Estado", "En trámite");

                        command.ExecuteNonQuery();
                    }
                    TempData["SuccessMessage"] = "Jardín agregado con éxito";
                    return RedirectToPage("/Jardin/Index");
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return Page();
            }
        }
    }
}
