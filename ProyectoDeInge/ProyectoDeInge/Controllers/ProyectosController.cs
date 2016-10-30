using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoDeInge.Models;

namespace ProyectoDeInge.Controllers
{
    public class ProyectosController : Controller
    {
        private BD_IngeGrupo2Entities2 db = new BD_IngeGrupo2Entities2();

        // GET: Proyectos
        public ActionResult Index()
        {
            ProyectosViewModel modelo = new ProyectosViewModel();
            modelo.listaProyectos = db.PROYECTO.ToList();
            return View(modelo);
        }

        // GET: Proyectos/Create
        public ActionResult Create()
        {

            ProyectosViewModel proyecto = new ProyectosViewModel();//nuevo viewModel

            List<USUARIOS> todosUsuarios = db.USUARIOS.ToList();//todos los usuarios
            List<USUARIOS> lideres = new List<USUARIOS>();
            AspNetRoles rol = db.AspNetRoles.Find("2");//busco rol de desarrollador

            foreach (var item in todosUsuarios)
            {
                AspNetUsers asp = db.AspNetUsers.Find(item.ID_ASP);//busca AspNetUser del usuario

                if (asp.AspNetRoles.Contains(rol))
                {
                    if ((item.LIDER == true) && (item.PRYCTOID == null))//si esta en el proyecto lo agrega a recursos
                    {
                        lideres.Add(item);
                        //item.PRYCTOID = proyecto.modeloProyecto.ID;
                    }
                }
            }
            //llena SelectList de recursos asignados
            proyecto.lideres = lideres.Select(o => new SelectListItem
            {
                Text = o.NOMBRE + " " + o.APELLIDO1 + " " + o.APELLIDO2 + " - " + o.CEDULA,
                Value = o.NOMBRE + " " + o.APELLIDO1 + " " + o.APELLIDO2 + " - " + o.CEDULA
                //Value = o.CEDULA.ToString()
            });
            //ViewBag.liderProyecto = new SelectList(db.USUARIOS.Where(r => r.LIDER.Equals(true)), "ID", "NOMBRE");
            ViewBag.liderProyecto = new SelectList(proyecto.lideres, "Text", "Value");
            proyecto.verificaPermisos = obtienePermisos();

            return View();
        }


        // POST: Proyectos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(/*Bind(Include = "ID,NOMBRE,DESCRIPCION,FECHAINICIO,FECHAFINAL,DURACION,ESTADO")] PROYECTO pROYECTO*/ ModeloIntermedio pROYECTO)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ESTADO = new SelectList(db.PROYECTO, "ESTADO");
                db.PROYECTO.Add(pROYECTO.modeloProyecto);

                var proyectoID = pROYECTO.modeloUsuario.PRYCTOID;
                string cedula = string.Empty;
                int val;

                for (int i = 0; i < proyectoID.Length; i++)
                {
                    if (Char.IsDigit(proyectoID[i]))
                        cedula += proyectoID[i];
                }

                if (cedula.Length > 0)
                {
                    val = int.Parse(cedula);
                }
                USUARIOS uSUARIO = db.USUARIOS.Find(cedula);
                uSUARIO.PRYCTOID = pROYECTO.modeloProyecto.ID;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pROYECTO);
        }        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public List<USUARIOS> buscaUsuariosRol(string rolId)
        {
            List<USUARIOS> usuariosDeRol = new List<USUARIOS>();
            var todosUsuarios = db.USUARIOS.ToList();
            AspNetRoles rol = db.AspNetRoles.Find(rolId);

            foreach (var item in todosUsuarios)
            {
                AspNetUsers user = db.AspNetUsers.Find(item.ID_ASP);
                if (user.AspNetRoles.Contains(rol))
                {
                    usuariosDeRol.Add(item);
                }
            }
            return usuariosDeRol;
        }

        public List<USUARIOS> buscaResursos()
        {
            return buscaUsuariosRol("2");
        }

        public void populateUsuarios(PROYECTO proyecto)
        {
            var todosRecursos = buscaResursos();
            /*var todosRecursos = db.USUARIOS.ToList();*/		//(where rol = 2)  --desarrolladores

            var recursos = new HashSet<string>(proyecto.USUARIOS.Select(u => u.CEDULA));
            var recursosAsignados = new List<RecursosViewModel>();
            var recursosDisponibles = new List<RecursosViewModel>();
            foreach (var rec in todosRecursos)
            {
                if (rec.PRYCTOID == proyecto.ID)
                {
                    recursosAsignados.Add(new RecursosViewModel
                    {
                        Cedula = rec.CEDULA,                        
                        Nombre = rec.NOMBRE,
                        usuarioProyecto = rec.PRYCTOID,                        
                        Apellido1 = rec.APELLIDO1,                        
                        Apelliso2 = rec.APELLIDO2,
                        usuarioID = rec.ID_ASP
                    });
                }
                else
                {//falta meter esto en un if(rec.PRYCTOID == null), para que no cargue los que ya estan trabajando en otro proyecto
                    recursosDisponibles.Add(new RecursosViewModel
                    {
                        Cedula = rec.CEDULA,
                        Nombre = rec.NOMBRE,
                        usuarioProyecto = rec.PRYCTOID,
                        Apellido1 = rec.APELLIDO1,
                        Apelliso2 = rec.APELLIDO2,
                        usuarioID = rec.ID_ASP
                    });
                }
            }
            ViewBag.Asignados = new MultiSelectList(recursosAsignados, "Cedula", "nombreCompleto"/*, "Apellido1", "Apellido2"*/);
            ViewBag.Disponibles = new MultiSelectList(recursosDisponibles, "Cedula", "nombreCompleto"/*, "Apellido1", "Apellido2"*/);
        }

        public HashSet<string> obtienePermisos() {
            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual
            HashSet<string> permisos = new HashSet<string>();
            var listauser = db.AspNetUsers.ToArray();

            for (int i = 0; i < listauser.Length; i++)
            {                           //de todos los AspNetUser del sistema, encuentra el que tenga el email activo actualmente
                if (listauser[i].Email == User.Identity.Name)
                {
                    fg = listauser[i];                  //obtiene el AspNetUser actual
                }
            }

            AspNetRoles role = fg.AspNetRoles.First();  //consigue el rol del usuario
            var per = role.PERMISOS;                    //copia los permisos que tiene asignado

            foreach (PERMISOS p in role.PERMISOS)
            {     //los copia a un HashSet<string>
                permisos.Add(p.ID);
            }
            return permisos;
        }

        public ActionResult Unificado(string id) {
            if (id == null) {//si id es null
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProyectosViewModel proyecto = new ProyectosViewModel();//nuevo viewModel
            proyecto.modeloProyecto = db.PROYECTO.Find(id);//encuentra al proyecto
            if (proyecto.modeloProyecto == null) {//si el proyecto no existe
                return HttpNotFound();
            }
                        
            proyecto.verificaPermisos = obtienePermisos();
            populateUsuarios(proyecto.modeloProyecto);

            return View(proyecto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unificado(ProyectosViewModel modelo) {
            if (ModelState.IsValid)
            {
                db.Entry(modelo.modeloProyecto).State = EntityState.Modified;
                var id = modelo.modeloProyecto.ID;
                /*asignar y desasignar recursos*/
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(modelo);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //Eliminar Proyecto
        public ActionResult eliminarProyecto(string id)
        {
            PROYECTO proyect = db.PROYECTO.Find(id);
            var state = "Finalizado";
            if (proyect.ESTADO == state)
            {
                db.PROYECTO.Remove(proyect);
                db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }


    }
}
