using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoDeInge.Models
{
   public class CambiosViewModel
    {
        //public USUARIOS solicitante { get; set; }
        //public USUARIOS revision { get; set; }
        //public REQUERIMIENTOS requerimiento { get; set; }
        public CAMBIOS cambio { get; set; }
        public CAMBIOS actual { get; set; }

        public List<CAMBIOS> listaCambios = new List<CAMBIOS>();
        //public List<USUARIOS> listaUsuarios = new List<USUARIOS>();

        //PARA SOLICITUD DE CAMBIOS se necesita
        public CAMBIOS solicitud { get; set; }
        public REQUERIMIENTOS propuesto { get; set; }   // Nuevo requerimiento propuesto
        public REQUERIMIENTOS vigente { get; set; }     // Requerimiento actual (ultimo version aprobada)
    }
}
