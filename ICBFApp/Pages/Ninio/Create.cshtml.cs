using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static ICBFApp.Pages.Jardin.IndexModel;
using static ICBFApp.Pages.Ninio.IndexModel;
using static ICBFApp.Pages.Usuario.IndexModel;

namespace ICBFApp.Pages.Ninio
{
    public class CreateModel : PageModel
    {
        private readonly string _connectionString;

        public CreateModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionSQLServer");
        }

        public List<JardinInfo> listaJardines { get; set; } = new List<JardinInfo>();
        public List<UsuarioInfo> listaAcudientes { get; set; } = new List<UsuarioInfo>();
        public List<UsuarioInfo> listaMadresComunitarias { get; set; } = new List<UsuarioInfo>();
        public string[] listaTiposSangre { get; set; } = new string[] { "O+", "O-", "A+", "A-", "AB+", "AB-" };
        public NinioInfo ninio = new NinioInfo();
        public DatosBasicosInfo datosBasicos = new DatosBasicosInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    String sqlJardines = "SELECT idJardin, nombre from jardines";
                    using (SqlCommand command = new SqlCommand(sqlJardines, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Verificar si hay filas en el resultado antes de intentar leer
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var id = reader.GetInt32(0).ToString();
                                    var nombreJardin = reader.GetString(1);

                                    listaJardines.Add(new JardinInfo
                                    {
                                        idJardin = id,
                                        nombre = nombreJardin
                                    });

                                    foreach (var jardin in listaJardines)
                                    {
                                        Console.WriteLine("List item - id: {0}, nombreJardin: {1}", jardin.idJardin, jardin.nombre);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay filas en el resultado.");
                                Console.WriteLine("No se encontraron datos en la tabla jardines.");
                            }
                        }
                    }

                    String sqlEps = "SELECT idEps, nombre FROM eps";
                    using (SqlCommand command = new SqlCommand(sqlEps, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Verificar si hay filas en el resultado antes de intentar leer
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var idEps = reader.GetInt32(0).ToString();
                                    var nombre = reader.GetString(1);

                                    

                                   
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay filas en el resultado.");
                                Console.WriteLine("No se encontraron datos en la tabla eps.");
                            }
                        }
                    }

                    String sqlAcudiente = "SELECT IdUsuario, identificacion, nombres FROM Usuarios as u " +
                        "INNER JOIN DatosBasicos as d ON u.IdDatosBasicos = d.IdDatosBasicos " +
                        "INNER JOIN Roles as r ON u.IdRol = r.IdRol " +
                        "WHERE r.nombre = 'Acudiente';";
                    using (SqlCommand command = new SqlCommand(sqlAcudiente, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Verificar si hay filas en el resultado antes de intentar leer
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var idUsuario = reader.GetInt32(0).ToString();
                                    var identificacion = reader.GetString(1);
                                    var nombres = reader.GetString(2);

                                    DatosBasicosInfo datosAcudiente = new DatosBasicosInfo();
                                    datosAcudiente.identificacion = identificacion;
                                    datosAcudiente.nombres = nombres;

                                    listaAcudientes.Add(new UsuarioInfo
                                    {
                                        idUsuario = idUsuario,
                                        datosBasicos = datosAcudiente
                                    });

                                    foreach (var acudiente in listaAcudientes)
                                    {
                                        Console.WriteLine("List item - id: {0}, identificacion: {1}", acudiente.idUsuario, acudiente.datosBasicos.identificacion);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay filas en el resultado.");
                                Console.WriteLine("No se encontraron datos en la tabla usuarios-acudiente.");
                            }
                        }
                    }

                    String sqlMadreComunitaria = "SELECT idUsuario, identificacion, nombres FROM Usuarios as u " +
                        "INNER JOIN DatosBasicos as d ON u.idDatosBasicos = d.idDatosBasicos " +
                        "INNER JOIN Roles as r ON u.idRol = r.idRol " +
                        "WHERE r.nombre = 'Madre Comunitaria';";
                    using (SqlCommand command = new SqlCommand(sqlMadreComunitaria, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Verificar si hay filas en el resultado antes de intentar leer
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var idUsuario = reader.GetInt32(0).ToString();
                                    var identificacion = reader.GetString(1);
                                    var nombres = reader.GetString(2);

                                    DatosBasicosInfo datosMadreComunitaria = new DatosBasicosInfo();
                                    datosMadreComunitaria.identificacion = identificacion;
                                    datosMadreComunitaria.nombres = nombres;

                                    listaMadresComunitarias.Add(new UsuarioInfo
                                    {
                                        idUsuario = idUsuario,
                                        datosBasicos = datosMadreComunitaria
                                    });

                                    foreach (var madreComunitaria in listaMadresComunitarias)
                                    {
                                        Console.WriteLine("List item - id: {0}, identificacion: {1}", madreComunitaria.idUsuario, madreComunitaria.datosBasicos.identificacion);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay filas en el resultado.");
                                Console.WriteLine("No se encontraron datos en la tabla usuarios - madre comunitaria.");
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
            string identificacion = Request.Form["identificacion"];
            string nombres = Request.Form["nombres"];
            string fechaNacimiento = Request.Form["fechaNacimiento"];
            string ciudadNacimiento = Request.Form["ciudadNacimiento"];
            string celular = Request.Form["celular"];
            string direccion = Request.Form["direccion"];
            string tipoSangre = Request.Form["tipoSangre"];
            string acudienteIdString = Request.Form["acudiente"];
            string madreComunitariaIdString = Request.Form["madreComunitaria"];
            string jardinIdString = Request.Form["jardin"];
            string epsIdString = Request.Form["eps"];
            int epsId;
            int acudienteId;
            int madreComunitariaId;
            int jardinId;
            int tipoDocId = 3;
            int edad = calcularEdad(fechaNacimiento);

            if (string.IsNullOrEmpty(identificacion) || string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(fechaNacimiento) 
                || string.IsNullOrEmpty(ciudadNacimiento) || string.IsNullOrEmpty(celular) || string.IsNullOrEmpty(direccion)
                || string.IsNullOrEmpty(tipoSangre))
            {
                errorMessage = "Todos los campos son obligatorios";
                OnGet();
                return Page();
            }

            if (!int.TryParse(acudienteIdString, out acudienteId))
            {
                errorMessage = "Acudiente inválido seleccionado";
                OnGet();
                return Page();
            }

            if (!int.TryParse(madreComunitariaIdString, out madreComunitariaId))
            {
                errorMessage = "Madre Comunitaria inválida seleccionada";
                OnGet();
                return Page();
            }

            if (!int.TryParse(jardinIdString, out jardinId))
            {
                errorMessage = "Jardín inválido seleccionado";
                OnGet();
                return Page();
            }

            if (!int.TryParse(epsIdString, out epsId))
            {
                errorMessage = "EPS inválido seleccionado";
                OnGet();
                return Page();
            }

            if (edad > 5)
            {
                errorMessage = "La edad máxima permitida que es de 5 años";
                OnGet();
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    String sqlExists = "SELECT COUNT(*) FROM Ninos as n " +
                        "INNER JOIN DatosBasicos as d ON n.idDatosBasicos = d.idDatosBasicos " +
                        "WHERE d.identificacion = @identificacion;";
                    using (SqlCommand commandCheck = new SqlCommand(sqlExists, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@identificacion", identificacion);

                        int count = (int)commandCheck.ExecuteScalar();
                        if (count > 0)
                        {
                            errorMessage = "El niño " + nombres + " con identificación " + identificacion + " ya existe. " +
                                           "Verifique la información e intente de nuevo";
                            OnGet();
                            return Page();
                        }
                    }

                    String sqlTipoId = "SELECT idTipoDoc FROM TipoDocumento WHERE tipo = 'NIUP';";
                    using (SqlCommand commandTypeDoc = new SqlCommand(sqlTipoId, connection))
                    {
                        using (SqlDataReader reader = commandTypeDoc.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                tipoDocId = reader.GetInt32(0);
                            }
                        }
                    }

                    String sqlInsert = "INSERT INTO DatosBasicos" +
                        "(identificacion, nombres, fechaNacimiento, celular, direccion, idTipoDocumento)" +
                        "VALUES" +
                        "(@identificacion, @nombres, @fechaNacimiento, @celular, @direccion, @tipoDocumento)";

                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@identificacion", identificacion);
                        command.Parameters.AddWithValue("@nombres", nombres);
                        command.Parameters.AddWithValue("@fechaNacimiento", fechaNacimiento);
                        command.Parameters.AddWithValue("@celular", celular);
                        command.Parameters.AddWithValue("@direccion", direccion);
                        command.Parameters.AddWithValue("@tipoDocumento", tipoDocId);

                        command.ExecuteNonQuery();
                    }

                    String sqlSelectDatosBasicos = "SELECT TOP 1 idDatosBasicos FROM DatosBasicos ORDER BY idDatosBasicos DESC";

                    using (SqlCommand command2 = new SqlCommand(sqlSelectDatosBasicos, connection))
                    {
                        using (SqlDataReader reader = command2.ExecuteReader())
                        {
                            // Validar si hay datos
                            if (reader.Read())
                            {
                                datosBasicos.idDatosBasicos = reader.GetInt32(0).ToString();
                            }
                        }
                    }

                    String sqlInsertNinio = "INSERT INTO Ninos (tipoSangre, ciudadNacimiento, idJardin, idAcudiente, idMadreComunitaria, idDatosBasicos, idEps)" +
                            "VALUES (@tipoSangre, @ciudadNacimiento, @idJardin, @idAcudiente, @idMadreComunitaria, @idDatosBasicos, @idEps);";

                    using (SqlCommand command2 = new SqlCommand(sqlInsertNinio, connection))
                    {
                        command2.Parameters.AddWithValue("@tipoSangre", tipoSangre);
                        command2.Parameters.AddWithValue("@ciudadNacimiento", ciudadNacimiento);
                        command2.Parameters.AddWithValue("@idJardin", jardinId);
                        command2.Parameters.AddWithValue("@idAcudiente", acudienteId);
                        command2.Parameters.AddWithValue("@idMadreComunitaria", madreComunitariaId);
                        command2.Parameters.AddWithValue("@idDatosBasicos", datosBasicos.idDatosBasicos);
                        command2.Parameters.AddWithValue("@idEps", epsId);

                        command2.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Niño creado exitosamente";
                return RedirectToPage("/Ninio/Index");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return Page();
            }
        }

        public int calcularEdad(string fechaNacimientoStr)
        {
            DateTime fechaNacimiento;
            bool isValidDate = DateTime.TryParse(fechaNacimientoStr, out fechaNacimiento);

            if (!isValidDate)
            {
                errorMessage = "La fecha de nacimiento esta en un formato no válido.";
                OnGet();
            }

            DateTime today = DateTime.Today;
            int age = today.Year - fechaNacimiento.Year;

            // Comprueba si el cumpleaños aún no ha ocurrido en el año actual
            if (fechaNacimiento.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}