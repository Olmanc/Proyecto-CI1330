using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoDeInge.Models
{
    public class permisosViewModel
    {
        public AspNetRoles role { get; set; }
        public IEnumerable<SelectListItem> listaPermisos { get; set; }

        public List<AspNetRoles> todoRoles = new List<AspNetRoles>();
        public List<PERMISOS> todoPermisos = new List<PERMISOS>();
        //public Gate gate = new Gate();

        private List<string> _permisosAsignados;
        public List<string> permisosAsignados
        {
            get
            {
                if (_permisosAsignados == null)
                {
                    _permisosAsignados = role.PERMISOS.Select(m => m.ID).ToList();
                }
                return _permisosAsignados;
            }
            set { _permisosAsignados = value; }
        }
    }

    public class permiso2Viewmodel
    {
        public AspNetRoles Admin { get; set; }
        public AspNetRoles Desa { get; set; }
        public AspNetRoles Usuario { get; set; }
        public List<AspNetRoles> Roles = new List<AspNetRoles>();
        public List<PERMISOS> Permisos = new List<PERMISOS>();
        public HashSet<string> verificaPermisos = new HashSet<string>();   //para verificar permisos
    }
}
