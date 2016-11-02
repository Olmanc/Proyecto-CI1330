﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoDeInge.Models;
using System.Data.Entity.Infrastructure;

namespace ProyectoDeInge.Controllers
{
    public class ProyectosController : Controller
    {
        private BD_IngeGrupo2Entities2 db = new BD_IngeGrupo2Entities2();

        // GET: Proyectos
        public ActionResult Index()
        {
            ProyectosViewModel modelo = new ProyectosViewModel();
            modelo.listaProyectos = db.PROYECTO.ToList(); //lista de todos los proyectos en la BD
            List<PROYECTO> todoProyectos = db.PROYECTO.ToList(); //lista de todos los proyectos en la BD
            foreach (var proyecto in todoProyectos)
            {
                if (proyecto.BORRADO == true)   //si el proyecto estaba cancelado y fue borrado de la aplicación
                {                               // pero este no siempre permanece en la BD
                    modelo.listaProyectos.Remove(proyecto); //elimino proyectos que no se deben mostrar en la aplicación
                }
            }
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
                Value = o.NOMBRE + " " + o.APELLIDO1 + " " + o.APELLIDO2 
                //Value = o.CEDULA.ToString()
            });
            //ViewBag.liderProyecto = new SelectList(db.USUARIOS.Where(r => r.LIDER.Equals(true)), "ID", "NOMBRE");
            ViewBag.liderProyecto = new SelectList(proyecto.lideres, "Text", "Value");
            proyecto.verificaPermisos = obtienePermisos();


            populateUsuarios_Create(proyecto.modeloProyecto);

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
                ProyectosViewModel proyecto = new ProyectosViewModel();//nuevo viewModel
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

                if (pROYECTO.modeloProyecto.FECHAINICIO == null) //fecha por defecto: La del día de creación del proyecto
                {
                    var fechaInicial = DateTime.Now;
                    pROYECTO.modeloProyecto.FECHAINICIO = fechaInicial;
                }

                //if ((pROYECTO.modeloProyecto.FECHAINICIO != null) && (pROYECTO.modeloProyecto.FECHAFINAL != null) && ((pROYECTO.modeloProyecto.DURACION == null) || (pROYECTO.modeloProyecto.DURACION == 0)))
                //{
                //    bool b = false;
                //    var fechaInicial = pROYECTO.modeloProyecto.FECHAINICIO.;
                //    var fechaFinal = pROYECTO.modeloProyecto.FECHAFINAL;
                //    var duracion = fechaFinal - fechaInicial;
                //    var d = duracion.ToString();
                    
                //    //for (int i = 0; ((i < d.Length) && (b == false)); i++)
                //    //{
                //    //    if (Char.IsDigit('.'))
                //    //        b = true;
                //    //        for (int j = 0; j < i; j++)
                //    //        {
                                
                           
                //    }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pROYECTO);
        }


        public HashSet<string> obtienePermisos()
        {
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

        public void populateUsuarios_Create(PROYECTO proyecto)
        {
            var todosRecursos = buscaResursos();
            /*var todosRecursos = db.USUARIOS.ToList();*/		//(where rol = 2)  --desarrolladores

            var recursosAsignados = new List<RecursosViewModel>();
            var recursosDisponibles = new List<RecursosViewModel>();
            foreach (var rec in todosRecursos)
            {
                if (rec.PRYCTOID == null)
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

        public ActionResult Unificado(string id)
        {
            if (id == null)
            {//si id es null
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProyectosViewModel proyecto = new ProyectosViewModel();//nuevo viewModel


            //probar este
            //proyecto.modeloProyecto = db.PROYECTO.Include(p => p.USUARIOS).Where(i => i.ID == id).Single();
            proyecto.modeloProyecto = db.PROYECTO.Find(id);//encuentra al proyecto
            

            if (proyecto.modeloProyecto == null)
            {//si el proyecto no existe
                return HttpNotFound();
            }

        public ActionResult Unificado(string id) {
            if (id == null){//si id es null
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProyectosViewModel proyecto = new ProyectosViewModel();//nuevo viewModel


            //probar este
            //proyecto.modeloProyecto = db.PROYECTO.Include(p => p.USUARIOS).Where(i => i.ID == id).Single();
            proyecto.modeloProyecto = db.PROYECTO.Find(id);//encuentra al proyecto


            if (proyecto.modeloProyecto == null)
            {//si el proyecto no existe
                return HttpNotFound();
            }

            List<USUARIOS> lideres = new List<USUARIOS>();

            proyecto.verificaPermisos = obtienePermisos();
            populateUsuarios(proyecto.modeloProyecto);

            return View(proyecto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unificado(ProyectosViewModel modelo, string[] recursosAsignados, string[] recursosDisponibles)
        {

            if (modelo.modeloProyecto == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (modelo.modeloProyecto.ESTADO == "Finalizado")
            {
                //revisa requerimientos
                var requerimientos = db.REQUERIMIENTOS.ToList();
                foreach (var item in requerimientos)
                {
                    if (item.PRYCTOID == modelo.modeloProyecto.ID)
                    {
                        if (item.ESTADO == "Finalizado" || item.ESTADO == "Cancelado")
                        {

                        }
                        else
                        {
                            //mensaje de error
                        }
                    }
                }
            }
            try
            {
                if (recursosAsignados != null) {
                    foreach (var r in recursosAsignados)
                    {
                        var variable = db.USUARIOS.Find(r);
                        variable.PRYCTOID = modelo.modeloProyecto.ID;
                        db.Entry(variable).State = EntityState.Modified;
                        db.SaveChanges();
                        modelo.modeloProyecto.USUARIOS.Add(variable);
                    }
                }
                if (recursosDisponibles != null) {
                    foreach (var r in recursosDisponibles)
                    {
                        var variable = db.USUARIOS.Find(r);
                        variable.PRYCTOID = null;
                        db.Entry(variable).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                

                db.Entry(modelo.modeloProyecto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (RetryLimitExceededException)
            {
                //mensaje?
                ModelState.AddModelError("", "No fue posible modificar los datos del proyecto.");
            }

            //mensaje de error
            modelo.verificaPermisos = obtienePermisos();
            populateUsuarios(modelo.modeloProyecto);
            return View(modelo);

            //return null;


            //if (ModelState.IsValid)
            //{
            //    db.Entry(modelo.modeloProyecto).State = EntityState.Modified;
            //    var id = modelo.modeloProyecto.ID;
            //    /*asignar y desasignar recursos*/
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            //return View(modelo);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //Eliminar Proyecto
        public ActionResult eliminarProyecto(string id)
        {
            PROYECTO proyect = db.PROYECTO.Find(id);
            List<USUARIOS> users = db.USUARIOS.ToList();    //lista de todos los usuarios
            if (proyect.ESTADO == "Finalizado" || proyect.ESTADO == "Cerrado")
            {
                //db.PROYECTO.Remove(proyect);  //esto no porque solo se borra en la aplicación pero debe permanecer en la BD
                proyect.BORRADO = true; //para indicar que no se debe mostrar en la aplicación
                //db.SaveChanges();
                foreach (var persona in users)
                {
                    if (persona.PRYCTOID == id) //si el usuario trabaja en el proyecto a borrar
                    {
                        persona.PRYCTOID = null; // cambie el proyecto que trabaja a NULL
                    }
                }
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
