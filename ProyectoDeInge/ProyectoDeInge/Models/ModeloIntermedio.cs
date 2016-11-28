using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProyectoDeInge.Models;
using System.Web.Mvc;

namespace ProyectoDeInge.Models
{
    public class ModeloIntermedio
    {
        public USUARIOS modeloUsuario { get; set; }
        public TELEFONOS modeloTelefono { get; set; }
        public TELEFONOS modeloTelefono2 { get; set; }
        public CORREOS modeloCorreo { get; set; }
        public CORREOS modeloCorreo2 { get; set; }
        public PROYECTO modeloProyecto { get; set; }
        public AspNetUsers modeloAsp { get; set; }
        public string rol { get; set; }
        //public List<string> listaRol = new List<string>();
        public List<USUARIOS> listaUsuarios = new List<USUARIOS>();
        public List<TELEFONOS> listaTelefonos = new List<TELEFONOS>();
        public List<CORREOS> listaCorreos = new List<CORREOS>();
        public List<PROYECTO> listaProyecto = new List<PROYECTO>();
        
        public HashSet<string> verificaPermisos = new HashSet<string>();        //para verificar los permisos del usuario actual
    }
}
