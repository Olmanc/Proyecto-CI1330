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

        
        // GET: Roles/Edit/5
        public ActionResult Edit(/*String id*/)
        {
            var modelo = new permiso2Viewmodel();
            modelo.Admin = db.AspNetRoles.Find("1");    //consigue los roles del administrador
            modelo.Desa = db.AspNetRoles.Find("2");     //consigue los roles del desarrollador
            modelo.Usuario = db.AspNetRoles.Find("3");  //consigue los roles del usuario
            modelo.Roles = db.AspNetRoles.ToList();     //lista de TODOS los roles, para la vista
            modelo.Permisos = db.PERMISOS.ToList();     //lista de permisos, para la vista

                       
            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual
            var listauser = db.AspNetUsers.ToArray();
            for (int i = 0; i<listauser.Length; i++) {  //de todos los AspNetUser del sistema, encuentra el que tenga el email activo actualmente
                if (listauser[i].Email == User.Identity.Name) {
                    fg = listauser[i];                  //obtiene el AspNetUser actual
                }
            }
            
            AspNetRoles role = fg.AspNetRoles.First();  //consigue el rol del usuario
            var per = role.PERMISOS;                    //copia los permisos que tiene asignado

            foreach (PERMISOS p in role.PERMISOS) {     //los copia a un HashSet<string>
                modelo.verificaPermisos.Add(p.ID);
            }         

            return View(modelo);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection resultado)
        {         
            if (resultado.Count < db.AspNetRoles.Count()+1)//verifica que todos los roles tienen al menos un permiso asignado
            {
                Response.Write("<Script>alert('ERROR - No pueden haber roles sin permisos.')</Script>");
                return this.Edit();                
            }
            permisosViewModel Administrador = new permisosViewModel();  //crea un administrador
            permisosViewModel Desarrollador = new permisosViewModel();  //crea un desarrollador
            permisosViewModel Usuario = new permisosViewModel();        //crea un usuario
            List<string> permisosAdmin = new List<string>();            //lista de permisos de administrador
            List<string> permisosDesarrollador = new List<string>();    //lista de permisos de desarrollador
            List<string> permisosUsuario = new List<string>();          //lista de permisos de usuario

            var listaAd = resultado["Administrador"];   //checkbox marcados para administrador
            var listaDe = resultado["Desarrollador"];   //checkbox marcados para desarrollador
            var listaUs = resultado["Usuario"];         //checkbox marcados para usuario
            var lista2 = new string[4];

            
            permiso2Viewmodel prueba = new permiso2Viewmodel();
            prueba.Permisos = db.PERMISOS.ToList();            
            var lista = listaAd.Split(',');         
            
            if (lista.Count() < prueba.Permisos.Count) {    //verifica que el administrador tiene todos los permisos
                Response.Write("<Script>alert('ERROR - El administrador debe tener TODOS los permisos asignados.')</Script>");
                return this.Edit();
            }
            
            for (int i = 0; i < lista.Length; i++)      //obtiene los checkbox de los permisos marcados para Administrador (Todos)
            {                
                permisosAdmin.Add(lista[i]);
            }

            lista = listaDe.Split(',');     
            if (lista.Contains("05") || lista.Contains("04")) //verifica que el rol no pueda modificar/eliminar usuarios si no los puede consultar
            {
                if (!lista.Contains("03"))
                {
                    Response.Write("<Script>alert('ERROR - No es posible asignar permiso de Modificar/Eliminar sin permiso para Consultar.')</Script>");
                    return this.Edit();
                }
            }
            for (int i = 0; i < lista.Length; i++)      //obtiene los checkbox de los permisos marcados para Desarrollador
            {                
                permisosDesarrollador.Add(lista[i]);
            }

            lista = listaUs.Split(',');
            if (lista.Contains("05") || lista.Contains("04")) //verifica que el rol no pueda modificar/eliminar usuarios si no los puede consultar
            {
                if (!lista.Contains("03"))
                {
                    Response.Write("<Script>alert('ERROR - No es posible asignar permiso de Modificar/Eliminar sin permiso para Consultar.')</Script>");
                    return this.Edit();
                }
            }
            for (int i = 0; i < lista.Length; i++)      //obtiene los checkbox de los permisos marcados para Usuario
            {                
                permisosUsuario.Add(lista[i]);
            }
            
            if (Administrador == null)                          //verifica que el administrador fue instanciado
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Administrador.permisosAsignados = permisosAdmin;            //asigna los nuevos permisos al administrador
            
            if (Desarrollador == null)                          //verifica que el desarrollador fue instanciado
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Desarrollador.permisosAsignados = permisosDesarrollador;    //asigna nuevos permisos al desarrollador
            
            if (Usuario == null)                                //verifica que el usuario fue instanciado
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario.permisosAsignados = permisosUsuario;                //asigna nuevos permisos al usuario

            TempData["Error"] = "error";

            try
            {       //intenta actualizar la base
                if (ModelState.IsValid)                             //verifica que el modelo es válido
                {
                    var adminActualiza = db.AspNetRoles             //permisos asignados al Administrador (encuentra por Id = "1")
                    .Include(i => i.PERMISOS).First(i => i.Id == "1");

                    if (TryUpdateModel(adminActualiza, "AspNetRoles", new string[] { "Name" }))
                    {
                        var adminNuevos = db.PERMISOS.Where(        //los nuevos permisos asignados al administrador
                            m => Administrador.permisosAsignados.Contains(m.ID)).ToList();
                        var adminActualizado = new HashSet<string>(Administrador.permisosAsignados);    //Set de los ID de los permisos asignados (Administrador)

                        foreach (PERMISOS permiso in db.PERMISOS)           //para todos los permisos del sistema
                        {
                            if (!adminActualizado.Contains(permiso.ID))     //si el permisos no está en los nuevos permisos asignados
                            {
                                adminActualiza.PERMISOS.Remove(permiso);    //se elimina
                            }
                            else
                            {                                               //si el permiso está entre los nuevos permisos
                                adminActualiza.PERMISOS.Add(permiso);       //se agrega al sistema
                            }
                        }
                        db.Entry(adminActualiza).State = System.Data.Entity.EntityState.Modified;   //actualiza la base de datos
                        db.SaveChanges();       //guarda los cambios
                    }

                    var desaActualiza = db.AspNetRoles       //permisos asignados al Desarrollador (encuentra por Id = "2")
                    .Include(i => i.PERMISOS).First(i => i.Id == "2");

                    if (TryUpdateModel(desaActualiza, "AspNetRoles", new string[] { "Name" }))
                    {
                        var desaNuevos = db.PERMISOS.Where(         //los nuevos permisos asignados al desarrollador
                            m => Desarrollador.permisosAsignados.Contains(m.ID)).ToList();
                        var desaActualizado = new HashSet<string>(Desarrollador.permisosAsignados);     //Set de los ID de los permisos asignados (Desarrollador)

                        foreach (PERMISOS permiso in db.PERMISOS)           //para todos los permisos del sistema
                        {
                            if (!desaActualizado.Contains(permiso.ID))      //si el permisos no está en los nuevos permisos asignados
                            {
                                desaActualiza.PERMISOS.Remove(permiso);     //se elimina
                            }
                            else
                            {                                               //si el permiso está entre los nuevos permisos
                                desaActualiza.PERMISOS.Add(permiso);        //se agrega al sistema
                            }
                        }
                        db.Entry(desaActualiza).State = System.Data.Entity.EntityState.Modified;    //actualiza la base de datos
                        db.SaveChanges();       //guarda los cambios
                    }

                    var usuActualiza = db.AspNetRoles        //permisos asignados al Usuario (encuentra por Id = "3")
                    .Include(i => i.PERMISOS).First(i => i.Id == "3");

                    if (TryUpdateModel(usuActualiza, "AspNetRoles", new string[] { "Name" }))
                    {
                        var usuNuevos = db.PERMISOS.Where(          //los nuevos permisos asignados al desarrollador
                            m => Usuario.permisosAsignados.Contains(m.ID)).ToList();
                        var usuActualizado = new HashSet<string>(Usuario.permisosAsignados);            //Set de los ID de los permisos asignados (Usuario)

                        foreach (PERMISOS permiso in db.PERMISOS)           //para todos los permisos del sistema
                        {
                            if (!usuActualizado.Contains(permiso.ID))       //si el permisos no está en los nuevos permisos asignados
                            {
                                usuActualiza.PERMISOS.Remove(permiso);      //se elimina
                            }
                            else
                            {                                               //si el permiso está entre los nuevos permisos
                                usuActualiza.PERMISOS.Add(permiso);         //se agrega al sistema
                            }
                        }
                        db.Entry(usuActualiza).State = System.Data.Entity.EntityState.Modified;     //actualiza la base de datos
                        db.SaveChanges();       //guarda los cambios
                    }
                    TempData["Exito"] = "Permisos asignados exitosamente!";
                    TempData["Error"] = null;                 
                    return RedirectToAction("Index");
                }
            }
            catch
            {   //si falla, retorna a la misma pantalla
                return this.Edit();
            }
            return RedirectToAction("Index");       //cuando es exitoso, es redirigido a una vista para ver los permisos de cada rol
        }
    }  
}
