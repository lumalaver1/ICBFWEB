using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace ICBFApp.Pages.Rol
{
    public class IndexModel : PageModel
    {
        /* ESTA VISTA Y TODO EL CRUD SOBRARIA, PORQUE LOS ROLES YA DEBERIAN 
         * ESTAR QUEMADOS/DEFINIDOS A NIVEL BASE DE DATOS*/

        private readonly string _connectionString;

        public IndexModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionSQLServer");
        }

        public List<RolInfo> listRol = new List<RolInfo>();
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
                    String sqlSelect = "SELECT * FROM Roles";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Validar si hay datos
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    RolInfo rolInfo = new RolInfo();
                                    rolInfo.idRol = reader.GetInt32(0).ToString();
                                    rolInfo.nombre = reader.GetString(1);

                                    listRol.Add(rolInfo);
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

        public class RolInfo
        {
            public string idRol { get; set; }
            public string nombre { get; set; }

        }
    }
}
