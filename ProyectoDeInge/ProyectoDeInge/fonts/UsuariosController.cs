using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ProyectoDeInge.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Web.Security;

namespace ProyectoDeInge.Controllers
{
    public class UsuariosController : Controller
    {
        private BD_IngeGrupo2Entities2 db = new BD_IngeGrupo2Entities2();

        // GET: Usuarios
        public ActionResult Index()
        {
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.listaUsuarios = db.USUARIOS.ToList();
            modelo.listaTelefonos = db.TELEFONOS.ToList();
            modelo.listaCorreos = db.CORREOS.ToList();
            modelo.listaProyecto = db.PROYECTO.ToList();
            return View(modelo);
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(string id)
        {
            ModeloIntermedio modelo = new ModeloIntermedio();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USUARIOS user = db.USUARIOS.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            TELEFONOS tel = new TELEFONOS();
            List<TELEFONOS> listaTelefs = db.TELEFONOS.ToList();
            foreach (var item in listaTelefs)
            {
                if ((user.CEDULA == id) && (user.CEDULA == item.CEDULA))
                {
                    tel.NUMERO = item.NUMERO;
                    tel.CEDULA = id;
                }
            }

            CORREOS email = new CORREOS();
            List<CORREOS> listaEmails = db.CORREOS.ToList();
            foreach (var item in listaEmails)
            {
                if ((user.CEDULA == id) && (user.CEDULA == item.CEDULA))
                {
                    email.CORREO = item.CORREO;
                    email.CEDULA = id;
                }
            }



            /*ApplicationUser usuario = db..Where(u => u.CEDULA.Equals(id, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var account = new AccountController();

            ViewBag.RolesForThisUser = account.UserManager.GetRoles(usuario.Id);*/
            //String.Join(",", Roles.GetRolesForUser());



            modelo.modeloUsuario = user;
            modelo.modeloTelefono = tel;
            modelo.modeloCorreo = email;
            return View(modelo);
        }
    

    // GET: Usuarios/Create
    public ActionResult Create()
        {
            return View();
        }


        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(/*[Bind(Include = "CEDULA,NOMBRE,APELLIDO1,APELLIDO2,PRYCTOID,LIDER,TELEFONO,TELEFONO2,CORREO,CORREOS2")]*/ ModeloIntermedio modelo)
        {
            if (ModelState.IsValid)
            {
                db.USUARIOS.Add(modelo.modeloUsuario);
                db.SaveChanges();
                if(modelo.modeloCorreo != null)
                {
                    modelo.modeloCorreo.CEDULA = modelo.modeloUsuario.CEDULA;
                    db.CORREOS.Add(modelo.modeloCorreo);
                }

                if (modelo.modeloCorreo2 != null)
                {
                    modelo.modeloCorreo2.CEDULA = modelo.modeloUsuario.CEDULA;
                    db.CORREOS.Add(modelo.modeloCorreo2);
                }

                if (modelo.modeloTelefono != null)
                {
                    modelo.modeloTelefono.CEDULA = modelo.modeloUsuario.CEDULA;
                    db.TELEFONOS.Add(modelo.modeloTelefono);
                }

                if (modelo.modeloTelefono2 != null)
                {
                    modelo.modeloTelefono2.CEDULA = modelo.modeloUsuario.CEDULA;
                    db.TELEFONOS.Add(modelo.modeloTelefono2);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PRYCTOID = new SelectList(db.PROYECTO, "ID", "NOMBRE", modelo.modeloUsuario.PRYCTOID);
            return View(modelo);
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USUARIOS uSUARIOS = db.USUARIOS.Find(id);
            if (uSUARIOS == null)
            {
                return HttpNotFound();
            }
            ViewBag.PRYCTOID = new SelectList(db.PROYECTO, "ID", "NOMBRE", uSUARIOS.PRYCTOID);
            return View(uSUARIOS);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CEDULA,NOMBRE,PRYCTOID,LIDER")] USUARIOS uSUARIOS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uSUARIOS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PRYCTOID = new SelectList(db.PROYECTO, "ID", "NOMBRE", uSUARIOS.PRYCTOID);
            return View(uSUARIOS);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            USUARIOS uSUARIOS = db.USUARIOS.Find(id);
            if (uSUARIOS == null)
            {
                return HttpNotFound();
            }
            return View(uSUARIOS);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            USUARIOS uSUARIOS = db.USUARIOS.Find(id);
            db.USUARIOS.Remove(uSUARIOS);
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
    }
}
