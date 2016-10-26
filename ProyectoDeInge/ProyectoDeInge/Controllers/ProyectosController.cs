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

        // GET: Proyectos/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROYECTO pROYECTO = db.PROYECTO.Find(id);
            if (pROYECTO == null)
            {
                return HttpNotFound();
            }
            return View(pROYECTO);
        }

        // GET: Proyectos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Proyectos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(/*Bind(Include = "ID,NOMBRE,DESCRIPCION,FECHAINICIO,FECHAFINAL,DURACION,ESTADO")] PROYECTO pROYECTO*/ModeloIntermedio pROYECTO)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ESTADO = new SelectList(db.PROYECTO, "ESTADO");
                db.PROYECTO.Add(pROYECTO.modeloProyecto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pROYECTO);
        }

        // GET: Proyectos/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROYECTO pROYECTO = db.PROYECTO.Find(id);
            if (pROYECTO == null)
            {
                return HttpNotFound();
            }
            return View(pROYECTO);
        }

        // POST: Proyectos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NOMBRE,DESCRIPCION,FECHAINICIO,FECHAFINAL,DURACION,ESTADO")] PROYECTO pROYECTO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pROYECTO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pROYECTO);
        }

        // GET: Proyectos/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROYECTO pROYECTO = db.PROYECTO.Find(id);
            if (pROYECTO == null)
            {
                return HttpNotFound();
            }
            return View(pROYECTO);
        }

        // POST: Proyectos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PROYECTO pROYECTO = db.PROYECTO.Find(id);
            db.PROYECTO.Remove(pROYECTO);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public List<string> buscaResursos() {
            return null;
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

            List<USUARIOS> todosUsuarios = db.USUARIOS.ToList();//todos los usuarios
            List<USUARIOS> recursos = new List<USUARIOS>();
            List<USUARIOS> disponibles = new List<USUARIOS>();
            AspNetRoles rol = db.AspNetRoles.Find("2");//busco rol de desarrollador

            foreach (var item in todosUsuarios) {
                AspNetUsers asp = db.AspNetUsers.Find(item.ID_ASP);//busca AspNetUser del usuario

                if (asp.AspNetRoles.Contains(rol)) {
                    if (item.PRYCTOID == id)//si esta en el proyecto lo agrega a recursos
                    {
                        recursos.Add(item);
                    }
                    else if(item.PRYCTOID == null)//si no esta trabajando en ningun proyecto, lo agrego a recursos disponibles
                    {
                        disponibles.Add(item);
                    }
                }                
            }
            //llena SelectList de recursos asignados
            proyecto.recursosAsignados = recursos.Select(o => new SelectListItem {
                Text = o.NOMBRE + " " + o.APELLIDO1 + " " + o.APELLIDO2,
                Value = o.CEDULA.ToString()
            });
            //llena SelectList de recursos disponibles
            proyecto.otrosRecursos = disponibles.Select(o => new SelectListItem
            {
                Text = o.NOMBRE + " " + o.APELLIDO1 + " " + o.APELLIDO2,
                Value = o.CEDULA.ToString()
            });
            
            proyecto.verificaPermisos = obtienePermisos();
            
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
    }
}
