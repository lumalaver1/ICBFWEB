using Microsoft.AspNetCore.Mvc.RazorPages;
using static ICBFApp.Pages.Rol.IndexModel;
using static ICBFApp.Pages.TipoDocumento.IndexModel;
using static ICBFApp.Pages.Usuario.IndexModel;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace ICBFApp.Pages.Usuario
{
    public class EditModel : PageModel
    {
        private readonly string _connectionString;

        public EditModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionSQLServer");
        }

        public List<RolInfo> rolInfo { get; set; } = new List<RolInfo>();
        public List<TipoDocInfo> tipoDocInfo { get; set; } = new List<TipoDocInfo>();
        public UsuarioInfo usuarioInfo = new UsuarioInfo();
        public DatosBasicosInfo datosBasicos = new DatosBasicosInfo();
        public TipoDocInfo tipoDocInfoSelected = new TipoDocInfo();
        public RolInfo rolinfoSelected = new RolInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
            String idUsuario = Request.Query["id"];
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    String sqlUsuario = "SELECT idUsuario, d.idTipoDocumento, t.tipo, u.idDatosBasicos, identificacion, nombres, fechaNacimiento, celular, direccion, u.idRol, r.nombre, idUsuario " +
                        "FROM usuarios as u " +
                        "INNER JOIN Roles as r ON u.idRol = r.idRol " +
                        "INNER JOIN DatosBasicos as d ON u.idDatosBasicos = d.idDatosBasicos " +
                        "INNER JOIN TipoDocumento as t ON d.idTipoDocumento = t.idTipoDoc " +
                        "WHERE idUsuario = @idUsuario;";
                    using (SqlCommand command = new SqlCommand(sqlUsuario, connection))
                    {
                        command.Parameters.AddWithValue("@idUsuario", idUsuario);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                tipoDocInfoSelected.idTipoDoc = reader.GetInt32(1).ToString();
                                tipoDocInfoSelected.tipo = reader.GetString(2);

                                datosBasicos.idDatosBasicos = "" + reader.GetInt32(3);
                                datosBasicos.identificacion = reader.GetString(4);
                                datosBasicos.nombres = reader.GetString(5);
                                datosBasicos.fechaNacimiento = reader.GetDateTime(6).Date.ToString("yyyy-MM-dd");//
                                datosBasicos.celular = reader.GetString(7);
                                datosBasicos.direccion = reader.GetString(8);
                                datosBasicos.tipoDoc = tipoDocInfoSelected;

                                rolinfoSelected.idRol = reader.GetInt32(9).ToString();
                                rolinfoSelected.nombre = reader.GetString(10);

                                usuarioInfo.idUsuario = "" + reader.GetInt32(0);
                                usuarioInfo.datosBasicos = datosBasicos;
                                usuarioInfo.rol = rolinfoSelected;
                            }
                        }
                    }
                    String sqlRoles = "SELECT * FROM roles;";
                    using (SqlCommand command = new SqlCommand(sqlRoles, connection))
                    {
                        command.Parameters.AddWithValue("@idRol", rolinfoSelected.idRol);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Verificar si hay filas en el resultado antes de intentar leer
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var id = reader.GetInt32(0).ToString();
                                    var nombreRol = reader.GetString(1);

                                    rolInfo.Add(new RolInfo
                                    {
                                        idRol = reader.GetInt32(0).ToString(),
                                        nombre = reader.GetString(1)
                                    });

                                    foreach (var rol in rolInfo)
                                    {
                                        Console.WriteLine("List item - id: {0}, nombreRol: {1}", rol.idRol, rol.nombre);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay filas en el resultado.");
                                Console.WriteLine("No se encontraron datos en la tabla roles.");
                            }
                        }
                    }

                    String sqlTiposDoc = "SELECT * FROM tipoDocumento WHERE tipo != 'NIUP';";
                    using (SqlCommand command = new SqlCommand(sqlTiposDoc, connection))
                    {
                        command.Parameters.AddWithValue("@idTipoDoc", tipoDocInfoSelected.idTipoDoc);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Verificar si hay filas en el resultado antes de intentar leer
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var idDoc = reader.GetInt32(0).ToString();
                                    var tipoDoc = reader.GetString(1);

                                    tipoDocInfo.Add(new TipoDocInfo
                                    {
                                        idTipoDoc = reader.GetInt32(0).ToString(),
                                        tipo = reader.GetString(1)
                                    });

                                    foreach (var tipoDocu in tipoDocInfo)
                                    {
                                        Console.WriteLine("List item - id: {0}, tipo: {1}", tipoDocu.idTipoDoc, tipoDocu.tipo);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay filas en el resultado.");
                                Console.WriteLine("No se encontraron datos en la tabla tipo de Documento.");
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
            string celular = Request.Form["celular"];
            string direccion = Request.Form["direccion"];
            string tipoDocIdString = Request.Form["tipoDocumento"];
            string rolIdString = Request.Form["rol"];
            int rolId;
            int tipoDocId;
            int edad = calcularEdad(fechaNacimiento);

            if (string.IsNullOrEmpty(identificacion) || string.IsNullOrEmpty(nombres)
                || string.IsNullOrEmpty(fechaNacimiento) || string.IsNullOrEmpty(celular)
                || string.IsNullOrEmpty(direccion))
            {
                errorMessage = "Todos los campos son obligatorios";
                OnGet();
                return Page();
            }

            if (!int.TryParse(rolIdString, out rolId))
            {
                errorMessage = "Rol inválido seleccionado";
                OnGet();
                return Page();
            }

            if (!int.TryParse(tipoDocIdString, out tipoDocId))
            {
                errorMessage = "Tipo Documento inválido seleccionado";
                OnGet();
                return Page();
            }

            if (edad < 18)
            {
                errorMessage = "Debe ser mayor de edad";
                OnGet();
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string idDatosBasicos = Request.Form["idDatosBasicos"];
                    string idUsuario = Request.Form["idUsuario"];

                    String sqlUpdate = "UPDATE DatosBasicos SET " +
                        "identificacion = @identificacion, nombres = @nombres, fechaNacimiento = @fechaNacimiento, " +
                        "celular = @celular, direccion = @direccion, idTipoDocumento = @tipoDocumento " +
                        "WHERE idDatosBasicos = @idDatosBasicos";

                    using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                    {
                        command.Parameters.AddWithValue("@identificacion", identificacion);
                        command.Parameters.AddWithValue("@nombres", nombres);
                        command.Parameters.AddWithValue("@fechaNacimiento", fechaNacimiento);
                        command.Parameters.AddWithValue("@celular", celular);
                        command.Parameters.AddWithValue("@direccion", direccion);
                        command.Parameters.AddWithValue("@tipoDocumento", tipoDocId);
                        command.Parameters.AddWithValue("@idDatosBasicos", idDatosBasicos);

                        command.ExecuteNonQuery();
                    }

                    String sqlUpdateUsuario = "UPDATE usuarios SET idDatosBasicos = @datosBasicos, idRol = @rol " +
                            "WHERE idUsuario = @idUsuario;";

                    using (SqlCommand command2 = new SqlCommand(sqlUpdateUsuario, connection))
                    {
                        command2.Parameters.AddWithValue("@datosBasicos", idDatosBasicos);
                        command2.Parameters.AddWithValue("@rol", rolId);
                        command2.Parameters.AddWithValue("@idUsuario", idUsuario);

                        command2.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Usuario editado exitosamente";
                return RedirectToPage("/Usuario/Index");
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
                errorMessage = "La fecha de nacimiento no está en un formato válido.";
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