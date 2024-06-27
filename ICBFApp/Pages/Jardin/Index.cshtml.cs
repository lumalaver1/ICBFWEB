using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace ICBFApp.Pages.Jardin
{
    public class IndexModel : PageModel
    {
        private readonly string _connectionString;

        public IndexModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionSQLServer");
        }

        public List<JardinInfo> listJardin = new List<JardinInfo>();
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
                    String sqlSelect = "SELECT * FROM Jardines";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Validar si hay datos
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    JardinInfo jardinInfo = new JardinInfo();
                                    jardinInfo.idJardin = reader.GetInt32(0).ToString();
                                    jardinInfo.nombre = reader.GetString(1);
                                    jardinInfo.direccion = reader.GetString(2);
                                    jardinInfo.estado = reader.GetString(3);

                                    listJardin.Add(jardinInfo);
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

        public class JardinInfo
        {
            public string idJardin { get; set; }
            public string nombre { get; set; }
            public string direccion { get; set; }
            public string estado { get; set; }

        }
    }
}
