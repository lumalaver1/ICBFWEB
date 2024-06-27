using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Data.SqlClient;
using static ICBFApp.Pages.Jardin.IndexModel;
using static ICBFApp.Pages.Ninio.IndexModel;
using static ICBFApp.Pages.TipoDocumento.IndexModel;
using static ICBFApp.Pages.Usuario.IndexModel;

namespace ICBFApp.Services.Ninio
{
    public class GeneratePdfService : IGeneratePdfService
    {
        private readonly IWebHostEnvironment _host;
        private readonly string _connectionString;

        public GeneratePdfService(IWebHostEnvironment host, IConfiguration configuration)
        {
            _host = host;
            _connectionString = configuration.GetConnectionString("ConexionSQLServer");
        }

        public List<NinioInfo> listNinio = new List<NinioInfo>();

        public void GetData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sqlSelect = "SELECT identificacion, nombres, fechaNacimiento, j.nombre, " +
                        "(SELECT nombres FROM Usuarios as u " +
                        "INNER JOIN DatosBasicos as d ON u.idDatosBasicos = d.idDatosBasicos " +
                        "WHERE idUsuario = n.idAcudiente) as acudiente, " +
                        "(SELECT identificacion FROM Usuarios as u INNER JOIN DatosBasicos as d ON u.idDatosBasicos = d.idDatosBasicos " +
                        "WHERE idUsuario = n.idAcudiente) as identificacionAcudiente, " +
                        "(SELECT tipo FROM Usuarios as u " +
                        "INNER JOIN DatosBasicos as d ON u.idDatosBasicos = d.idDatosBasicos " +
                        "INNER JOIN TipoDocumento as t ON d.idTipoDocumento = t.idTipoDoc " +
                        "WHERE idUsuario = n.idAcudiente) as tipoDocAcudiente, " +
                        "(SELECT nombres FROM Usuarios as u " +
                        "INNER JOIN DatosBasicos as d ON u.idDatosBasicos = d.idDatosBasicos " +
                        "WHERE idUsuario = n.idMadreComunitaria) as madreCom, " +
                        "ciudadNacimiento, tipoSangre " +
                        "FROM Ninos as n " +
                        "INNER JOIN Jardines as j ON n.idJardin = j.idJardin " +
                        "INNER JOIN DatosBasicos as d ON n.idDatosBasicos = d.idDatosBasicos " +
                        "INNER JOIN TipoDocumento as t ON d.idTipoDocumento = t.idTipoDoc " +
                        "INNER JOIN Usuarios as acu ON n.idAcudiente = acu.idUsuario " +
                        "INNER JOIN Usuarios as mad ON n.idMadreComunitaria = mad.idUsuario;";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Validar si hay datos
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    DatosBasicosInfo datosBasicos = new DatosBasicosInfo();
                                    datosBasicos.identificacion = reader.GetString(0);
                                    datosBasicos.nombres = reader.GetString(1);
                                    datosBasicos.fechaNacimiento = reader.GetDateTime(2).Date.ToShortDateString();

                                    JardinInfo jardin = new JardinInfo();
                                    jardin.nombre = reader.GetString(3);

                                    TipoDocInfo tipoDocAcudiente = new TipoDocInfo();
                                    tipoDocAcudiente.tipo = reader.GetString(6).ToString();

                                    DatosBasicosInfo datosAcudiente = new DatosBasicosInfo();
                                    datosAcudiente.identificacion = reader.GetString(5);
                                    datosAcudiente.nombres = reader.GetString(4);
                                    datosAcudiente.tipoDoc = tipoDocAcudiente;

                                    UsuarioInfo acudiente = new UsuarioInfo();
                                    acudiente.datosBasicos = datosAcudiente;

                                    DatosBasicosInfo datosMadreComunitaria = new DatosBasicosInfo();
                                    datosMadreComunitaria.nombres = reader.GetString(7);

                                    UsuarioInfo madreComunitaria = new UsuarioInfo();
                                    madreComunitaria.datosBasicos = datosMadreComunitaria;

                                    NinioInfo ninio = new NinioInfo();
                                    ninio.ciudadNacimiento = reader.GetString(8);
                                    ninio.tipoSangre = reader.GetString(9);
                                    ninio.edad = calcularEdad(reader.GetDateTime(2).Date.ToShortDateString());
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

        public Document GeneratePdfQuest()
        {
            GetData();
            DateTime today = DateTime.Today;
            var report = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.ARCH_C);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Row(row =>
                    {
                        var rutaImgSena = Path.Combine(_host.WebRootPath, "images/logoSena.png");
                        byte[] imageDataSena = File.ReadAllBytes(rutaImgSena);

                        var rutaImgICBF = Path.Combine(_host.WebRootPath, "images/logoICBF.png");
                        byte[] imageDataICBF = File.ReadAllBytes(rutaImgICBF);

                        //row.ConstantItem(150).Height(60).Placeholder();
                        row.ConstantItem(75).AlignMiddle().Height(50).Image(imageDataSena);
                        row.ConstantItem(75).AlignMiddle().Height(65).Image(imageDataICBF);

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Height(20).Text("Asociación del ICBF").Bold().FontSize(14).AlignRight();
                            col.Item().Height(20).Text("Reporte Diario").Bold().AlignRight();
                            col.Item().Height(20).Text("Fecha de emisión: " + today.Date.ToShortDateString()).AlignRight();
                        });
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().PaddingVertical(10).AlignCenter()
                        .Text("Listado de Niños Inscritos")
                        .Bold().FontSize(24).FontColor("#39a900");

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(80);
                                columns.RelativeColumn(2);
                                columns.ConstantColumn(50);
                                columns.RelativeColumn(1.2f);
                                columns.ConstantColumn(50);
                                columns.ConstantColumn(70);
                                columns.ConstantColumn(80);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                // Fila de títulos de sección
                                header.Cell().ColumnSpan(5).Background(Colors.Green.Darken4).Border(0.5f).BorderColor(Colors.Black).AlignMiddle().Text("DATOS NIÑO").Bold().FontColor("#fff").AlignCenter();
                                header.Cell().ColumnSpan(3).Background(Colors.Green.Darken4).Border(0.5f).BorderColor(Colors.Black).AlignMiddle().Text("DATOS ACUDIENTE").Bold().FontColor("#fff").AlignCenter();
                                header.Cell().ColumnSpan(2).Background(Colors.Green.Darken4).Border(0.5f).BorderColor(Colors.Black).AlignMiddle().Text("DATOS JARDÍN").Bold().FontColor("#fff").AlignCenter();

                                // Fila de encabezados de columnas
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Colors.Black).AlignMiddle().Text("Identificación").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Colors.Black).AlignMiddle().Text("Nombres").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Colors.Black).AlignMiddle().Text("Edad").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Colors.Black).AlignMiddle().Text("Ciudad Nacimiento").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Colors.Black).AlignMiddle().Text("Tipo Sangre").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Colors.Black).AlignMiddle().Text("Tipo Documento").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Colors.Black).AlignMiddle().Text("Identificación").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Colors.Black).AlignMiddle().Text("Nombres").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Colors.Black).AlignMiddle().Text("Jardín").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Colors.Black).AlignMiddle().Text("MadreComunitaria").FontColor("#fff").AlignCenter();
                            });


                            foreach (var nino in listNinio)
                            {
                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Text(nino.datosBasicos.identificacion).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Text(nino.datosBasicos.nombres).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Text(nino.edad.ToString()).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Text(nino.ciudadNacimiento).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Text(nino.tipoSangre).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Text(nino.acudiente.datosBasicos.tipoDoc.tipo).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Text(nino.acudiente.datosBasicos.identificacion).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Text(nino.acudiente.datosBasicos.nombres).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Text(nino.jardin.nombre).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Colors.Black).Text(nino.madreComunitaria.datosBasicos.nombres).AlignCenter();
                            }
                        });
                    });

                    page.Footer()
                        .AlignRight()
                        .Text(txt =>
                        {
                            txt.Span("Pagina ").FontSize(10);
                            txt.CurrentPageNumber().FontSize(10);
                            txt.Span(" de ").FontSize(10);
                            txt.TotalPages().FontSize(10);
                        });
                });
            });

            return report;
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
    }
}
