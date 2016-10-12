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
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Mail;

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

            TELEFONOS tel2 = new TELEFONOS();
            foreach (var item in listaTelefs)
            {
                if ((user.CEDULA == id) && (user.CEDULA == item.CEDULA) && (item.NUMERO != tel.NUMERO))
                {
                    tel2.NUMERO = item.NUMERO;
                    tel2.CEDULA = id;
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

            CORREOS email2 = new CORREOS();
            foreach (var item in listaEmails)
            {
                if ((user.CEDULA == id) && (user.CEDULA == item.CEDULA) && (item.CORREO != email.CORREO))
                {
                    email2.CORREO = item.CORREO;
                    email2.CEDULA = id;
                }
            }

            //ViewBag.Rol = GetRolesForUser(id);

            modelo.modeloUsuario = user;
            modelo.modeloTelefono = tel;
            modelo.modeloTelefono2 = tel2;
            modelo.modeloCorreo = email;
            modelo.modeloCorreo2 = email2;
            return View(modelo);
        }

        /*public async Task<ActionResult> GetRolesForUser(string userId)
        {
            using (
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
            {
                var rolesForUser = await userManager.GetRolesAsync(userId);

                return this.View(rolesForUser);
            }
        }*/

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
        public async Task<ActionResult> Create(/*[Bind(Include = "CEDULA,NOMBRE,APELLIDO1,APELLIDO2,PRYCTOID,LIDER,TELEFONO,TELEFONO2,CORREO,CORREOS2")]*/ ModeloIntermedio modelo)
        {
            if (ModelState.IsValid)
            {
                var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                //var RoleManager = Request.GetOwinContext().Get<ApplicationRoleManager>();
                //var ident = modelo.modeloUsuario.CEDULA;
                var Password = "JHGK-1234";
                var User = new ApplicationUser { UserName = modelo.modeloCorreo.CORREO, Email = modelo.modeloCorreo.CORREO };
                User.EmailConfirmed = true;
                User.PhoneNumberConfirmed = true;
                var result = UserManager.CreateAsync(User, Password);
                modelo.modeloUsuario.ID_ASP = User.Id;
                db.USUARIOS.Add(modelo.modeloUsuario);
                db.SaveChanges();
                if (modelo.modeloCorreo != null)
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
                string text = string.Format("Your Password is: " + Password);
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                msg.From = new MailAddress("voncita20@outlook.com");
                msg.To.Add(new MailAddress(modelo.modeloCorreo.CORREO));
                msg.Subject = "Your Password";
                msg.Body = text;
                msg.IsBodyHtml = true;

                // msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain)); 
                // msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html)); 

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "voncita20@outlook.com",
                        Password = "Javi-200495"
                    };

                    smtp.Credentials = credential;
                    smtp.Host = "smtp-mail.outlook.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(msg);
                    //return RedirectToAction("") 
                }
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

        // Método get para la vista unificada
        public ActionResult Unificado(string id)
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
            tel.CEDULA = id;
            List<TELEFONOS> listaTelefs = db.TELEFONOS.ToList();
            foreach (var item in listaTelefs)
            {
                if ((user.CEDULA == id) && (user.CEDULA == item.CEDULA))
                {
                    tel.NUMERO = item.NUMERO;
                }
            }

            TELEFONOS tel2 = new TELEFONOS();
            tel2.CEDULA = id;
            foreach (var item in listaTelefs)
            {
                if ((user.CEDULA == id) && (user.CEDULA == item.CEDULA) && (item.NUMERO != tel.NUMERO))
                {
                    tel2.NUMERO = item.NUMERO;
                }
            }

            CORREOS email = new CORREOS();
            email.CEDULA = id;
            List<CORREOS> listaEmails = db.CORREOS.ToList();
            foreach (var item in listaEmails)
            {
                if ((user.CEDULA == id) && (user.CEDULA == item.CEDULA))
                {
                    email.CORREO = item.CORREO;
                }
            }

            CORREOS email2 = new CORREOS();
            email2.CEDULA = id;
            foreach (var item in listaEmails)
            {
                if ((user.CEDULA == id) && (user.CEDULA == item.CEDULA) && (item.CORREO != email.CORREO))
                {
                    email2.CORREO = item.CORREO;
                }
            }




            modelo.modeloAsp = db.AspNetUsers.Find(user.ID_ASP);


            //ViewBag.Rol = GetRolesForUser(user.ID_ASP);

            modelo.modeloUsuario = user;
            modelo.modeloTelefono = tel;
            modelo.modeloTelefono2 = tel2;
            modelo.modeloCorreo = email;
            modelo.modeloCorreo2 = email2;
            return View(modelo);
        }

        //Método para Javascript para eliminar una persona, se llama en el jquery

        public ActionResult eliminarPersona(string id)
        {
            USUARIOS persona = db.USUARIOS.Find(id);
            db.USUARIOS.Remove(persona);
            db.SaveChanges();
            return Json(new { success = true });
        }

        public ActionResult cancelar(ModeloIntermedio modelo)
        {
            return View(modelo);
        }

        // Método post de la vista Unificada, se llama unicamente en el botón Guardar de la sección modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unificado(ModeloIntermedio modelo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(modelo.modeloUsuario).State = EntityState.Modified;
                var id = modelo.modeloUsuario.CEDULA;
                if (!string.IsNullOrEmpty(modelo.modeloTelefono.NUMERO) && 
                    !string.IsNullOrEmpty(modelo.modeloTelefono2.NUMERO))
                {
                    db.Database.ExecuteSqlCommand(
                   "Delete From TELEFONOS Where CEDULA = " + id);
                    modelo.modeloTelefono.CEDULA = id;
                    modelo.modeloTelefono2.CEDULA = id;
                    db.TELEFONOS.Add(modelo.modeloTelefono);
                    db.TELEFONOS.Add(modelo.modeloTelefono2);                   
                }
                if (!string.IsNullOrEmpty(modelo.modeloCorreo.CORREO) && 
                    !string.IsNullOrEmpty(modelo.modeloCorreo2.CORREO))
                {
                    db.Database.ExecuteSqlCommand(
                    "Delete From CORREOS Where CEDULA = " + id);
                    modelo.modeloCorreo.CEDULA = id;
                    modelo.modeloCorreo2.CEDULA = id;
                    db.CORREOS.Add(modelo.modeloCorreo);
                    db.CORREOS.Add(modelo.modeloCorreo2);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(modelo);
        }
        /*
        public async Task<string> GetRolesForUser(string userId)
        {
            using (
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
            {
                var rolesForUser = await userManager.GetRolesAsync(userId);

                return rolesForUser.First();
            }
        }*/

    }
}
