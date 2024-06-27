using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static ICBFApp.Pages.AvancesAcademicos.IndexModel;
using static ICBFApp.Pages.Asistencia.IndexModel;
using System.Data.SqlClient;

namespace ICBFApp.Pages.Asistencia
{
    public class EditModel : PageModel
    {
        public AsistenciaInfo asistenciaInfo = new AsistenciaInfo();
        public string[] listaEstado { get; set; } = new string[] { "Enfermo", "Sano", "Decaido" };
        public string errorMessage = "";
        public string successMessage = "";

        private readonly string _connectionString;

        public EditModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionSQLServer");
        }

        public void OnGet()
        {
            String idAsistencia = Request.Query["IdAsistencia"];

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Asistencias WHERE IdAsistencia = @IdAsistencia";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdAsistencia", idAsistencia);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                asistenciaInfo.idAsistencia = "" + reader.GetInt32(0);
                                asistenciaInfo.fecha = reader.GetDateTime(1).Date.ToString("yyyy-MM-dd");
                                asistenciaInfo.estadoNino = reader.GetString(2);
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
            asistenciaInfo.idAsistencia = Request.Form["idAsistencia"];
            asistenciaInfo.fecha = Request.Form["fecha"];
            asistenciaInfo.estadoNino = Request.Form["estadoNino"];

            if (string.IsNullOrEmpty(asistenciaInfo.fecha) || string.IsNullOrEmpty(asistenciaInfo.estadoNino))
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
                    String sqlUpdate = "UPDATE Asistencias SET Fecha = @Fecha, EstadoNino = @EstadoNino WHERE IdAsistencia = @IdAsistencia";
                    using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                    {
                        command.Parameters.AddWithValue("@IdAsistencia", asistenciaInfo.idAsistencia);
                        command.Parameters.AddWithValue("@Fecha", asistenciaInfo.fecha);
                        command.Parameters.AddWithValue("@EstadoNino", asistenciaInfo.estadoNino);

                        command.ExecuteNonQuery();
                    }

                    TempData["SuccessMessage"] = "Asistencia editada exitosamente";
                    return RedirectToPage("/Asistencia/Index");
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
