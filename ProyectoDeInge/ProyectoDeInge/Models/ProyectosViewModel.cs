using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Web.Mvc;

namespace ProyectoDeInge.Models
{
    public class ProyectosViewModel
    {
        public IEnumerable<SelectListItem> lideres { get; set; }
        public USUARIOS modeloUsuario { get; set; } //usuario que es lider del proyecto

        public PROYECTO modeloProyecto { get; set; }

        //public List<string> listaRecursos = new List<string>();

        //public List<string> recursosDisponibles = new List<string>();

        public List<RecursosViewModel> listaRecursos = new List<RecursosViewModel>();
        public List<RecursosViewModel> listaDisponibless = new List<RecursosViewModel>();

        //public List<string> listaRequerimientos = new List<string>();

        public List<PROYECTO> listaProyectos = new List<PROYECTO>();
        
        public IEnumerable<SelectListItem> recursosAsignados { get; set; }
        public IEnumerable<SelectListItem> otrosRecursos { get; set; }
        

        public HashSet<string> verificaPermisos = new HashSet<string>();        //para verificar los permisos del usuario actual
    }
}