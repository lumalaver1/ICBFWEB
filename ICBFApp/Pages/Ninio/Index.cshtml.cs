using ICBFApp.Services.Ninio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuestPDF.Fluent;
using System.Data.SqlClient;
using static ICBFApp.Pages.Jardin.IndexModel;
using static ICBFApp.Pages.TipoDocumento.IndexModel;
using static ICBFApp.Pages.Usuario.IndexModel;
using static QuestPDF.Helpers.Colors;

namespace ICBFApp.Pages.Ninio
{
    public class IndexModel : PageModel
    {
        private readonly IGeneratePdfService _generatePdfService;
        private readonly string _connectionString;

        public IndexModel(IGeneratePdfService generatePdfService, IConfiguration configuration)
        {
            _generatePdfService = generatePdfService;
            _connectionString = configuration.GetConnectionString("ConexionSQLServer");
        }

        public List<NinioInfo> listNinio = new List<NinioInfo>();
        public string SuccessMessage { get; set; }

        public void OnGet()
        {
            if (TempData.ContainsKey("SuccessMessage"))
            {
                SuccessMessage = TempData["SuccessMessage"] as string;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    String sqlSelect = "SELECT d.idTipoDocumento, t.tipo, n.idDatosBasicos, identificacion, nombres, fechaNacimiento, " +
                        "e.idEps, e.nombre, j.idJardin, j.nombre, n.idAcudiente, " +
                        "(SELECT idDatosBasicos FROM Usuarios WHERE idUsuario = n.idAcudiente) as idDatosBasicosAcudiente, " +
                        "(SELECT nombres FROM Usuarios as u " +
                        "INNER JOIN DatosBasicos as d ON u.idDatosBasicos = d.idDatosBasicos " +
                        "WHERE idUsuario = n.idAcudiente) as acudiente, " +
                        "idNino, ciudadNacimiento, tipoSangre, " +
                        "(SELECT identificacion FROM Usuarios as u " +
                        "INNER JOIN DatosBasicos as d ON u.idDatosBasicos = d.idDatosBasicos " +
                        "WHERE idUsuario = n.idAcudiente) as identificacionAcudiente, n.idMadreComunitaria, " +
                        "(SELECT idDatosBasicos FROM Usuarios WHERE idUsuario = n.idMadreComunitaria) as idDatosBasicosMadreC,  " +
                        "(SELECT nombres FROM Usuarios as u " +
                        "INNER JOIN DatosBasicos as d ON u.idDatosBasicos = d.idDatosBasicos " +
                        "WHERE idUsuario = n.idMadreComunitaria) as madreComunitaria " +
                        "FROM Ninos as n " +
                        "INNER JOIN Jardines as j ON n.idJardin = j.idJardin " +
                        "INNER JOIN DatosBasicos as d ON n.idDatosBasicos = d.idDatosBasicos " +
                        "INNER JOIN TipoDocumento as t ON d.idTipoDocumento = t.idTipoDoc " +
                        "INNER JOIN EPS as e ON n.idEps = e.idEps " +
                        "INNER JOIN Usuarios as acu ON n.idAcudiente = acu.idUsuario " +
                        "INNER JOIN Usuarios as mad ON n.idMadreComunitaria = mad.idUsuario; ";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Validar si hay datos
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    TipoDocInfo tipoDocInfo = new TipoDocInfo();
                                    tipoDocInfo.idTipoDoc = reader.GetInt32(0).ToString();
                                    tipoDocInfo.tipo = reader.GetString(1).ToString();

                                    DatosBasicosInfo datosBasicos = new DatosBasicosInfo();
                                    datosBasicos.idDatosBasicos = reader.GetInt32(2).ToString();
                                    datosBasicos.tipoDoc = tipoDocInfo;
                                    datosBasicos.identificacion = reader.GetString(3);
                                    datosBasicos.nombres = reader.GetString(4);
                                    datosBasicos.fechaNacimiento = reader.GetDateTime(5).Date.ToShortDateString();

                                    JardinInfo jardin = new JardinInfo();
                                    jardin.idJardin = reader.GetInt32(8).ToString();
                                    jardin.nombre = reader.GetString(9);

                                    DatosBasicosInfo datosAcudiente = new DatosBasicosInfo();
                                    datosAcudiente.idDatosBasicos = reader.GetInt32(11).ToString();
                                    datosAcudiente.identificacion = reader.GetString(16);
                                    datosAcudiente.nombres = reader.GetString(12);

                                    UsuarioInfo acudiente = new UsuarioInfo();
                                    acudiente.idUsuario = reader.GetInt32(10).ToString();
                                    acudiente.datosBasicos = datosAcudiente;

                                    DatosBasicosInfo datosMadreComunitaria = new DatosBasicosInfo();
                                    datosMadreComunitaria.idDatosBasicos = reader.GetInt32(18).ToString();
                                    datosMadreComunitaria.nombres = reader.GetString(19);

                                    UsuarioInfo madreComunitaria = new UsuarioInfo();
                                    madreComunitaria.idUsuario = reader.GetInt32(17).ToString();
                                    madreComunitaria.datosBasicos = datosMadreComunitaria;

                                    NinioInfo ninio = new NinioInfo();
                                    ninio.idNinio = reader.GetInt32(13).ToString();
                                    ninio.ciudadNacimiento = reader.GetString(14);
                                    ninio.tipoSangre = reader.GetString(15);
                                    ninio.edad = calcularEdad(reader.GetDateTime(5).Date.ToShortDateString());
                                    ninio.jardin = jardin;
                                    ninio.acudiente = acudiente;
                                    ninio.madreComunitaria = madreComunitaria;
                                    ninio.datosBasicos = datosBasicos;
                                    

                                    listNinio.Add(ninio);
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay filas en el resultado");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }

        public IActionResult OnPostDownloadPdf()
        {
            var report = _generatePdfService.GeneratePdfQuest();
            byte[] pdfBytes = report.GeneratePdf();
            var mimeType = "application/pdf";
            //return File(pdfBytes, mimeType, "Reporte.pdf"); // si le quito el nombre del archivo, no lo descarga auto
            return File(pdfBytes, mimeType); // si le quito el nombre del archivo, no lo descarga auto
        }

        public int calcularEdad(string fechaNacimientoStr)
        {
            DateTime fechaNacimiento;
            bool isValidDate = DateTime.TryParse(fechaNacimientoStr, out fechaNacimiento);

            if (!isValidDate)
            {
                throw new ArgumentException("La fecha de nacimiento no está en un formato válido.");
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

        public class NinioInfo
        {
            public string idNinio { get; set; }
            public string tipoSangre { get; set; }
            public string ciudadNacimiento { get; set; }
            public string peso { get; set; } //
            public string estatura { get; set; } //
            public int edad { get; set; }
            public JardinInfo jardin { get; set; }
            public UsuarioInfo acudiente { get; set; }
            public UsuarioInfo madreComunitaria { get; set; }
            public DatosBasicosInfo datosBasicos {  get; set; }
        }
    }
}
