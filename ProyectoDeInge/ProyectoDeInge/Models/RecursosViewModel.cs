using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoDeInge.Models
{
    public class RecursosViewModel
    {
        public string Cedula { get; set; }
        public string /*usuario*/Nombre { get; set; }
        public string usuarioProyecto { get; set; }
        public string /*usuario*/Apellido1 { get; set; }
        public string /*usuario*/Apelliso2 { get; set; }
        public string usuarioID { get; set; }
        public bool usuarioLider { get; set; }

        public string nombreCompleto { get { return string.Format("{0} {1} {2}", Nombre, Apellido1, Apelliso2); } }
    }
}