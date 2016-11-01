﻿using System;
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
    public class RequerimientosController : Controller
    {
        private BD_IngeGrupo2Entities2 db = new BD_IngeGrupo2Entities2();

        // GET: Requerimientos
        public ActionResult Index(string pro)
        {
            ViewBag.pro = new SelectList(db.PROYECTO, "ID", "NOMBRE");
            var rEQUERIMIENTOS = db.REQUERIMIENTOS.Include(r => r.PROYECTO).Include(r => r.USUARIOS);
            rEQUERIMIENTOS = rEQUERIMIENTOS.Where(s => s.PRYCTOID.Contains(pro));
            return View(rEQUERIMIENTOS.ToList());
        }

        // GET: Requerimientos/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REQUERIMIENTOS rEQUERIMIENTOS = db.REQUERIMIENTOS.Find(id);
            if (rEQUERIMIENTOS == null)
            {
                return HttpNotFound();
            }
            return View(rEQUERIMIENTOS);
        }

        // GET: Requerimientos/Create
        public ActionResult Create()
        {
            ViewBag.PRYCTOID = new SelectList(db.PROYECTO, "ID", "NOMBRE");
            ViewBag.ENCARGADO = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE");
            var reque = new REQUERIMIENTOS();
            reque.crearCriterios(0);
            return View(reque);
        }

        // POST: Requerimientos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NOMBRE,ESFUERZO,IMAGEN,DESCRIPCION,PRIORIDAD,OBSERVACIONES,MODULO,FECHAINCIO,FECHAFINAL,ESTADO,ENCARGADO,PRYCTOID,VERSION_ID,CRIT_ACEPTACION")] REQUERIMIENTOS rEQUERIMIENTOS)
        {
            if (ModelState.IsValid)
            {
                foreach (CRIT_ACEPTACION criterio in rEQUERIMIENTOS.CRIT_ACEPTACION.ToList())
                {
                    if (criterio.DEL == true)
                    {
                        // Delete Phone Numbers which is marked to remov
                        rEQUERIMIENTOS.CRIT_ACEPTACION.Remove(criterio);
                    }else
                    {
                        criterio.ID = Guid.NewGuid().ToString().Substring(0, 7);
                    }
                }
                db.REQUERIMIENTOS.Add(rEQUERIMIENTOS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PRYCTOID = new SelectList(db.PROYECTO, "ID", "NOMBRE", rEQUERIMIENTOS.PRYCTOID);
            ViewBag.ENCARGADO = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE", rEQUERIMIENTOS.ENCARGADO);
            return View(rEQUERIMIENTOS);
        }

        // GET: Requerimientos/Edit/5
        public ActionResult Edit(string id, int version)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REQUERIMIENTOS rEQUERIMIENTOS = db.REQUERIMIENTOS.Find(id, version);
            if (rEQUERIMIENTOS == null)
            {
                return HttpNotFound();
            }
            ViewBag.PRYCTOID = new SelectList(db.PROYECTO, "ID", "NOMBRE", rEQUERIMIENTOS.PRYCTOID);
            ViewBag.ENCARGADO = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE", rEQUERIMIENTOS.ENCARGADO);
            return View(rEQUERIMIENTOS);
        }

        // POST: Requerimientos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NOMBRE,ESFUERZO,IMAGEN,DESCRIPCION,PRIORIDAD,OBSERVACIONES,MODULO,FECHAINCIO,FECHAFINAL,ESTADO,ENCARGADO,PRYCTOID,VERSION_ID,CRIT_ACEPTACION")] REQUERIMIENTOS rEQUERIMIENTOS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rEQUERIMIENTOS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PRYCTOID = new SelectList(db.PROYECTO, "ID", "NOMBRE", rEQUERIMIENTOS.PRYCTOID);
            ViewBag.ENCARGADO = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE", rEQUERIMIENTOS.ENCARGADO);
            return View(rEQUERIMIENTOS);
        }

        // GET: Requerimientos/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REQUERIMIENTOS rEQUERIMIENTOS = db.REQUERIMIENTOS.Find(id);
            if (rEQUERIMIENTOS == null)
            {
                return HttpNotFound();
            }
            return View(rEQUERIMIENTOS);
        }

        // POST: Requerimientos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, int version)
        {
            REQUERIMIENTOS rEQUERIMIENTOS = db.REQUERIMIENTOS.Find(id, version);
            db.REQUERIMIENTOS.Remove(rEQUERIMIENTOS);
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

        public ActionResult Unificado2(string id, int version)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REQUERIMIENTOS rEQUERIMIENTOS = db.REQUERIMIENTOS.Find(id, version);
            if (rEQUERIMIENTOS == null)
            {
                return HttpNotFound();
            }
            ViewBag.PRYCTOID = new SelectList(db.PROYECTO, "ID", "NOMBRE", rEQUERIMIENTOS.PRYCTOID);
            ViewBag.ENCARGADO = new SelectList(db.USUARIOS, "CEDULA", "NOMBRE", rEQUERIMIENTOS.ENCARGADO);

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
                rEQUERIMIENTOS.verificaPermisos.Add(p.ID);
            }

            return View(rEQUERIMIENTOS);
        }

        public ActionResult eliminarReq(string id, int version)
        {
            REQUERIMIENTOS req = db.REQUERIMIENTOS.Find(id, version);
            db.REQUERIMIENTOS.Remove(req);
            db.SaveChanges();
            return Json(new { success = true });
        }

        public ActionResult cancelar(REQUERIMIENTOS REQ)
        {
            return View(REQ);
        }

        // Método post de la vista Unificada, se llama unicamente en el botón Guardar de la sección modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unificado2(REQUERIMIENTOS req)
        {
            if (ModelState.IsValid)
            {
                var criterios = db.CRIT_ACEPTACION.Where(i => i.REQUERIMIENTO_ID == req.ID && i.VERSION_ID == req.VERSION_ID);
                foreach(var c in criterios)
                {
                    db.CRIT_ACEPTACION.Remove(c);
                }
                foreach (CRIT_ACEPTACION criterio in req.CRIT_ACEPTACION.ToList())
                {
                    if (criterio.DEL == true)
                    {
                        // Delete Phone Numbers which is marked to remov
                        req.CRIT_ACEPTACION.Remove(criterio);
                    }
                    else
                    {
                        criterio.ID = Guid.NewGuid().ToString().Substring(0, 7);
                    }
                }
                db.REQUERIMIENTOS.Add(req);
                db.Entry(req).State = EntityState.Modified;
                var id = req.ID;
                var vers = req.VERSION_ID;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(req);
        }
    }
}
