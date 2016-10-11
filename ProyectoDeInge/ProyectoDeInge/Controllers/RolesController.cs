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
    public class RolesController : Controller
    {
        private BD_IngeGrupo2Entities2 db = new BD_IngeGrupo2Entities2();

        // GET: Roles
        public ActionResult Index()
        {
            return View(db.AspNetRoles.ToList());
        }

        // GET: Roles/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetRoles aspNetRoles = db.AspNetRoles.Find(id);
            if (aspNetRoles == null)
            {
                return HttpNotFound();
            }
            return View(aspNetRoles);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] AspNetRoles aspNetRoles)
        {
            if (ModelState.IsValid)
            {
                db.AspNetRoles.Add(aspNetRoles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetRoles);
        }

        // GET: Roles/Edit/5
        public ActionResult Edit(/*String id*/)
        {
            var modelo = new permiso2Viewmodel();
            modelo.Admin = db.AspNetRoles.Find("1");
            modelo.Desa = db.AspNetRoles.Find("2");
            modelo.Usuario = db.AspNetRoles.Find("3");
            modelo.Roles = db.AspNetRoles.ToList();
            modelo.Permisos = db.PERMISOS.ToList();
            /*
            var permisosViewModel = new permisosViewModel();            
            permisosViewModel.todoPermisos = db.Permisos.ToList();
            permisosViewModel.todoRoles = db.AspNetRoles.ToList();
            */

            return View(modelo);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection resultado)
        {
            if (resultado.Count < 4)
            {
                Response.Write("<Script>alert('ERROR - No pueden haber roles sin permisos.')</Script>");
                return this.Edit();
                /*                return RedirectToAction("Edit");                   return new EmptyResult();                return View(permisos);                return RedirectToAction("Edit");
                */
            }
            permisosViewModel Administrador = new permisosViewModel();
            permisosViewModel Desarrollador = new permisosViewModel();
            permisosViewModel Usuario = new permisosViewModel();
            List<string> permisosAdmin = new List<string>();
            List<string> permisosDesarrollador = new List<string>();
            List<string> permisosUsuario = new List<string>();

            var listaAd = resultado["Administrador"];
            var listaDe = resultado["Desarrollador"];
            var listaUs = resultado["Usuario"];
            var lista2 = new string[4];

            if (listaAd == null || listaDe == null || listaUs == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var lista = listaAd.Split(',');
            if (lista.Contains("3") || lista.Contains("4"))
            {
                if (!lista.Contains("2"))
                {
                    Response.Write("<Script>alert('ERROR - No es posible asignar permiso de Modificar/Eliminar sin permiso para Consultar.')</Script>");
                    return this.Edit();
                }
            }
            for (int i = 0; i < lista.Length; i++)
            {
                //lista2[i] = Convert.ToInt32(lista[i]);
                permisosAdmin.Add(lista[i]);
            }

            lista = listaDe.Split(',');
            if (lista.Contains("3") || lista.Contains("4"))
            {
                if (!lista.Contains("2"))
                {
                    Response.Write("<Script>alert('ERROR - No es posible asignar permiso de Modificar/Eliminar sin permiso para Consultar.')</Script>");
                    return this.Edit();
                }
            }
            for (int i = 0; i < lista.Length; i++)
            {
                //lista2[i] = Convert.ToInt32(lista[i]);
                permisosDesarrollador.Add(lista[i]);
            }

            lista = listaUs.Split(',');
            if (lista.Contains("3") || lista.Contains("4"))
            {
                if (!lista.Contains("2"))
                {
                    Response.Write("<Script>alert('ERROR - No es posible asignar permiso de Modificar/Eliminar sin permiso para Consultar.')</Script>");
                    return this.Edit();
                }
            }
            for (int i = 0; i < lista.Length; i++)
            {
                //lista2[i] = Convert.ToInt32(lista[i]);
                permisosUsuario.Add(lista[i]);
            }

            if (Administrador == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Administrador.permisosAsignados = permisosAdmin;
            if (Desarrollador == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Desarrollador.permisosAsignados = permisosDesarrollador;
            if (Usuario == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario.permisosAsignados = permisosUsuario;

            try
            {
                if (ModelState.IsValid)
                {
                    var adminActualiza = db.AspNetRoles
                    .Include(i => i.PERMISOS).First(i => i.Id == "1");

                    if (TryUpdateModel(adminActualiza, "AspNetRoles", new string[] { "Name" }))
                    {
                        var adminNuevos = db.PERMISOS.Where(
                            m => Administrador.permisosAsignados.Contains(m.ID)).ToList();
                        var adminActualizado = new HashSet<string>(Administrador.permisosAsignados);

                        foreach (PERMISOS permiso in db.PERMISOS)
                        {
                            if (!adminActualizado.Contains(permiso.ID))
                            {
                                adminActualiza.PERMISOS.Remove(permiso);
                            }
                            else
                            {
                                adminActualiza.PERMISOS.Add(permiso);
                            }
                        }
                        db.Entry(adminActualiza).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                    var desaActualiza = db.AspNetRoles
                    .Include(i => i.PERMISOS).First(i => i.Id == "2");

                    if (TryUpdateModel(desaActualiza, "AspNetRoles", new string[] { "Name" }))
                    {
                        var desaNuevos = db.PERMISOS.Where(
                            m => Desarrollador.permisosAsignados.Contains(m.ID)).ToList();
                        var desaActualizado = new HashSet<string>(Desarrollador.permisosAsignados);

                        foreach (PERMISOS permiso in db.PERMISOS)
                        {
                            if (!desaActualizado.Contains(permiso.ID))
                            {
                                desaActualiza.PERMISOS.Remove(permiso);
                            }
                            else
                            {
                                desaActualiza.PERMISOS.Add(permiso);
                            }
                        }
                        db.Entry(desaActualiza).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                    var usuActualiza = db.AspNetRoles
                    .Include(i => i.PERMISOS).First(i => i.Id == "3");

                    if (TryUpdateModel(usuActualiza, "AspNetRoles", new string[] { "Name" }))
                    {
                        var usuNuevos = db.PERMISOS.Where(
                            m => Usuario.permisosAsignados.Contains(m.ID)).ToList();
                        var usuActualizado = new HashSet<string>(Usuario.permisosAsignados);

                        foreach (PERMISOS permiso in db.PERMISOS)
                        {
                            if (!usuActualizado.Contains(permiso.ID))
                            {
                                usuActualiza.PERMISOS.Remove(permiso);
                            }
                            else
                            {
                                usuActualiza.PERMISOS.Add(permiso);
                            }
                        }
                        db.Entry(usuActualiza).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    TempData["Exito"] = "Permisos asignados exitosamente!";
                    //Response.Write("<Script>alert('Permisos actualizados exitosamente')</Script>");
                    return RedirectToAction("Index");
                }
            }
            catch
            {

                return this.Edit();
            }


            return RedirectToAction("Index");
        }
    }
    /*
        // GET: Roles/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetRoles aspNetRoles = db.AspNetRoles.Find(id);
            if (aspNetRoles == null)
            {
                return HttpNotFound();
            }
            return View(aspNetRoles);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetRoles aspNetRoles = db.AspNetRoles.Find(id);
            db.AspNetRoles.Remove(aspNetRoles);
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
    }*/
}
