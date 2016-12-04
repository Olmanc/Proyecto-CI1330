using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoDeInge.Models;
using System.Data.Entity.Infrastructure;

namespace ProyectoDeInge.Controllers
{
    public class ProyectosController : Controller
    {
        private BD_IngeGrupo2Entities2 db = new BD_IngeGrupo2Entities2();

        // GET: Proyectos
        public ActionResult Index(string buscar)
        {
            ProyectosViewModel modelo = new ProyectosViewModel();
            if (buscar != null)
            {
                var proyectos = db.PROYECTO.Where(s => s.ESTADO.Contains(buscar) || s.ID.Contains(buscar)
                                                    || s.NOMBRE.Contains(buscar) || s.DESCRIPCION.Contains(buscar));
                modelo.listaProyectos = proyectos.ToList();
            }
            else
            {
                modelo.listaProyectos = db.PROYECTO.ToList(); //lista de todos los proyectos en la BD
            }
            List<PROYECTO> todoProyectos = db.PROYECTO.ToList(); //lista de todos los proyectos en la BD
            foreach (var proyecto in todoProyectos)
            {
                if (proyecto.BORRADO == true)   //si el proyecto estaba cancelado y fue borrado de la aplicación
                {                               // pero este no siempre permanece en la BD
                    modelo.listaProyectos.Remove(proyecto); //elimino proyectos que no se deben mostrar en la aplicación
                }
            }

            modelo.verificaPermisos = obtienePermisos();
            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual            
            var listauser = db.AspNetUsers.ToArray();

            for (int i = 0; i < listauser.Length; i++)
            {                           //de todos los AspNetUser del sistema, encuentra el que tenga el email activo actualmente
                if (listauser[i].Email == User.Identity.Name)
                {
                    fg = listauser[i];                  //obtiene el AspNetUser actual
                }
            }

            var rol = fg.AspNetRoles.First();   //obtiene el rol del ususario
            if (rol.Id == "2")
            {                //si el usuario es desarrollador
                USUARIOS actual = db.USUARIOS.Where(i => i.ID_ASP == fg.Id).Single();
                if (actual.PRYCTOID != null && actual.LIDER != null)
                {  //si el desarrollador es lider en un proyecto
                    if (modelo.modeloProyecto != null)
                    {
                        if (modelo.modeloProyecto.ID != null)//esta en una vista con un proyecto especificado (vista de consulta(unificado))
                        {
                            if (actual.PRYCTOID == modelo.modeloProyecto.ID && actual.LIDER == true)
                            {//otorga todos los permisos sobre el proyecto donde es lider (CRUD para proyectos, requerimientos y cambios)
                                modelo.verificaPermisos.Add("06");
                                modelo.verificaPermisos.Add("07");
                                modelo.verificaPermisos.Add("08");
                                modelo.verificaPermisos.Add("09");
                                modelo.verificaPermisos.Add("10");
                                modelo.verificaPermisos.Add("11");
                                modelo.verificaPermisos.Add("12");
                                modelo.verificaPermisos.Add("13");
                                modelo.verificaPermisos.Add("14");
                                modelo.verificaPermisos.Add("15");
                                modelo.verificaPermisos.Add("16");
                                modelo.verificaPermisos.Add("17");
                            }
                        }
                    }
                }
            }

            return View(modelo);
        }

        // GET: Proyectos/Create
        /*
         REQ: No aplica
         EFE: Carga las opciones de posibles líderes y recursos para el proyecto que se quiere crear
         MOD: Vista para agregar proyecto       
        */
        public ActionResult Create()
        {

            ProyectosViewModel proyecto = new ProyectosViewModel();//nuevo viewModel

            List<USUARIOS> todosUsuarios = db.USUARIOS.ToList();//todos los usuarios
            List<USUARIOS> lideres = new List<USUARIOS>();
            AspNetRoles rol = db.AspNetRoles.Find("2");//busco rol de desarrollador

            foreach (var item in todosUsuarios)
            {
                AspNetUsers asp = db.AspNetUsers.Find(item.ID_ASP);//busca AspNetUser del usuario

                if (asp.AspNetRoles.Contains(rol))
                {
                    if ((item.LIDER == true) && (item.PRYCTOID == null))//si esta en el proyecto lo agrega a recursos
                    {
                        lideres.Add(item);
                        //item.PRYCTOID = proyecto.modeloProyecto.ID;
                    }
                }
            }
            //llena SelectList de recursos asignados
            proyecto.lideres = lideres.Select(o => new SelectListItem
            {
                Text = o.NOMBRE + " " + o.APELLIDO1 + " " + o.APELLIDO2 + " - " + o.CEDULA,
                Value = o.NOMBRE + " " + o.APELLIDO1 + " " + o.APELLIDO2 
                //Value = o.CEDULA.ToString()
            });
            ViewBag.liderProyecto = new SelectList(proyecto.lideres, "Text", "Value");
            proyecto.verificaPermisos = obtienePermisos();
            var fg = new AspNetUsers();    //instancia AspNetUser para usuario actual            
            var listauser = db.AspNetUsers.ToArray(); // array de users de ASP

            for (int i = 0; i < listauser.Length; i++)
            {    
                if (listauser[i].Email == User.Identity.Name)
                {
                    fg = listauser[i];                  //obtiene el AspNetUser actual
                }
            }
            var role = fg.AspNetRoles.First();   //obtiene el rol del ususario
            if (role.Id == "2")
            {      //si el usuario es desarrollador
                USUARIOS actual = db.USUARIOS.Where(i => i.ID_ASP == fg.Id).Single();
                if (actual.PRYCTOID != null && actual.LIDER != null)
                {  //si el desarrollador es lider en un proyecto
                    if (proyecto.modeloProyecto != null)
                    {
                        if (proyecto.modeloProyecto.ID != null)//esta en una vista con un proyecto especificado (vista de consulta(unificado))
                        {
                            if (actual.PRYCTOID == proyecto.modeloProyecto.ID && actual.LIDER == true)
                            {//otorga todos los permisos sobre el proyecto donde es lider (CRUD para proyectos, requerimientos y cambios)
                                proyecto.verificaPermisos.Add("06");
                                proyecto.verificaPermisos.Add("07");
                                proyecto.verificaPermisos.Add("08");
                                proyecto.verificaPermisos.Add("09");
                                proyecto.verificaPermisos.Add("10");
                                proyecto.verificaPermisos.Add("11");
                                proyecto.verificaPermisos.Add("12");
                                proyecto.verificaPermisos.Add("13");
                                proyecto.verificaPermisos.Add("14");
                                proyecto.verificaPermisos.Add("15");
                                proyecto.verificaPermisos.Add("16");
                                proyecto.verificaPermisos.Add("17");
                            }
                        }
                    }
                }
            }

            populateUsuarios_Create(proyecto.modeloProyecto); //llenar tabla de recursos asignados y disponibles
            TempData["Parcial"] = "crear";
            return View(proyecto);
        }


        // POST: Proyectos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*
         REQ: ModeloIntermedio del proyecto por crearse y campos obligatorios llenos
         EFE: Crea un nuevo proyecto
         MOD: La base de datos (modifica los atributos respectivos de acuerdo a lo que el usuario digitó)        
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(/*ModeloIntermedio pROYECTO*/ProyectosViewModel pROYECTO)
        {
            //if (ModelState.IsValid)
            //{

            if (db.PROYECTO.Find(pROYECTO.modeloProyecto.ID) != null) {
                TempData["Create"] = "Existe";
                return RedirectToAction("Create");
            }

            if (pROYECTO.modeloProyecto == null)//si la vista no devolvio un proyecto en el ProyectosViewmodel
            {//tira error
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if ((pROYECTO.modeloProyecto.ID != null) || (pROYECTO.modeloProyecto.NOMBRE != null))
            {
                ProyectosViewModel proyecto = new ProyectosViewModel();//nuevo viewModel
                ViewBag.ESTADO = new SelectList(db.PROYECTO, "ESTADO"); //Viewbag de estados de proyecto
                db.PROYECTO.Add(pROYECTO.modeloProyecto);

                if (pROYECTO.modeloUsuario.PRYCTOID != null)
                {
                    var proyectoID = pROYECTO.modeloUsuario.PRYCTOID; //id del proyecto donde se asignó el usuario
                    string cedula = string.Empty; // nuevo string
                    int val; //nuevo entero

                    for (int i = 0; i < proyectoID.Length; i++)
                    { //buscar id del usuario para asignarle el proyecto
                        if (Char.IsDigit(proyectoID[i]))
                            cedula += proyectoID[i];
                    }

                    if (cedula.Length > 0)
                    {
                        val = int.Parse(cedula);
                    }
                    USUARIOS uSUARIO = db.USUARIOS.Find(cedula); //buscar usuario por cédula
                    uSUARIO.PRYCTOID = pROYECTO.modeloProyecto.ID; //asignarle el proyecto al usuario
                }

                if (pROYECTO.modeloProyecto.FECHAINICIO == null) //Si la fecha de inicio es nula la fecha por defecto corresponde la del día de creación del proyecto
                {
                    var fechaInicial = DateTime.Now; //fecha del día respectivo a la creación del proyecto
                    pROYECTO.modeloProyecto.FECHAINICIO = fechaInicial; //se le asigna tal fecha a la fecha inicial de modeloProyecto
                }

                if ((pROYECTO.modeloProyecto.FECHAINICIO != null) && (pROYECTO.modeloProyecto.FECHAFINAL != null) && ((pROYECTO.modeloProyecto.DURACION == null) || (pROYECTO.modeloProyecto.DURACION == 0)))
                { //calculo de la duracion, a partir de fechaFinalizacion-FechaInicial, por si el usuario la deja nula
                    bool b = false;
                    var dias = "";
                    int totalDias = 0;
                    var fechaInicial = pROYECTO.modeloProyecto.FECHAINICIO;
                    var fechaFinal = pROYECTO.modeloProyecto.FECHAFINAL;
                    var duracion = fechaFinal - fechaInicial;
                    var d = duracion.ToString();

                    for (int i = 0; ((i < d.Length) && (b == false)); i++)
                    { //conversion de formato date a int
                        if (d[i].Equals('.'))
                        {
                            b = true;
                            for (int j = 0; j < i; j++)
                            {
                                dias += d[j];
                            }
                            totalDias = Int32.Parse(dias.ToString());
                        }
                    }
                    int meses = totalDias / 30; // convertir de dias a meses
                    pROYECTO.modeloProyecto.DURACION = meses; //se le asigna al atributo duracion de modeloProyecto
                                                              //totalDias = 67890;
                }
                db.SaveChanges(); //guarda todos los cambios
                TempData["Create"] = "Exito";
                return RedirectToAction("Index"); //regresa al Index de proyectos
            }
            else
            {
                TempData["Create"] = "Error";                
                return RedirectToAction("Create");
            }
            
            //}

            //return View(pROYECTO);
        }



        /*
         REQ: NA
         EFE: carga los Id de los permisos del rol asignado al usuario activo
         MOD: Hashset de los Id de los permisos concdidos (string)
             */
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


        /*
         REQ: string (Id del rol que se quiere buscar)
         EFE: obtiene a todos los usuarios del rol especificado
         MOD: Lista de los usuarios pertenecientes al rol
             */
        public List<USUARIOS> buscaUsuariosRol(string rolId)
        {
            List<USUARIOS> usuariosDeRol = new List<USUARIOS>();//lista de usuarios del rol
            var todosUsuarios = db.USUARIOS.ToList();//todos los usuarios del sistema
            AspNetRoles rol = db.AspNetRoles.Find(rolId);//busca el rol con Id = rolId

            foreach (var item in todosUsuarios)//busca todos los usuarios
            {
                AspNetUsers user = db.AspNetUsers.Find(item.ID_ASP);
                if (user.AspNetRoles.Contains(rol))//si el usuario es del rol buscado
                {
                    usuariosDeRol.Add(item);//es agregado a la lista
                }
            }
            return usuariosDeRol;//MOD lista de usuarios con el rol buscado
        }


        /*
         REQ: NA
         EFE: busca a todos los desarrolladores de la base de datos
         MOD: lista de todos los desarrolladores
             */
        public List<USUARIOS> buscaResursos()
        {
            return buscaUsuariosRol("2");//devuelve a todos los desarrolladores
        }


        /*
        REQ: proyecto que se está agregando
        EFE: carga ViewBags con los desarrolladores asignados al proyecto (parametro) y los que no estan asignados a ningun proyecto
        MOD: NA
       */
        public void populateUsuarios_Create(PROYECTO proyecto)
        {
            var todosRecursos = buscaResursos();            

            var recursosAsignados = new List<RecursosViewModel>();
            var recursosDisponibles = new List<RecursosViewModel>();
            foreach (var rec in todosRecursos)
            {
                if (rec.PRYCTOID == null)
                {
                    if ((rec.PRYCTOID == null) && (rec.LIDER == false))
                    {
                        recursosDisponibles.Add(new RecursosViewModel
                        {
                            Cedula = rec.CEDULA,
                            Nombre = rec.NOMBRE,
                            usuarioProyecto = rec.PRYCTOID,
                            Apellido1 = rec.APELLIDO1,
                            Apelliso2 = rec.APELLIDO2,
                            usuarioID = rec.ID_ASP
                        });
                    }
                   
                }
            }
            ViewBag.Asignados = new MultiSelectList(recursosAsignados, "Cedula", "nombreCompleto");
            ViewBag.Disponibles = new MultiSelectList(recursosDisponibles, "Cedula", "nombreCompleto");
        }

        public PartialViewResult llenaRecursos(string id) {
            var todosRecursos = buscaResursos();//obtiene a todos los desarrolladores            
            var modeloParcial = new ProyectosViewModel();

            //var recursos = new HashSet<string>(proyecto.USUARIOS.Select(u => u.CEDULA));//
            var recursosasignados = new List<RecursosViewModel>();//lista para recursos asignados al proyecto
            var recursosdisponibles = new List<RecursosViewModel>();//lista para recursos sin proyecto asignado

            var lider = db.USUARIOS.Find(id);

            foreach (var rec in todosRecursos)//de de todos los desarrolladores
            {
                if (rec.PRYCTOID == lider.PRYCTOID && rec.CEDULA != id)//si estan asignados al proyecto, es agregado a la lista de recursos
                {
                    recursosasignados.Add(new RecursosViewModel
                    {
                        Cedula = rec.CEDULA,
                        Nombre = rec.NOMBRE,
                        usuarioProyecto = rec.PRYCTOID,
                        Apellido1 = rec.APELLIDO1,
                        Apelliso2 = rec.APELLIDO2,
                        usuarioID = rec.ID_ASP
                    });
                }
                else
                {//if(rec.PRYCTOID == null), para que no cargue los que ya estan trabajando en otro proyecto
                    if (rec.PRYCTOID == null)
                    {//agrega a la lista de desarrolladores disponibles
                        recursosdisponibles.Add(new RecursosViewModel
                        {
                            Cedula = rec.CEDULA,
                            Nombre = rec.NOMBRE,
                            usuarioProyecto = rec.PRYCTOID,
                            Apellido1 = rec.APELLIDO1,
                            Apelliso2 = rec.APELLIDO2,
                            usuarioID = rec.ID_ASP
                        });
                    }
                }
            }
            TempData["Parcial"] = "parcial";
            //llena viewBags para cada lista
            ViewBag.Asignados = new MultiSelectList(recursosasignados, "Cedula", "nombreCompleto");
            ViewBag.Disponibles = new MultiSelectList(recursosdisponibles, "Cedula", "nombreCompleto");
            return PartialView("_RecursosPartial", modeloParcial);
        }


        /*
         REQ: proyecto que se va a consultar/modificar/eliminar
         EFE: carga ViewBags con los desarrolladores asignados al proyecto (parametro) y los que no estan asignados a ningun proyecto
         MOD: NA
             */
        public void populateUsuarios(PROYECTO proyecto)
        {
            var todosRecursos = buscaResursos();//obtiene a todos los desarrolladores            

            //var recursos = new HashSet<string>(proyecto.USUARIOS.Select(u => u.CEDULA));//
            var recursosAsignados = new List<RecursosViewModel>();//lista para recursos asignados al proyecto
            var recursosDisponibles = new List<RecursosViewModel>();//lista para recursos sin proyecto asignado
            var listaLideres = new List<RecursosViewModel>();

            var lider = db.USUARIOS.Where(u => u.PRYCTOID == proyecto.ID && u.LIDER == true).Single();

            foreach (var rec in todosRecursos)//de de todos los desarrolladores
            {
                if (rec.PRYCTOID == proyecto.ID/* && rec.CEDULA != lider.CEDULA*/)//si estan asignados al proyecto, es agregado a la lista de recursos
                {
                    listaLideres.Add(new RecursosViewModel
                    {
                        Cedula = rec.CEDULA,
                        Nombre = rec.NOMBRE,
                        usuarioProyecto = rec.PRYCTOID,
                        Apellido1 = rec.APELLIDO1,
                        Apelliso2 = rec.APELLIDO2,
                        usuarioID = rec.ID_ASP
                    });
                    if (rec.LIDER == false) {
                        recursosAsignados.Add(new RecursosViewModel
                        {
                            Cedula = rec.CEDULA,
                            Nombre = rec.NOMBRE,
                            usuarioProyecto = rec.PRYCTOID,
                            Apellido1 = rec.APELLIDO1,
                            Apelliso2 = rec.APELLIDO2,
                            usuarioID = rec.ID_ASP
                        });
                    }                    
                }
                else
                {//if(rec.PRYCTOID == null), para que no cargue los que ya estan trabajando en otro proyecto
                    if (rec.PRYCTOID == null) {//agrega a la lista de desarrolladores disponibles
                        recursosDisponibles.Add(new RecursosViewModel
                        {
                            Cedula = rec.CEDULA,
                            Nombre = rec.NOMBRE,
                            usuarioProyecto = rec.PRYCTOID,
                            Apellido1 = rec.APELLIDO1,
                            Apelliso2 = rec.APELLIDO2,
                            usuarioID = rec.ID_ASP
                        });
                    }                    
                }
            }
            //llena viewBags para cada lista
            ViewBag.Asignados = new MultiSelectList(recursosAsignados, "Cedula", "nombreCompleto");
            ViewBag.Disponibles = new MultiSelectList(recursosDisponibles, "Cedula", "nombreCompleto");
            ViewBag.Lideres = new MultiSelectList(listaLideres, "Cedula", "nombreCompleto");
        }

        /*
         REQ: string del Id del proyecto consultado
         EFE: carga los datos del proyecto con sus recursos asignados y los permisos del usuario actual
         MOD: Vista unificada para consulta/modificacion/eliminado de proyectos
            */
        public ActionResult Unificado(string id) {
            if (id == null){//si id es null
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProyectosViewModel proyecto = new ProyectosViewModel();//nuevo viewModel para pasar a la vista

            proyecto.modeloProyecto = db.PROYECTO.Find(id);//encuentra al proyecto con ID = id
            
            if (proyecto.modeloProyecto == null)
            {//si el proyecto no existe, tira error
                return HttpNotFound();
            }

            proyecto.modeloUsuario = db.USUARIOS.Where(u => u.PRYCTOID == id && u.LIDER == true).Single();
                     
            proyecto.verificaPermisos = obtienePermisos();//obtiene los permisos del rol del usuario

            ////
            var fg = new AspNetUsers();                 //instancia AspNetUser para usuario actual
                    
            var listauser = db.AspNetUsers.ToArray();
            for (int i = 0; i < listauser.Length; i++)
            {                           //de todos los AspNetUser del sistema, encuentra el usuario activo actualmente
                if (listauser[i].Email == User.Identity.Name)
                {
                    fg = listauser[i];                  //obtiene el AspNetUser actual
                }
            }

            USUARIOS actual = db.USUARIOS.Where(i => i.ID_ASP == fg.Id).Single(); //obtiene el usuario actual, a partir del AspNetUser actual
            if (actual.PRYCTOID == proyecto.modeloProyecto.ID && actual.LIDER == true)//si es lider de un proyecto
            {//otorga todos los permisos sobre los proyectos (CRUD para proyectos, requerimientos y cambios)
                proyecto.verificaPermisos.Add("06");
                proyecto.verificaPermisos.Add("07");
                proyecto.verificaPermisos.Add("08");
                proyecto.verificaPermisos.Add("09");
                proyecto.verificaPermisos.Add("10");
                proyecto.verificaPermisos.Add("11");
                proyecto.verificaPermisos.Add("12");
                proyecto.verificaPermisos.Add("13");
                proyecto.verificaPermisos.Add("14");
                proyecto.verificaPermisos.Add("15");
                proyecto.verificaPermisos.Add("16");
                proyecto.verificaPermisos.Add("17");
            }


            populateUsuarios(proyecto.modeloProyecto);//llena los viewbags para recursos asignados y disponibles

            return View(proyecto);
        }

        /*
         REQ: ProyectoViewmodel del proyecto modificado y arrays de las cedula de los recursos asignados y desasignados
         EFE: guarda cambios efectuados a los datos del proyecto y asigna/desasigna recursos de acuerdo a los arrays
         MOD: Vista unificada para consulta, con los datos modificados         
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unificado(ProyectosViewModel modelo, string[] recursosAsignados, string[] recursosDisponibles)
        {

            if (modelo.modeloProyecto == null)//si la vista no devolvio un proyecto en el ProyectosViewmodel
            {//tira error
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //si intenta cambiar el estado a finalizado
            if (modelo.modeloProyecto.ESTADO == "Finalizado")
            {
                //revisa requerimientos del proyecto
                var requerimientos = db.REQUERIMIENTOS.ToList();
                foreach (var item in requerimientos)
                {
                    if (item.PRYCTOID == modelo.modeloProyecto.ID)
                    {//si todos los requerimientos estan finalizados o cancelados
                        if (item.ESTADO == "Finalizado" || item.ESTADO == "Cancelado")
                        {
                            //no hay problema
                        }
                        else
                        {//si hay requerimientos sin finalizar, muestra mensaje de error
                            TempData["Unificado"] = "Finalizado";
                            //devuelve a la pantalla de consulta del proyecto
                            return this.Unificado(modelo.modeloProyecto.ID);
                        }
                    }
                }
            }

            if (modelo.modeloProyecto.FECHAINICIO == null) //fecha por dEFE: El del día de creación del proyecto (del sistema)
            {
                var fechaInicial = DateTime.Now;
                modelo.modeloProyecto.FECHAINICIO = fechaInicial;
            }
            if (modelo.modeloProyecto.FECHAFINAL != null)
            {//si se especificó una fecha de finalizacion
                if (modelo.modeloProyecto.FECHAFINAL < modelo.modeloProyecto.FECHAINICIO)
                {//verifica que la fecha de inicio es antes que la fecha de finalizacion
                    TempData["Unificado"] = "Fecha";
                    //devuelve a la pantalla de consulta del proyecto
                    return this.Unificado(modelo.modeloProyecto.ID);
                }
            }


            //intenta actualizar los datos del proyecto
            try
            {
                var lider = db.USUARIOS.Find(modelo.modeloUsuario.CEDULA);//asigna recurso como lider del proyecto
                lider.LIDER = true;
                db.Entry(lider).State = EntityState.Modified;
                db.SaveChanges();

                //si esta en la lista de recursos asignados, es asignado al proyecto
                if (recursosAsignados != null)
                {
                    foreach (var r in recursosAsignados)
                    {
                        var variable = db.USUARIOS.Find(r);
                        variable.LIDER = false;                     //desasigna lider anterior
                        variable.PRYCTOID = modelo.modeloProyecto.ID;
                        db.Entry(variable).State = EntityState.Modified;
                        db.SaveChanges();
                        modelo.modeloProyecto.USUARIOS.Add(variable);
                    }
                }//si esta en la lista de recursos disponible, es desasignado del proyecto
                if (recursosDisponibles != null)
                {
                    foreach (var r in recursosDisponibles)
                    {
                        var variable = db.USUARIOS.Find(r);
                        variable.PRYCTOID = null;
                        db.Entry(variable).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                //si ambas fechas fueron establecidas
                if ((modelo.modeloProyecto.FECHAINICIO != null) && (modelo.modeloProyecto.FECHAFINAL != null))
                {
                    bool b = false;
                    var dias = "";
                    int totalDias = 0;
                    var fechaInicial = modelo.modeloProyecto.FECHAINICIO;//fecha de inicio
                    var fechaFinal = modelo.modeloProyecto.FECHAFINAL;//fecha de finalizacon
                    var duracion = fechaFinal - fechaInicial;//tiempo entre ambas fechas
                    var d = duracion.ToString();

                    for (int i = 0; ((i < d.Length) && (b == false)); i++)
                    {
                        if (d[i].Equals('.'))
                        {
                            b = true;
                            for (int j = 0; j < i; j++)
                            {
                                dias += d[j];
                            }
                            totalDias = Int32.Parse(dias.ToString());//cantidad de dias previsto para el proyecto
                        }
                    }
                    int meses = totalDias / 30;//cantidad de meses calculado para el proyecto
                    modelo.modeloProyecto.DURACION = meses;//asigna duracion para el proyecto
                    totalDias = 67890;
                }
                var p = db.PROYECTO.Find(modelo.modeloProyecto.ID);
                p.NOMBRE = modelo.modeloProyecto.NOMBRE;
                p.DESCRIPCION = modelo.modeloProyecto.DESCRIPCION;
                p.FECHAINICIO = modelo.modeloProyecto.FECHAINICIO;
                p.FECHAFINAL = modelo.modeloProyecto.FECHAFINAL;
                p.DURACION = modelo.modeloProyecto.DURACION;
                p.ESTADO = modelo.modeloProyecto.ESTADO;
                //si el modelo fue modificado
                db.Entry(p).State = EntityState.Modified;
                //guarda cambios en la base de datos
                db.SaveChanges();
                //mensaje de cambio exitoso
                TempData["Unificado"] = "Exito";//Response.Write("<Script>alert('Proyecto modificado exitosamente')</Script>");                
                //devuelve al listado de proyectos
                return RedirectToAction("Unificado", new { id = modelo.modeloProyecto.ID });
            }
            catch (RetryLimitExceededException)
            {   //si no funciona el try, devuelve mensaje de error
                TempData["Unificado"] = "Error";                
            }
            //mensaje de error
            TempData["Unificado"] = "Error";
            modelo.verificaPermisos = obtienePermisos();
            populateUsuarios(modelo.modeloProyecto);
            //devuelve al listado de proyectos
            //return this.Unificado(modelo.modeloProyecto.ID);
            return RedirectToAction("Unificado", new { id = modelo.modeloProyecto.ID });
        }



        //////////////////////////////////////////////////////////////////////////////////////////////
        //Eliminar Proyecto
        public ActionResult eliminarProyecto(string id)
        {
            PROYECTO proyect = db.PROYECTO.Find(id);
            List<USUARIOS> users = db.USUARIOS.ToList();    //lista de todos los usuarios
            if (proyect.ESTADO == "Finalizado" || proyect.ESTADO == "Cerrado")
            {
                //db.PROYECTO.Remove(proyect);  //esto no porque solo se borra en la aplicación pero debe permanecer en la BD
                proyect.BORRADO = true; //para indicar que no se debe mostrar en la aplicación
                //db.SaveChanges();
                foreach (var persona in users)
                {
                    if (persona.PRYCTOID == id) //si el usuario trabaja en el proyecto a borrar
                    {
                        persona.PRYCTOID = null; // cambie el proyecto que trabaja a NULL
                    }
                }
                db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
    }
}
