using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static ICBFApp.Pages.AvancesAcademicos.IndexModel;
using static ICBFApp.Pages.Ninio.IndexModel;
using static ICBFApp.Pages.Usuario.IndexModel;

namespace ICBFApp.Pages.AvancesAcademicos
{
    public class CreateModel : PageModel
    {

        public AvanceAcademicoInfo avanceAcademicoInfo = new AvanceAcademicoInfo();
        public List<UsuarioInfo> listaAcudientes { get; set; } = new List<UsuarioInfo>();
        public List<NinioInfo> listaNinios { get; set; } = new List<NinioInfo>();
        public List<DatosBasicosInfo> listaDatosBasicos { get; set; } = new List<DatosBasicosInfo>();
        public string errorMessage = "";
        public string successMessage = "";

        private readonly string _connectionString;

        public CreateModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionSQLServer");
        }

        public void OnGet()
        {
                try
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        String sqlNinio = "SELECT Ninos.IdNino, DatosBasicos.Identificacion, DatosBasicos.Nombres " +
                            "FROM Ninos " +
                            "INNER JOIN DatosBasicos ON Ninos.IdDatosBasicos = DatosBasicos.IdDatosBasicos;";
                        using (SqlCommand command = new SqlCommand(sqlNinio, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                // Verificar si hay filas en el resultado antes de intentar leer
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                    var idNinio = reader.GetInt32(0).ToString();
                                    var identificacion = reader.GetString(1);
                                    var nombres = reader.GetString(2);
                                    DatosBasicosInfo datosNinios = new DatosBasicosInfo();
                                    datosNinios.identificacion = reader.GetString(1);
                                    datosNinios.nombres = reader.GetString(2);

                                    listaNinios.Add(new NinioInfo
                                    {
                                        idNinio = reader.GetInt32(0).ToString(),
                                        datosBasicos = datosNinios

                                    });

                                        foreach (var Ninio in listaNinios)
                                        {
                                            Console.WriteLine("List item - id: {0}, identificacion: {1}, nombres: {2}", Ninio.idNinio, Ninio.datosBasicos.identificacion, Ninio.datosBasicos.nombres);
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No hay filas en el resultado.");
                                    Console.WriteLine("No se encontraron datos en la tabla avances académicos.");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.ToString());
                    errorMessage = ex.Message;
                }
        }

        public IActionResult OnPost()
        {
            string nivel = Request.Form["nivel"];
            string notas = Request.Form["notas"];
            string descripcion = Request.Form["descripcion"];
            string fechaEntrega = Request.Form["fechaEntrega"];
            string ninioIdString = Request.Form["ninio"];
            int ninioId;

            if (string.IsNullOrEmpty(nivel) || string.IsNullOrEmpty(notas)
                || string.IsNullOrEmpty(fechaEntrega))
            {
                errorMessage = "Todos los campos son obligatorios";
                OnGet();
                return Page();
            }

            if (!int.TryParse(ninioIdString, out ninioId))
            {
                errorMessage = "Niño inválido seleccionado";
                OnGet();
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    String sqlInsertAvanceAcademico = "INSERT INTO AvancesAcademicos (nivel, notas, descripcion, fechaEntrega, idNino)" +
                            "VALUES (@Nivel, @Notas, @Descripcion, @FechaEntrega, @IdNino);";

                    using (SqlCommand command2 = new SqlCommand(sqlInsertAvanceAcademico, connection))
                    {
                        command2.Parameters.AddWithValue("@Nivel", nivel);
                        command2.Parameters.AddWithValue("@Notas", notas);
                        command2.Parameters.AddWithValue("@Descripcion", descripcion);
                        command2.Parameters.AddWithValue("@FechaEntrega", fechaEntrega);
                        command2.Parameters.AddWithValue("@IdNino", ninioId);

                        command2.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Avance académico creado exitosamente";
                return RedirectToPage("/AvancesAcademicos/Index");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return Page();
            }
        }
    }
}
