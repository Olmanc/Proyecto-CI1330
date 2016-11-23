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
    public class CambiosController : Controller
    {
        private BD_IngeGrupo2Entities2 db = new BD_IngeGrupo2Entities2();
        
        public ActionResult Requerimientos(string pro)
        {
            ViewBag.pro = new SelectList(db.PROYECTO, "ID", "NOMBRE");
            if (pro == "") pro = null;
            ViewBag.pid = pro;
            var rEQUERIMIENTOS = db.REQUERIMIENTOS.Where(s => s.PRYCTOID.Contains(pro) && s.ESTADO_CAMBIOS.Contains("Aprobado"));
            List<string> verificaPermisos = new List<string>();

            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual
            var listauser = db.AspNetUsers.ToArray();
            for (int i = 0; i < listauser.Length; i++)
            {  //de todos los AspNetUser del sistema, encuentra el que tenga el email activo actualmente
                if (listauser[i].Email == User.Identity.Name)
                {
                    fg = listauser[i];                  //obtiene el AspNetUser actual
                }
            }

            AspNetRoles role = fg.AspNetRoles.First();  //consigue el rol del usuario
            var per = role.PERMISOS;                    //copia los permisos que tiene asignado

            foreach (PERMISOS p in role.PERMISOS)
            {     //los copia a un HashSet<string>
                verificaPermisos.Add(p.ID); //Metodo que verifica el permiso del usuario actual.
            }

            ViewBag.Permisos = verificaPermisos;

            return View(rEQUERIMIENTOS.ToList());
        }


        public ActionResult Versiones(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var rEQUERIMIENTOS = db.REQUERIMIENTOS.Where(s => s.ID.Contains(id));
            if (rEQUERIMIENTOS == null)
            {
                return HttpNotFound();
            }
            return View(rEQUERIMIENTOS.ToList());
        }
     


        // GET: Cambios/Details/5
        /*public ActionResult ConsultarVers(string id, int version)
        {
            //var Intermedio = new ModeloIntermedioCambios();
            var Intermedio = new ModeloIntermedioCambios();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Intermedio.consultado = db.REQUERIMIENTOS.Find(id, version);
            Intermedio.actual = db.REQUERIMIENTOS.Where(s => s.ID.Contains(id) && s.ESTADO_CAMBIOS.Contains("Aprobado")).First();
            if (Intermedio.consultado == null)
            {
                return HttpNotFound();
            }
            return View(Intermedio);
        }*/

        // GET: Cambios/Create
        public ActionResult Create(string id, int version)
        {
            var cambio = new CAMBIOS();
            var correoUsuario = db.CORREOS.Where(s => s.CORREO.Contains(User.Identity.Name)).Single();
            USUARIOS actual = correoUsuario.USUARIOS;
            int v = db.REQUERIMIENTOS.Where(s => s.ID.Contains(id)).Select(s => s.VERSION_ID).Max() + 1;
            REQUERIMIENTOS rEQUERIMIENTOS = db.REQUERIMIENTOS.Find(id, version);
            rEQUERIMIENTOS.VERSION_ID = v;
            rEQUERIMIENTOS.ESTADO_CAMBIOS = "Pendiente";
            cambio.USUARIOS = actual;
            cambio.CEDULA = actual.CEDULA;
            cambio.FECHA = DateTime.Now;
            cambio.REQUERIMIENTOS1 = rEQUERIMIENTOS;
            cambio.VIEJO_REQ_ID = id;
            cambio.VIEJO_VER_ID = version;
            cambio.NUEVO_REQ_ID = id;
            cambio.NUEVO_VER_ID = v;
            ViewBag.PRYCTOID = new SelectList(db.PROYECTO, "ID", "NOMBRE", rEQUERIMIENTOS.PRYCTOID);
            ViewBag.ENCARGADO = new SelectList(db.USUARIOS.Where(s => s.PRYCTOID.Contains(rEQUERIMIENTOS.PRYCTOID)), "CEDULA", "NOMBRE", rEQUERIMIENTOS.ENCARGADO);
            return View(cambio);
        }

        // POST: Cambios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(/*[Bind(Include = "ID,CEDULA,FECHA,DESCRIPCION,JUSTIFICACION,VIEJO_REQ_ID,VIEJO_VER_ID,NUEVO_REQ_ID,NUEVO_VER_ID,JUST_REV,FECHA_REV,CED_REV,REQUERIMIENTOS1")] */CAMBIOS cAMBIOS)
        {
            if (ModelState.IsValid)
            {
                cAMBIOS.ID = Guid.NewGuid().ToString().Substring(0, 7);
                db.REQUERIMIENTOS.Add(cAMBIOS.REQUERIMIENTOS1);
                db.CAMBIOS.Add(cAMBIOS);
                db.SaveChanges();
            }
            return RedirectToAction("Requerimientos");
        }

        // GET: Cambios/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAMBIOS cAMBIOS = db.CAMBIOS.Find(id);
            if (cAMBIOS == null)
            {
                return HttpNotFound();
            }
            ViewBag.VIEJO_REQ_ID = new SelectList(db.REQUERIMIENTOS, "ID", "NOMBRE", cAMBIOS.VIEJO_REQ_ID);
            ViewBag.CED_REV = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE", cAMBIOS.CED_REV);
            ViewBag.CEDULA = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE", cAMBIOS.CEDULA);
            ViewBag.NUEVO_REQ_ID = new SelectList(db.REQUERIMIENTOS, "ID", "NOMBRE", cAMBIOS.NUEVO_REQ_ID);
            return View(cAMBIOS);
        }

        // POST: Cambios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CEDULA,FECHA,DESCRIPCION,JUSTIFICACION,VIEJO_REQ_ID,VIEJO_VER_ID,NUEVO_REQ_ID,NUEVO_VER_ID,JUST_REV,FECHA_REV,CED_REV")] CAMBIOS cAMBIOS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cAMBIOS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.VIEJO_REQ_ID = new SelectList(db.REQUERIMIENTOS, "ID", "NOMBRE", cAMBIOS.VIEJO_REQ_ID);
            ViewBag.CED_REV = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE", cAMBIOS.CED_REV);
            ViewBag.CEDULA = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE", cAMBIOS.CEDULA);
            ViewBag.NUEVO_REQ_ID = new SelectList(db.REQUERIMIENTOS, "ID", "NOMBRE", cAMBIOS.NUEVO_REQ_ID);
            return View(cAMBIOS);
        }

        // GET: Cambios/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAMBIOS cAMBIOS = db.CAMBIOS.Find(id);
            if (cAMBIOS == null)
            {
                return HttpNotFound();
            }
            return View(cAMBIOS);
        }

        // POST: Cambios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            CAMBIOS cAMBIOS = db.CAMBIOS.Find(id);
            db.CAMBIOS.Remove(cAMBIOS);
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

        /*/////////////////////////////////////////////////////////////////////////////////////////////////////////
         INDEX de las SOLICITUDES*/
        /* EFECTO: muestra listado de solicitudes
         * REQUIERE: N/A
         * MODIFICA: N/A  */
        public ActionResult IndexSolicitudes()
        {
            CambiosViewModel modelo = new CambiosViewModel();
            modelo.listaCambios = db.CAMBIOS.ToList();
            //var cAMBIOS = db.CAMBIOS.Include(c => c.REQUERIMIENTOS).Include(c => c.USUARIOS).Include(c => c.USUARIOS1).Include(c => c.REQUERIMIENTOS1);
            return View(modelo);
        }

        /*////////////////////////////////////////////////////////////////////////////////////////////////////////////
         Detalles de una solicitud*/
        /* EFECTO: muestra todos los datos de una solicitud
         * REQUIERE: id de la solicitud a consultar
         * MODIFICA: N/A  */
        public ActionResult DetallesSolicitud(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CambiosViewModel modelo = new CambiosViewModel();
            modelo.solicitud = db.CAMBIOS.Find(id);
            if (modelo.solicitud == null)
            {
                return HttpNotFound();
            }
            modelo.propuesto = db.REQUERIMIENTOS.Find(modelo.solicitud.NUEVO_REQ_ID, modelo.solicitud.NUEVO_VER_ID);
            modelo.vigente = db.REQUERIMIENTOS.Find(modelo.solicitud.VIEJO_REQ_ID, modelo.solicitud.VIEJO_VER_ID);
            modelo.solicitante = db.USUARIOS.Find(modelo.solicitud.CEDULA);
            return View(modelo);
        }
    }
}

