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
using System.IO;
using System.Web.Hosting;
using System.Globalization;

namespace ProyectoDeInge.Controllers
{
    public class UsuariosController : Controller
    {
        private BD_IngeGrupo2Entities2 db = new BD_IngeGrupo2Entities2();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        // GET: Usuarios
        public ActionResult Index()
        {
            ModeloIntermedio modelo = new ModeloIntermedio();
            modelo.listaUsuarios = db.USUARIOS.ToList();
            modelo.listaTelefonos = db.TELEFONOS.ToList();
            modelo.listaCorreos = db.CORREOS.ToList();
            modelo.listaProyecto = db.PROYECTO.ToList();

            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual
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
                modelo.verificaPermisos.Add(p.ID);
            }
            
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
            var modelo = new ModeloIntermedio();
            return View(modelo);
        }


        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ModeloIntermedio modelo)
        {
            if (db.USUARIOS.Find(modelo.modeloUsuario.CEDULA) != null) {
                TempData["Create"] = "Existe";
                return RedirectToAction("Create");
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = modelo.modeloCorreo.CORREO, Email = modelo.modeloCorreo.CORREO };
                user.EmailConfirmed = true;
                user.PhoneNumberConfirmed = true;
                var result = await UserManager.CreateAsync(user, "Pass.123");

                if (result.Succeeded)
                {
                    var usuarioNuevo = modelo.modeloUsuario;

                    var aspUser = new AspNetUsers();
                    var listauser = db.AspNetUsers.ToArray();

                    for (int i = 0; i < listauser.Length; i++)
                    {
                        if (listauser[i].Email == modelo.modeloCorreo.CORREO)
                        {
                            aspUser = listauser[i];
                        }
                    }
                    var role = db.AspNetRoles.Where(r => r.Id == modelo.rol).First();
                    aspUser.AspNetRoles.Add(role);
                    var id = aspUser.Id;
                    usuarioNuevo.ID_ASP = id;
                    db.USUARIOS.Add(usuarioNuevo);

                    //correo1 nunca va a ser nulo
                    CORREOS cor1 = modelo.modeloCorreo;
                    cor1.CEDULA = usuarioNuevo.CEDULA;
                    db.CORREOS.Add(cor1);

                    if (modelo.modeloCorreo2.CORREO != null)
                    {
                        CORREOS cor2 = modelo.modeloCorreo2;
                        cor2.CEDULA = usuarioNuevo.CEDULA;
                        db.CORREOS.Add(cor2);
                    }
                    if (modelo.modeloTelefono.NUMERO != null)
                    {
                        TELEFONOS tel1 = modelo.modeloTelefono;
                        tel1.CEDULA = usuarioNuevo.CEDULA;
                        db.TELEFONOS.Add(tel1);
                    }
                    if (modelo.modeloTelefono2.NUMERO != null)
                    {
                        TELEFONOS tel2 = modelo.modeloTelefono2;
                        tel2.CEDULA = usuarioNuevo.CEDULA;
                        db.TELEFONOS.Add(tel2);
                    }
                    db.SaveChanges();

                    SendEmailViewModel email = new SendEmailViewModel();
                    email.FirstName = modelo.modeloUsuario.NOMBRE;
                    email.Email = modelo.modeloCorreo.CORREO;
                    var message = await EMailTemplate("WelcomeEmail");
                    message = message.Replace("@ViewBag.Name", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(email.FirstName));
                    await MessageServices.SendEmailAsync(email.Email, "Bienvenido", message);
                }
                else
                {
                    Response.Write("<Script>alert('ERROR - no creo AspNetUser.')</Script>");
                }                
            }

            return RedirectToAction("Index");
            //ViewBag.PRYCTOID = new SelectList(db.PROYECTO, "ID", "NOMBRE", modelo.modeloUsuario.PRYCTOID);
            //return View(modelo);
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
            
            
            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual
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
                modelo.verificaPermisos.Add(p.ID);
            }
            
            
            
            return View(modelo);
        }

        //Método para Javascript para eliminar una persona, se llama en el jquery

        public ActionResult eliminarPersona(string id)
        {
            var correos = db.CORREOS.Where(c => c.CEDULA == id);
            foreach( var c in correos){
                db.CORREOS.Remove(c);
            }
            var telefonos = db.TELEFONOS.Where(t => t.CEDULA == id);
            foreach (var t in telefonos) {
                db.TELEFONOS.Remove(t);
            }
            USUARIOS persona = db.USUARIOS.Find(id);
            string aspId = persona.ID_ASP;
            db.USUARIOS.Remove(persona);
            var user = db.AspNetUsers.Find(aspId);
            db.AspNetUsers.Remove(user);
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

        public static async Task<string> EMailTemplate(string template)
        {
            var templateFilePath = HostingEnvironment.MapPath("~/Content/templates/") + template + ".cshtml";
            StreamReader objstreamreaderfile = new StreamReader(templateFilePath);
            var body = await objstreamreaderfile.ReadToEndAsync();
            objstreamreaderfile.Close();
            return body;
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

     
    
         
