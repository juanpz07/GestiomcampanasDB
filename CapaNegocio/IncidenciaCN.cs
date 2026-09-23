using System.Data;
using CapaDatos;

namespace CapaNegocio
{
    public class IncidenciaCN
    {
        private readonly IncidenciaCD incidenciaCD = new IncidenciaCD();

        public DataTable ListarIncidencias(string filtro)
        {
            return incidenciaCD.ListarIncidencias(filtro);
        }

        public bool RegistrarIncidencia(int ventaId, int tipoProblemaId, string observacion, string estado)
        {
            return incidenciaCD.InsertarIncidencia(ventaId, tipoProblemaId, observacion, estado);
        }

        public DataTable ObtenerVentas()
        {
            return incidenciaCD.ObtenerVentas();
        }

        public DataTable ObtenerTiposProblema()
        {
            return incidenciaCD.ObtenerTiposProblema();
        }

        public DataTable ObtenerLogsAuditoria()
        {
            return incidenciaCD.ObtenerLogsAuditoria();
        }
    }
}