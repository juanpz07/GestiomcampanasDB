using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace CapaDatos
{
    public class IncidenciaCD
    {
        private readonly Conexion conexion = new Conexion();

        // Método para Listar / Filtrar Incidencias
        public DataTable ListarIncidencias(string filtro)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_ListarIncidencias", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Filtro", string.IsNullOrEmpty(filtro) ? (object)DBNull.Value : filtro);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(tabla);
            }
            return tabla;
        }

        // Método para Insertar Incidencia
        public bool InsertarIncidencia(int ventaId, int tipoProblemaId, string observacion, string estado)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarIncidencia", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VentaID", ventaId);
                cmd.Parameters.AddWithValue("@TipoProblemaID", tipoProblemaId);
                cmd.Parameters.AddWithValue("@Observacion", observacion);
                cmd.Parameters.AddWithValue("@Estado", estado);

                con.Open();
                int filasAfectadas = cmd.ExecuteNonQuery();
                return filasAfectadas > 0;
            }
        }

        // Método para Obtener Ventas (para los desplegables)
        public DataTable ObtenerVentas()
        {
            DataTable tabla = new DataTable();
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerVentas", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(tabla);
            }
            return tabla;
        }

        // Método para Obtener Tipos de Problema (para los desplegables)
        public DataTable ObtenerTiposProblema()
        {
            DataTable tabla = new DataTable();
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerTiposProblema", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(tabla);
            }
            return tabla;
        }

        // Método para Obtener Logs de Auditoría para el PDF
        public DataTable ObtenerLogsAuditoria()
        {
            DataTable tabla = new DataTable();
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                string query = "SELECT LogID, TablaAfectada, Operacion, Detalle, FechaRegistro, UsuarioBD FROM AuditoriaLog ORDER BY LogID DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(tabla);
            }
            return tabla;
        }
    }
}