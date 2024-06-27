using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static ICBFApp.Pages.Asistencia.IndexModel;
using static ICBFApp.Pages.AvancesAcademicos.IndexModel;
using static ICBFApp.Pages.Jardin.IndexModel;

namespace ICBFApp.Pages.AvancesAcademicos
{
    public class EditModel : PageModel
    {
        public AvanceAcademicoInfo avanceAcademicoInfo = new AvanceAcademicoInfo();
        public int[] listaAño { get; set; } = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        public string[] listaNivel { get; set; } = new string[] { "Prenatal", "Natal", "Párvulo", "Prejardín", "Jardín" };
        public string[] listaNota { get; set; } = new string[] { "S", "A", "B" };
        public string errorMessage = "";
        public string successMessage = "";

        private readonly string _connectionString;

        public EditModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionSQLServer");
        }

        public void OnGet()
        {
            String idAvanceAcademico = Request.Query["IdAvanceAcademico"];

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM AvancesAcademicos WHERE IdAvanceAcademico = @IdAvanceAcademico";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdAvanceAcademico", idAvanceAcademico);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                avanceAcademicoInfo.idAvanceAcademico = "" + reader.GetInt32(0);
                                avanceAcademicoInfo.nivel = reader.GetString(1);
                                avanceAcademicoInfo.notas = reader.GetString(2);
                                avanceAcademicoInfo.descripcion = reader.GetString(3);
                                avanceAcademicoInfo.fechaEntrega = reader.GetDateTime(4).ToString("yyyy-MM-dd");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public IActionResult OnPost()
        {
            avanceAcademicoInfo.idAvanceAcademico = Request.Form["IdAvanceAcademico"];
            avanceAcademicoInfo.nivel = Request.Form["nivel"];
            avanceAcademicoInfo.notas = Request.Form["notas"];
            avanceAcademicoInfo.descripcion = Request.Form["descripcion"];
            avanceAcademicoInfo.fechaEntrega = Request.Form["fechaEntrega"];

            if (string.IsNullOrEmpty(avanceAcademicoInfo.nivel) || string.IsNullOrEmpty(avanceAcademicoInfo.notas) || string.IsNullOrEmpty(avanceAcademicoInfo.fechaEntrega))
            {
                errorMessage = "Todos los campos son obligatorios";
                OnGet();
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {

                    connection.Open();

                    // Espacio para validar que el jadin no exista
                    String sqlUpdate = "UPDATE AvancesAcademicos SET nivel = @Nivel, notas = @Notas, descripcion = @Descripcion, fechaEntrega = @FechaEntrega WHERE idAvanceAcademico = @IdAvanceAcademico";
                    using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                    {
                        command.Parameters.AddWithValue("@IdAvanceAcademico", avanceAcademicoInfo.idAvanceAcademico);
                        command.Parameters.AddWithValue("@Nivel", avanceAcademicoInfo.nivel);
                        command.Parameters.AddWithValue("@Notas", avanceAcademicoInfo.notas);
                        command.Parameters.AddWithValue("@Descripcion", avanceAcademicoInfo.descripcion);
                        command.Parameters.AddWithValue("@FechaEntrega", avanceAcademicoInfo.fechaEntrega);

                        command.ExecuteNonQuery();
                    }

                    TempData["SuccessMessage"] = "Avance académico editado exitosamente";
                    return RedirectToPage("/AvancesAcademicos/Index");
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
