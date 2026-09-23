using Microsoft.Data.SqlClient;

namespace CapaDatos
{
    public class Conexion
    {
        // Cambiamos Database=TiendaDb por Database=GestionCampanasDB
        private readonly string cadenaConexion = @"Server=DESKTOP-AMI0R55;Database=GestionCampanasDB;User Id=SA;Password=User.sede;TrustServerCertificate=True;";

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}