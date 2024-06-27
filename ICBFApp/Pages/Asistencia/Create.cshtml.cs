using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static ICBFApp.Pages.Ninio.IndexModel;
using static ICBFApp.Pages.Usuario.IndexModel;
using static ICBFApp.Pages.Asistencia.IndexModel;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ICBFApp.Pages.Asistencia
{
    public class CreateModel : PageModel
    {

        public AsistenciaInfo asistenciaInfo = new AsistenciaInfo();
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

        public IActionResult OnGet()
        {
            // Validar la hora actual
            var horaActual = DateTime.Now.TimeOfDay;
            var horaInicio = new TimeSpan(8, 0, 0); // 8:00 AM
            var horaFin = new TimeSpan(10, 0, 0);  // 10:00 AM

            if (horaActual < horaInicio || horaActual > horaFin)
            {
                TempData["ErrorMessage"] = "Solo se permiten registros entre 8 AM y 10 AM";
                return RedirectToPage("/Asistencia/Index"); // Retorna la misma página para mostrar el mensaje de error
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    String sqlNinio = "SELECT Ninos.IdNino, DatosBasicos.Identificacion, DatosBasicos.Nombres " +
                        "FROM Ninos " +
                        "INNER JOIN DatosBasicos ON Ninos.idDatosBasicos = DatosBasicos.idDatosBasicos;";
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
                                    datosNinios.identificacion = identificacion;
                                    datosNinios.nombres = nombres;

                                    listaNinios.Add(new NinioInfo
                                    {
                                        idNinio = idNinio,
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
                                Console.WriteLine("No se encontraron datos en la tabla asistencias.");
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

            return Page();
        }

        public IActionResult OnPost()
        {
            string fecha = Request.Form["fecha"];
            string estadoNino = Request.Form["estadoNino"];
            string ninioIdString = Request.Form["ninio"];
            int ninioId;

            if (string.IsNullOrEmpty(fecha) || string.IsNullOrEmpty(estadoNino))
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

                    String sqlInsertAvanceAcademico = "INSERT INTO Asistencias (Fecha, EstadoNino, IdNino)" +
                            "VALUES (@Fecha, @EstadoNino, @IdNino);";

                    using (SqlCommand command2 = new SqlCommand(sqlInsertAvanceAcademico, connection))
                    {
                        command2.Parameters.AddWithValue("@Fecha", fecha);
                        command2.Parameters.AddWithValue("@EstadoNino", estadoNino);
                        command2.Parameters.AddWithValue("@IdNino", ninioId);

                        command2.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Asistencia creada exitosamente";
                return RedirectToPage("/Asistencia/Index");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return Page();
            }
        }
    }
 }

