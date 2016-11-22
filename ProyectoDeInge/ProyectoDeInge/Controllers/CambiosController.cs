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

        // GET: Cambios
        public ActionResult Index()
        {
            CambiosViewModel modelo = new CambiosViewModel();
            modelo.listaCambios = db.CAMBIOS.ToList();
            var cAMBIOS = db.CAMBIOS.Include(c => c.REQUERIMIENTOS).Include(c => c.USUARIOS).Include(c => c.USUARIOS1).Include(c => c.REQUERIMIENTOS1);
            return View(/*cAMBIOS.ToList()*/modelo);
        }

        public ActionResult Versiones()
        {
            var cAMBIOS = db.CAMBIOS.Include(c => c.REQUERIMIENTOS).Include(c => c.USUARIOS).Include(c => c.USUARIOS1).Include(c => c.REQUERIMIENTOS1);
            return View(cAMBIOS.ToList());
        }


        // GET: Cambios/Details/5
        public ActionResult ConsultarVers(string id)
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

        // GET: Cambios/Create
        public ActionResult Create(string id, int version)
        {
            var cambio = new CAMBIOS();
            int v = db.REQUERIMIENTOS.Where(s => s.ID.Contains(id)).Select(s => s.VERSION_ID).Max() + 1;
            REQUERIMIENTOS rEQUERIMIENTOS = db.REQUERIMIENTOS.Find(id, version);
            rEQUERIMIENTOS.VERSION_ID = v;
            rEQUERIMIENTOS.ESTADO_CAMBIOS = "Pendiente";
            cambio.REQUERIMIENTOS1 = rEQUERIMIENTOS;
            cambio.VIEJO_REQ_ID = id;
            cambio.VIEJO_VER_ID = version;
            cambio.NUEVO_REQ_ID = id;
            cambio.NUEVO_VER_ID = v;
            ViewBag.VIEJO_REQ_ID = new SelectList(db.REQUERIMIENTOS, "ID", "NOMBRE");
            ViewBag.CED_REV = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE");
            ViewBag.CEDULA = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE");
            ViewBag.NUEVO_REQ_ID = new SelectList(db.REQUERIMIENTOS, "ID", "NOMBRE");
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
                db.CAMBIOS.Add(cAMBIOS);
                db.REQUERIMIENTOS.Add(cAMBIOS.REQUERIMIENTOS1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cAMBIOS);
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
        public ActionResult IndexSolicitudes()
        {
            CambiosViewModel modelo = new CambiosViewModel();
            modelo.listaCambios = db.CAMBIOS.ToList();
            //var cAMBIOS = db.CAMBIOS.Include(c => c.REQUERIMIENTOS).Include(c => c.USUARIOS).Include(c => c.USUARIOS1).Include(c => c.REQUERIMIENTOS1);
            return View(/*cAMBIOS.ToList()*/modelo);
        }

        /*////////////////////////////////////////////////////////////////////////////////////////////////////////////
         Detalles de una solicitud*/
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
            modelo.propuesto = db.REQUERIMIENTOS.Find(modelo.solicitud.NUEVO_REQ_ID,modelo.solicitud.NUEVO_VER_ID);
            modelo.vigente = db.REQUERIMIENTOS.Find(modelo.solicitud.VIEJO_REQ_ID,modelo.solicitud.VIEJO_VER_ID);
            modelo.solicitante = db.USUARIOS.Find(modelo.solicitud.CEDULA);
            return View(modelo);
        }
    }
}
