using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static ICBFApp.Pages.Rol.IndexModel;
using static ICBFApp.Pages.TipoDocumento.IndexModel;
using static ICBFApp.Pages.Usuario.IndexModel;
using System.Data.SqlClient;
using static ICBFApp.Pages.Ninio.IndexModel;
using System.Data;

namespace ICBFApp.Pages.AvancesAcademicos
{
    public class IndexModel : PageModel
    {

        public List<AvanceAcademicoInfo> listAvanceAcademico = new List<AvanceAcademicoInfo>();

        public string SuccessMessage { get; set; }

        private readonly string _connectionString;

        public IndexModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionSQLServer");
        }

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
                    String sqlSelect = "SELECT DatosBasicos.nombres, DatosBasicos.identificacion, AvancesAcademicos.nivel, AvancesAcademicos.notas, AvancesAcademicos.descripcion, AvancesAcademicos.fechaEntrega, AvancesAcademicos.idAvanceAcademico " +
                        "FROM AvancesAcademicos " +
                        "INNER JOIN Ninos ON AvancesAcademicos.idNino = Ninos.idNino " +
                        "INNER JOIN DatosBasicos ON Ninos.idDatosBasicos = DatosBasicos.idDatosBasicos;";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Validar si hay datos
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    // Crear un nuevo objeto DatosBasicosInfo
                                    DatosBasicosInfo datosBasicosInfo = new DatosBasicosInfo();
                                    datosBasicosInfo.nombres = reader.GetString(0).ToString();  // Nombre
                                    datosBasicosInfo.identificacion = reader.GetString(1).ToString();  // Identificación

                                    // Crear un nuevo objeto AvanceAcademicoInfo
                                    AvanceAcademicoInfo avanceAcademicoInfo = new AvanceAcademicoInfo();
                                    avanceAcademicoInfo.nivel = reader.GetString(2).ToString();  // Nivel
                                    avanceAcademicoInfo.notas = reader.GetString(3).ToString();  // Notas
                                    avanceAcademicoInfo.descripcion = reader.GetString(4).ToString();  // Descripción
                                    avanceAcademicoInfo.fechaEntrega = reader.GetDateTime(5).Date.ToShortDateString();
                                    avanceAcademicoInfo.idAvanceAcademico = reader.GetInt32(6).ToString();

                                    // Asignar datosBasicosInfo al avanceAcademicoInfo
                                    avanceAcademicoInfo.datosBasicosInfo = datosBasicosInfo;

                                    // Agregar avanceAcademicoInfo a la lista
                                    listAvanceAcademico.Add(avanceAcademicoInfo);
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

        public class AvanceAcademicoInfo
        {
            public string idAvanceAcademico { get; set; }
            public string nivel {  get; set; }
            public string notas { get; set; }
            public string descripcion { get; set; }
            public string fechaEntrega { get; set; }
            public DatosBasicosInfo datosBasicosInfo { get; set; }
            public NinioInfo ninioInfo { get; set; }
            public UsuarioInfo usuarioInfo { get; set; }
        }

    }
}
