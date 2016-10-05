using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProyectoDeInge.Models;

namespace ProyectoDeInge.Models
{
    public class ModeloIntermedio
    {
        public USUARIOS modeloUsuario { get; set; }
        public TELEFONOS modeloTelefono { get; set; }
        public CORREOS modeloCorreo { get; set; }
        public PROYECTO modeloProyecto { get; set; }
        public List<USUARIOS> listaUsuarios = new List<USUARIOS>();
        public List<TELEFONOS> listaTelefonos = new List<TELEFONOS>();
        public List<CORREOS> listaCorreos = new List<CORREOS>();
    }
}