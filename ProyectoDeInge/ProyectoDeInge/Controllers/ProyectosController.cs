using System;
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
            modelo.verificaPermisos = obtienePermisos();
            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual            
            var listauser = db.AspNetUsers.ToArray();

            for (int i = 0; i < listauser.Length; i++)
            {                           //de todos los AspNetUser del sistema, encuentra el que tenga el email activo actualmente
                if (listauser[i].Email == User.Identity.Name)
                {
                    fg = listauser[i];                  //obtiene el AspNetUser actual
                }
            }

            var rol = fg.AspNetRoles.First();
            if (rol.Id == "2") {                
                USUARIOS actual = db.USUARIOS.Where(i => i.ID_ASP == fg.Id).Single();
                if (actual.PRYCTOID != null && actual.LIDER != null) {
                    if (modelo.modeloProyecto != null) {
                        if (modelo.modeloProyecto.ID != null)
                        {
                            if (actual.PRYCTOID == modelo.modeloProyecto.ID && actual.LIDER == true)
                            {//permisos sobre los proyectos
                                modelo.verificaPermisos.Add("06");
                                modelo.verificaPermisos.Add("07");
                                modelo.verificaPermisos.Add("08");
                                modelo.verificaPermisos.Add("09");
                                modelo.verificaPermisos.Add("10");
                                modelo.verificaPermisos.Add("11");
                                modelo.verificaPermisos.Add("12");
                                modelo.verificaPermisos.Add("13");
                                modelo.verificaPermisos.Add("14");
                                modelo.verificaPermisos.Add("15");
                                modelo.verificaPermisos.Add("16");
                                modelo.verificaPermisos.Add("17");
                            }
                        }
                    } 
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
            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual            
            var listauser = db.AspNetUsers.ToArray();

            for (int i = 0; i < listauser.Length; i++)
            {                           //de todos los AspNetUser del sistema, encuentra el que tenga el email activo actualmente
                if (listauser[i].Email == User.Identity.Name)
                {
                    fg = listauser[i];                  //obtiene el AspNetUser actual
                }
            }
            var role = fg.AspNetRoles.First();
            if (role.Id == "2")
            {
                USUARIOS actual = db.USUARIOS.Where(i => i.ID_ASP == fg.Id).Single();
                if (actual.PRYCTOID != null && actual.LIDER != null)
                {
                    if (proyecto.modeloProyecto != null)
                    {
                        if (proyecto.modeloProyecto.ID != null)
                        {
                            if (actual.PRYCTOID == proyecto.modeloProyecto.ID && actual.LIDER == true)
                            {//permisos sobre los proyectos
                                proyecto.verificaPermisos.Add("06");
                                proyecto.verificaPermisos.Add("07");
                                proyecto.verificaPermisos.Add("08");
                                proyecto.verificaPermisos.Add("09");
                                proyecto.verificaPermisos.Add("10");
                                proyecto.verificaPermisos.Add("11");
                                proyecto.verificaPermisos.Add("12");
                                proyecto.verificaPermisos.Add("13");
                                proyecto.verificaPermisos.Add("14");
                                proyecto.verificaPermisos.Add("15");
                                proyecto.verificaPermisos.Add("16");
                                proyecto.verificaPermisos.Add("17");
                            }
                        }
                    }
                }
            }

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

                if ((pROYECTO.modeloProyecto.FECHAINICIO != null) && (pROYECTO.modeloProyecto.FECHAFINAL != null) && ((pROYECTO.modeloProyecto.DURACION == null) || (pROYECTO.modeloProyecto.DURACION == 0)))
                {
                    bool b = false;
                    var dias = "";
                    int totalDias = 0;
                    var fechaInicial = pROYECTO.modeloProyecto.FECHAINICIO;
                    var fechaFinal = pROYECTO.modeloProyecto.FECHAFINAL;
                    var duracion = fechaFinal - fechaInicial;
                    var d = duracion.ToString();

                    for (int i = 0; ((i < d.Length) && (b == false)); i++)
                    {
                        if (d[i].Equals('.'))
                        {
                            b = true;
                            for (int j = 0; j < i; j++)
                            {
                                dias += d[j]; 
                            }
                            totalDias = Int32.Parse(dias.ToString());
                        }
                    }
                    int meses = totalDias / 30;
                    totalDias = 67890;
                }
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
                {
                    if ((rec.PRYCTOID == null) && (rec.LIDER == false))
                    {
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
            //var opcionLideres = new List<RecursosViewModel>();
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
                       
            ViewBag.Asignados = new MultiSelectList(recursosAsignados, "Cedula", "nombreCompleto");
            ViewBag.Disponibles = new MultiSelectList(recursosDisponibles, "Cedula", "nombreCompleto");
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

            ////
            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual            
            var listauser = db.AspNetUsers.ToArray();

            for (int i = 0; i < listauser.Length; i++)
            {                           //de todos los AspNetUser del sistema, encuentra el que tenga el email activo actualmente
                if (listauser[i].Email == User.Identity.Name)
                {
                    fg = listauser[i];                  //obtiene el AspNetUser actual
                }
            }
            USUARIOS actual = db.USUARIOS.Where(i => i.ID_ASP == fg.Id).Single();
            if (actual.PRYCTOID == proyecto.modeloProyecto.ID && actual.LIDER == true)
            {//permisos sobre los proyectos
                proyecto.verificaPermisos.Add("06");
                proyecto.verificaPermisos.Add("07");
                proyecto.verificaPermisos.Add("08");
                proyecto.verificaPermisos.Add("09");
                proyecto.verificaPermisos.Add("10");
                proyecto.verificaPermisos.Add("11");
                proyecto.verificaPermisos.Add("12");
                proyecto.verificaPermisos.Add("13");
                proyecto.verificaPermisos.Add("14");
                proyecto.verificaPermisos.Add("15");
                proyecto.verificaPermisos.Add("16");
                proyecto.verificaPermisos.Add("17");
            }


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
                            Response.Write("<Script>alert('ERROR - No es posible cambiar el estado del proyecto a Finalizado. Todavia tiene requerimientos sin finalizar/cancelar')</Script>");
                            return this.Unificado(modelo.modeloProyecto.ID);
                            //return this.RedirectToAction("Index", "Proyectos");
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
                
                if (modelo.modeloProyecto.FECHAINICIO == null) //fecha por defecto: La del día de creación del proyecto
                {
                    var fechaInicial = DateTime.Now;
                    modelo.modeloProyecto.FECHAINICIO = fechaInicial;
                }

                if ((modelo.modeloProyecto.FECHAINICIO != null) && (modelo.modeloProyecto.FECHAFINAL != null)/* && ((modelo.modeloProyecto.DURACION == null) || (modelo.modeloProyecto.DURACION == 0))*/)
                {
                    bool b = false;
                    var dias = "";
                    int totalDias = 0;
                    var fechaInicial = modelo.modeloProyecto.FECHAINICIO;
                    var fechaFinal = modelo.modeloProyecto.FECHAFINAL;
                    var duracion = fechaFinal - fechaInicial;
                    var d = duracion.ToString();

                    for (int i = 0; ((i < d.Length) && (b == false)); i++)
                    {
                        if (d[i].Equals('.'))
                        {
                            b = true;
                            for (int j = 0; j < i; j++)
                            {
                                dias += d[j];
                            }
                            totalDias = Int32.Parse(dias.ToString());
                        }
                    }
                    int meses = totalDias / 30;
                    modelo.modeloProyecto.DURACION = meses;
                    totalDias = 67890;
                }

                //foreach (var i in modelo.modeloProyecto.USUARIOS) {
                //    if (i.LIDER == true) {
                //        i.LIDER = false;
                //    }
                //    if (i.CEDULA != modelo.modeloUsuario.PRYCTOID) {
                //        i.LIDER = true;
                //    }
                //}

                db.Entry(modelo.modeloProyecto).State = EntityState.Modified;
                db.SaveChanges();
                Response.Write("<Script>alert('Proyecto modificado exitosamente')</Script>");
                return RedirectToAction("Index");
            }
            catch (RetryLimitExceededException)
            {                
                Response.Write("<Script>alert('ERROR - No es posible modificar este proyecto')</Script>");
                //return this.Unificado(modelo.modeloProyecto.ID);
                //ModelState.AddModelError("", "No fue posible modificar los datos del proyecto.");
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
