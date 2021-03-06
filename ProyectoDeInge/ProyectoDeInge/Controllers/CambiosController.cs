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
    public class CambiosController : Controller
    {
        private BD_IngeGrupo2Entities2 db = new BD_IngeGrupo2Entities2();
        
       
      /*  EFECTO: Despliega la lista de Requerimientos de un proyecto en la vista REQUERIMIENTOS del módulo cambios.
          REQUIERE: String pro (Proyecto)
          MODIFICA: -
        */
        public ActionResult Requerimientos(string pro)
        {
            ViewBag.pro = new SelectList(db.PROYECTO.Where(s => s.BORRADO != true), "ID", "NOMBRE");
            if (pro == "") pro = null;
            ViewBag.pid = pro;
            var rEQUERIMIENTOS = db.REQUERIMIENTOS.Where(s => s.PRYCTOID == pro && s.ESTADO_CAMBIOS == "Aprobado");
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
        
        
        
         

       /* EFECTO: Despliega la lista de Versiones de un requerimiento.
          REQUIERE: String id (Id de requerimiento)
          MODIFICA: -
        */
        public ActionResult Versiones(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var rEQUERIMIENTOS = db.REQUERIMIENTOS.Where(s => s.ID ==id);
            if (rEQUERIMIENTOS == null)
            {
                return HttpNotFound();
            }
            ViewBag.Permisos = obtienePermisos();
            return View(rEQUERIMIENTOS.ToList());
        }
     


       
        /*EFECTO: Consulta la versión del requerimiento seleccionado y la compara con el requerimiento actual (El único aprobado)
          REQUIERE: String id, int Version (Requerimiento)
          MODIFICA: -
        */
        public ActionResult ConsultarVers(string id, int version)
        {
            //var Intermedio = new ModeloIntermedioCambios();
            var Intermedio = new ModeloIntermedioCambios();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Intermedio.consultado = db.REQUERIMIENTOS.Find(id, version);
            if (Intermedio.consultado.ESTADO_CAMBIOS == "Obsoleto")
            {
                Intermedio.Cambio = Intermedio.consultado.CAMBIOS.Where(s => s.NUEVO_REQ_ID == id).First(); //Se usa para desplegar el requerimiento consultado y comparar
            }
            else
            {
                if (Intermedio.consultado.CAMBIOS1.Count != 0) {
                    Intermedio.Cambio = Intermedio.consultado.CAMBIOS1.Where(s => s.NUEVO_REQ_ID == id).First(); 
                }                
            }           
            Intermedio.actual = db.REQUERIMIENTOS.Where(s => s.ID == id && s.ESTADO_CAMBIOS == "Aprobado").First();
            if (Intermedio.consultado == null)
            {
                return HttpNotFound();
            }
            return View(Intermedio);
        }

        /* EFECTO: Muestra la vista para crear una solicitud de cambio
         * REQUIERE: Selecionar un requerimiento previamente (id y version)
         * MODIFICA: N/A  */
        public ActionResult Create(string id, int version)
        {            
            var cambio = new CAMBIOS();
            var correoUsuario = db.CORREOS.Where(s => s.CORREO == User.Identity.Name).Single();
            USUARIOS actual = correoUsuario.USUARIOS;
            int v = db.REQUERIMIENTOS.Where(s => s.ID == id).Select(s => s.VERSION_ID).Max() + 1;
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
            ViewBag.ENCARGADO = new SelectList(db.USUARIOS.Where(s => s.PRYCTOID == rEQUERIMIENTOS.PRYCTOID), "CEDULA", "NOMBRE", rEQUERIMIENTOS.ENCARGADO);

            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual

            var listauser = db.AspNetUsers.ToArray();
            for (int i = 0; i < listauser.Length; i++)
            {                           //de todos los AspNetUser del sistema, encuentra el usuario activo actualmente
                if (listauser[i].Email == User.Identity.Name)
                {
                    fg = listauser[i];                  //obtiene el AspNetUser actual
                }
            }

            AspNetRoles rol = fg.AspNetRoles.First();
            if (rol.Name == "Desarrollador" || rol.Name == "Administrador") {
                TempData["Desarrollador"] = "display:true";
            }else
            {
                TempData["Desarrollador"] = "display:none";
            }
            return View(cambio);
        }

        /* EFECTO: Guarda en la base de datos la solicitud nueva
         * REQUIERE: El modelo con los datos iingresados por el usuario
         * MODIFICA: La base de datos con los nuevos datos*/
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
                TempData["Create"] = "Exito";
            }
            else {
                TempData["Create"] = "Error";
                return RedirectToAction("Create");
            }
            return RedirectToAction("Requerimientos");
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
            List<CAMBIOS> listaCambios = db.CAMBIOS.ToList();
            modelo.listaCambios = new List<CAMBIOS>();

            HashSet<string> permisos = obtienePermisos();//permisos del usuario
            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual

            var listauser = db.AspNetUsers.ToArray();
            for (int i = 0; i < listauser.Length; i++)
            {                           //de todos los AspNetUser del sistema, encuentra el usuario activo actualmente
                if (listauser[i].Email == User.Identity.Name)
                {
                    fg = listauser[i];                  //obtiene el AspNetUser actual
                }
            }
            USUARIOS usuarioActual = db.USUARIOS.Where(u => u.ID_ASP == fg.Id).First();
            AspNetRoles rol = fg.AspNetRoles.First();
            //Administrador
            if (rol.Name == "Administrador")
            {
                foreach (var l in listaCambios)
                {//todos las solicitudes
                    if ((l.REQUERIMIENTOS1.ESTADO_CAMBIOS == "Pendiente") || (l.REQUERIMIENTOS1.ESTADO_CAMBIOS == "En revisión"))
                    {
                        modelo.listaCambios.Add(l);
                    }
                }
            }
            //lider
            if (rol.Name == "Desarrollador")
            {
                if (usuarioActual.LIDER == true)//es lider
                {
                    foreach (var l in listaCambios)
                    {//solicitudes pendientes/en revisión del proyecto donde es lider
                        if (((l.REQUERIMIENTOS1.ESTADO_CAMBIOS == "Pendiente") || (l.REQUERIMIENTOS1.ESTADO_CAMBIOS == "En revisión")) && (l.REQUERIMIENTOS.PRYCTOID == usuarioActual.PRYCTOID))
                        {
                            modelo.listaCambios.Add(l);
                        }
                    }
                }
                else
                {
                    foreach (var l in listaCambios)
                    {//solicitudes pendientes/en revisión que solicitó
                        if (((l.REQUERIMIENTOS1.ESTADO_CAMBIOS == "Pendiente") || (l.REQUERIMIENTOS1.ESTADO_CAMBIOS == "En revisión")) && (l.CEDULA == usuarioActual.CEDULA))
                        {
                            modelo.listaCambios.Add(l);
                        }
                    }
                }
            }
            //otro - usuario
            if (rol.Name == "Usuario")
            {
                foreach (var l in listaCambios)
                {//solicitudes pendientes/en revisión que solicitó
                    if (((l.REQUERIMIENTOS1.ESTADO_CAMBIOS == "Pendiente") || (l.REQUERIMIENTOS1.ESTADO_CAMBIOS == "En revisión")) && (l.CEDULA == usuarioActual.CEDULA))
                    {
                        modelo.listaCambios.Add(l);
                    }
                }
            }
            //var cAMBIOS = db.CAMBIOS.Include(c => c.REQUERIMIENTOS).Include(c => c.USUARIOS).Include(c => c.USUARIOS1).Include(c => c.REQUERIMIENTOS1);
            ViewBag.Permisos = obtienePermisos();
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

            modelo.actual = db.REQUERIMIENTOS.Where(s => s.ID == modelo.solicitud.NUEVO_REQ_ID && s.ESTADO_CAMBIOS == "Aprobado").First();

            HashSet<string> permisos = obtienePermisos();//permisos del rol del usuario

            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual

            var listauser = db.AspNetUsers.ToArray();
            for (int i = 0; i < listauser.Length; i++)
            {                           //de todos los AspNetUser del sistema, encuentra el usuario activo actualmente
                if (listauser[i].Email == User.Identity.Name)
                {
                    fg = listauser[i];                  //obtiene el AspNetUser actual
                }
            }

            var usuario = db.USUARIOS.Where(u => u.ID_ASP.Equals(fg.Id)).Single();
            if (usuario.LIDER == true && usuario.PRYCTOID == modelo.solicitud.REQUERIMIENTOS.PRYCTOID)
            {
                permisos.Add("21");
            }
            if (modelo.solicitud.CEDULA == usuario.CEDULA)
            {
                permisos.Add("16");
                permisos.Add("17");
            }
            modelo.solicitud.CED_REV = usuario.CEDULA;
            ViewBag.ENCARGADO = new SelectList(db.USUARIOS.Where(s => s.PRYCTOID == modelo.propuesto.PRYCTOID), "CEDULA", "nombreCompleto", modelo.propuesto.ENCARGADO);
            ViewBag.Permisos = permisos;
            ViewBag.Cedula = usuario.CEDULA;
            return View(modelo);
        }

        /* EFECTO: hace cambios en datos basados en la información introducida por el usuario
         * REQUIERE: el modelo con la información llenada por el usuario
         * MODIFICA: base de datos partiendo de la información que introdujo el usuario  */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DetallesSolicitud(CambiosViewModel modelo)
        {
            if (modelo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            REQUERIMIENTOS cambioActualizado = db.REQUERIMIENTOS.Find(modelo.propuesto.ID, modelo.propuesto.VERSION_ID);
            REQUERIMIENTOS reqAntiguo = db.REQUERIMIENTOS.Find(modelo.actual.ID, modelo.actual.VERSION_ID);
            cambioActualizado.NOMBRE = modelo.propuesto.NOMBRE;
            cambioActualizado.DESCRIPCION = modelo.propuesto.DESCRIPCION;
            cambioActualizado.ENCARGADO = modelo.propuesto.ENCARGADO;
            cambioActualizado.ESFUERZO = modelo.propuesto.ESFUERZO;
            cambioActualizado.ESTADO = modelo.propuesto.ESTADO;
            cambioActualizado.FECHAFINAL = modelo.propuesto.FECHAFINAL;
            cambioActualizado.FECHAINCIO = modelo.propuesto.FECHAINCIO;
            //cambioActualizado.IMAGEN = modelo.propuesto.IMAGEN;
            //cambioActualizado.rutaImagen = modelo.propuesto.rutaImagen;
            //cambioActualizado.CRIT_ACEPTACION = modelo.propuesto.CRIT_ACEPTACION;
            cambioActualizado.MODULO = modelo.propuesto.MODULO;
            cambioActualizado.OBSERVACIONES = modelo.propuesto.OBSERVACIONES;
            cambioActualizado.PRIORIDAD = modelo.propuesto.PRIORIDAD;
            //cambioActualizado.PROYECTO = modelo.propuesto.PROYECTO;
            //cambioActualizado.PRYCTOID = modelo.propuesto.PRYCTOID;
            cambioActualizado.SPRINT = modelo.propuesto.SPRINT;
            //cambioActualizado.USUARIOS = modelo.propuesto.USUARIOS;
            //cambioActualizado.VERSION_ID = modelo.propuesto.VERSION_ID;

            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual

            var listauser = db.AspNetUsers.ToArray();
            for (int i = 0; i < listauser.Length; i++)
            {                           //de todos los AspNetUser del sistema, encuentra el usuario activo actualmente
                if (listauser[i].Email == User.Identity.Name)
                {
                    fg = listauser[i];                  //obtiene el AspNetUser actual
                }
            }

            var usuario = db.USUARIOS.Where(u => u.ID_ASP.Equals(fg.Id)).Single();

            CAMBIOS cambio = db.CAMBIOS.Find(modelo.solicitud.ID);

            cambio.DESCRIPCION = modelo.solicitud.DESCRIPCION;
            cambio.FECHA = DateTime.Now;
            cambio.JUSTIFICACION = modelo.solicitud.JUSTIFICACION;

            if ((cambioActualizado.ESTADO_CAMBIOS != modelo.propuesto.ESTADO_CAMBIOS) || (modelo.propuesto.ESTADO_CAMBIOS == "En revisión"))
            {
                cambio.CED_REV = usuario.CEDULA;
                if (modelo.solicitud.FECHA_REV == null)
                {
                    cambio.FECHA_REV = DateTime.Now;
                }
                else
                {
                    cambio.FECHA_REV = modelo.solicitud.FECHA_REV;
                }
                cambio.JUST_REV = modelo.solicitud.JUST_REV;
                AceptarSolicitud(modelo, reqAntiguo, cambioActualizado, cambio);
            }

            cambioActualizado.ESTADO_CAMBIOS = modelo.propuesto.ESTADO_CAMBIOS;
            //if (ModelState.IsValid)
            //{
            db.Entry(reqAntiguo).State = EntityState.Modified;
            db.Entry(cambioActualizado).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DetallesSolicitud", new { ID = modelo.solicitud.ID });
            //}
            //return null;
        }
        
        /* EFECTO: Requerimiento antiguo
         * REQUIERE: Que el estado sea Aprobado
         * MODIFICA: El requerimiento que será el nuevo aprobado y el anterior  */
        public /*ActionResult*/ void AceptarSolicitud(CambiosViewModel modelo, REQUERIMIENTOS antiguo, REQUERIMIENTOS nuevo, CAMBIOS cambio/*, CAMBIOS actual*/)
        {
            if (modelo.propuesto.ESTADO_CAMBIOS == "Aprobado")
            {
                //antiguo.ESTADO_CAMBIOS = "Obsoleto";
                REQUERIMIENTOS pivote = nuevo;
                nuevo = antiguo;
                antiguo = pivote;
                nuevo.ESTADO_CAMBIOS = "Obsoleto";
                cambio.VIEJO_REQ_ID = cambio.NUEVO_REQ_ID;
                cambio.VIEJO_VER_ID = cambio.NUEVO_VER_ID;
                modelo.vigente = antiguo;
                //modelo.propuesto.ESTADO_CAMBIOS = modelo.propuesto.ESTADO_CAMBIOS;
                //actual = cambio;
            }
            //return modelo;
        }

        // No se utiliza este método
        public /*ActionResult*/ void RechazarSolicitud(CambiosViewModel modelo)
        {
            modelo.propuesto.ESTADO_CAMBIOS = "Rechazado";
            //return null;
        }

        /* EFECTO: Elimina una solicitud de cambio, hecha por el propio usuario
         * REQUIERE: ID 
         * MODIFICA: La información de la base de datos  */
        public ActionResult eliminarSolicitud(string id)
        {
            CAMBIOS cambio = db.CAMBIOS.Find(id);
            REQUERIMIENTOS req = db.REQUERIMIENTOS.Find(cambio.NUEVO_REQ_ID, cambio.NUEVO_VER_ID);

            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual

            var listauser = db.AspNetUsers.ToArray();
            for (int i = 0; i < listauser.Length; i++)
            {                           //de todos los AspNetUser del sistema, encuentra el usuario activo actualmente
                if (listauser[i].Email == User.Identity.Name)
                {
                    fg = listauser[i];                  //obtiene el AspNetUser actual
                }
            }

            var usuario = db.USUARIOS.Where(u => u.ID_ASP.Equals(fg.Id)).Single();

            if ((cambio.CEDULA == usuario.CEDULA) && (req.ESTADO_CAMBIOS == "Pendiente"))
            {
                db.CAMBIOS.Remove(cambio);
                db.REQUERIMIENTOS.Remove(req);
                db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
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

    }
}

