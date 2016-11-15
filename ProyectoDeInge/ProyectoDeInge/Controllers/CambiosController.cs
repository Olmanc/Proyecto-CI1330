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
            var cAMBIOS = db.CAMBIOS.Include(c => c.REQUERIMIENTOS).Include(c => c.USUARIOS).Include(c => c.USUARIOS1).Include(c => c.REQUERIMIENTOS1);
            return View(cAMBIOS.ToList());
        }

        // GET: Cambios/Details/5
        public ActionResult Details(string id)
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
        public ActionResult Create()
        {
            ViewBag.VIEJO_REQ_ID = new SelectList(db.REQUERIMIENTOS, "ID", "NOMBRE");
            ViewBag.CED_REV = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE");
            ViewBag.CEDULA = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE");
            ViewBag.NUEVO_REQ_ID = new SelectList(db.REQUERIMIENTOS, "ID", "NOMBRE");
            return View();
        }

        // POST: Cambios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CEDULA,FECHA,DESCRIPCION,JUSTIFICACION,VIEJO_REQ_ID,VIEJO_VER_ID,NUEVO_REQ_ID,NUEVO_VER_ID,JUST_REV,FECHA_REV,CED_REV")] CAMBIOS cAMBIOS)
        {
            if (ModelState.IsValid)
            {
                db.CAMBIOS.Add(cAMBIOS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.VIEJO_REQ_ID = new SelectList(db.REQUERIMIENTOS, "ID", "NOMBRE", cAMBIOS.VIEJO_REQ_ID);
            ViewBag.CED_REV = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE", cAMBIOS.CED_REV);
            ViewBag.CEDULA = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE", cAMBIOS.CEDULA);
            ViewBag.NUEVO_REQ_ID = new SelectList(db.REQUERIMIENTOS, "ID", "NOMBRE", cAMBIOS.NUEVO_REQ_ID);
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
    }
}
