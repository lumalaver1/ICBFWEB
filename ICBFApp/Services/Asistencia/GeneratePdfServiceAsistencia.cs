using System.Data.SqlClient;
using ICBFApp.Services.Ninio;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using static ICBFApp.Pages.Asistencia.IndexModel;
using static ICBFApp.Pages.Jardin.IndexModel;
using static ICBFApp.Pages.Ninio.IndexModel;
using static ICBFApp.Pages.Usuario.IndexModel;
using static QuestPDF.Helpers.Colors;

namespace ICBFApp.Services.Asistencia
{
    public class GeneratePdfServiceAsistencia : IGeneratePdfServiceAsistencia
    {
        private readonly IWebHostEnvironment _host;
        private readonly string _connectionString;

        public GeneratePdfServiceAsistencia(IWebHostEnvironment host, IConfiguration configuration)
        {
            _host = host;
            _connectionString = configuration.GetConnectionString("ConexionSQLServer");
        }

        public List<AsistenciaInfo> asistenciaInfo = new List<AsistenciaInfo>();

        public void GetData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sqlSelect = "SELECT n.idNino, d.nombres, d.identificacion, j.nombre, j.direccion, a.fecha, a.estadoNino, " +
                        "(SELECT nombres FROM DatosBasicos as d INNER JOIN Usuarios as u ON d.idDatosBasicos = u.idDatosBasicos WHERE n.idMadreComunitaria = u.idUsuario) as madreComunitaria " +
                        "FROM Ninos as n " +
                        "INNER JOIN DatosBasicos as d ON n.idDatosBasicos = d.idDatosBasicos " +
                        "INNER JOIN Asistencias as a ON n.idNino = a.idNino " +
                        "INNER JOIN Jardines as j ON n.idJardin = j.idJardin; ";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Validar si hay datos
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    NinioInfo ninio = new NinioInfo();
                                    ninio.idNinio = reader.GetInt32(0).ToString();

                                    DatosBasicosInfo datosBasicos = new DatosBasicosInfo();
                                    datosBasicos.nombres = reader.GetString(1);
                                    datosBasicos.identificacion = reader.GetString(2);

                                    JardinInfo jardin = new JardinInfo();
                                    jardin.nombre = reader.GetString(3);
                                    jardin.direccion = reader.GetString(4);

                                    AsistenciaInfo asistencia = new AsistenciaInfo();
                                    asistencia.fecha = reader.GetDateTime(5).Date.ToShortDateString();
                                    asistencia.estadoNino = reader.GetString(6);

                                    DatosBasicosInfo datosBasicosMadreCom = new DatosBasicosInfo();
                                    datosBasicosMadreCom.nombres = reader.GetString(7);

                                    UsuarioInfo madreComunitaria = new UsuarioInfo();
                                    madreComunitaria.datosBasicos = datosBasicosMadreCom;

                                    // Asignar objetos a la asistencia
                                    asistencia.datosBasicosInfo = datosBasicos;
                                    asistencia.ninioInfo = ninio;
                                    ninio.jardin = jardin;
                                    ninio.madreComunitaria = madreComunitaria;

                                    // Añadir la asistencia a la lista
                                    asistenciaInfo.Add(asistencia);
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
                    page.PageColor(White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Row(row =>
                    {
                        var rutaImgSena = Path.Combine(_host.WebRootPath, "images/logoSena.png");
                        byte[] imageDataSena = File.ReadAllBytes(rutaImgSena);

                        var rutaImgICBF = Path.Combine(_host.WebRootPath, "images/logoICBF.png");
                        byte[] imageDataICBF = File.ReadAllBytes(rutaImgICBF);

                        row.ConstantItem(75).AlignMiddle().Height(50).Image(imageDataSena);
                        row.ConstantItem(75).AlignMiddle().Height(65).Image(imageDataICBF);

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Height(20).Text("Asociación del ICBF").Bold().FontSize(14).AlignRight();
                            col.Item().Height(20).Text("Reporte de Asistencia").Bold().AlignRight();
                            col.Item().Height(20).Text("Fecha de emisión: " + today.Date.ToShortDateString()).AlignRight();
                        });
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().PaddingVertical(10).AlignCenter()
                        .Text("Listado de Asistencias")
                        .Bold().FontSize(24).FontColor("#39a900");

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);
                                columns.RelativeColumn(1.2f);
                                columns.ConstantColumn(90);
                                columns.ConstantColumn(100);
                                columns.RelativeColumn(1.2f);
                                columns.ConstantColumn(200);
                                columns.RelativeColumn(1.2f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().ColumnSpan(2).Background(Green.Darken4).Border(0.5f).BorderColor(Black).AlignMiddle().Text("DATOS NIÑO").Bold().FontColor("#fff").AlignCenter();
                                header.Cell().ColumnSpan(2).Background(Green.Darken4).Border(0.5f).BorderColor(Black).AlignMiddle().Text("DATOS ASISTENCIA").Bold().FontColor("#fff").AlignCenter();
                                header.Cell().ColumnSpan(3).Background(Green.Darken4).Border(0.5f).BorderColor(Black).AlignMiddle().Text("DATOS JARDÍN").Bold().FontColor("#fff").AlignCenter();

                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Black).AlignMiddle().Text("Identificacion").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Black).AlignMiddle().Text("Nombres").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Black).AlignMiddle().Text("Estado Niño").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Black).AlignMiddle().Text("Fecha").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Black).AlignMiddle().Text("Nombre").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Black).AlignMiddle().Text("Direccion").FontColor("#fff").AlignCenter();
                                header.Cell().Background("#212529").Border(0.5f).BorderColor(Black).AlignMiddle().Text("Madre Comunitaria").FontColor("#fff").AlignCenter();
                            });

                            foreach (var asistencia in asistenciaInfo)
                            {
                                table.Cell().Border(0.5f).BorderColor(Black).Text(asistencia.datosBasicosInfo.identificacion).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Black).Text(asistencia.datosBasicosInfo.nombres).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Black).Text(asistencia.estadoNino).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Black).Text(asistencia.fecha.ToString()).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Black).Text(asistencia.ninioInfo.jardin.nombre).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Black).Text(asistencia.ninioInfo.jardin.direccion).AlignCenter();
                                table.Cell().Border(0.5f).BorderColor(Black).Text(asistencia.ninioInfo.madreComunitaria.datosBasicos.nombres).AlignCenter();
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
    }
}
