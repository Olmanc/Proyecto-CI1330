
use BD_IngeGrupo2

/*insercion de nuevos permisos*/

insert into AspNetRoles values('1','Administrador');
insert into AspNetRoles values('2','Desarrollador');
insert into AspNetRoles values('3','Usuario');


insert into PERMISOS values('00','Modificar contraseña','Permite modificar la contraseña de un usuario existente');--modificar contraseña
insert into PERMISOS values('01','Asignar permisos','Permite asignar permisos a los diferentes roles del sistema');--asignar permisos
insert into PERMISOS values('02','Agregar usuarios','Permite agregar usuarios al sistema');
insert into PERMISOS values('03','Consultar usuarios','Permite consultar usuarios almacenados en el sistema');
insert into PERMISOS values('04','Modificar usuarios','Permite editar los datos de una usuario almacenado');
insert into PERMISOS values('05','Eliminar usuarios','Permite eliminar usuarios del sistema');
insert into PERMISOS values('06','Agregar proyectos','Permite crear proyectos en el sistema');
insert into PERMISOS values('07','Consultar proyectos','Permite consultr los proyectos existenetes');
insert into PERMISOS values('08','Modificar proyectos','Permite editar datos de los proyectos existentes');
insert into PERMISOS values('09','Eliminar proyectos','Permite eliminar proyectos existentes'); --los proyectos no se eliminan, se cambia su estado a cerrado/finalizado
insert into PERMISOS values('10','Agregar requerimientos funcionales','Permite agregar requerimientos funcionales a un proyecto');
insert into PERMISOS values('11','Consultar requerimientos funcionales','Permite consultar los requerimientos de un proyecto');
insert into PERMISOS values('12','Modificar requerimientos funcionales','Permite Editar los datos de los requerimientos de una proyecto');
insert into PERMISOS values('13','Eliminar requerimientos funcionales','Permite eliminar requerimientos funcionales de un proyecto');
insert into PERMISOS values('14','Realizar cambios a requerimientos','Permite agregar cambios a los requerimientos funcionales');--cambios a requerimiento
insert into PERMISOS values('15','Consultar cambios a requerimientos','Permite consultar el historial de cambios realizados a los requerimientos funcionales');--consultar cambios
insert into PERMISOS values('16','Modificar cambios a requerimientos','Permite modificar datos de los cambios a requerimientos funcionales');--modificar cambios, por si escribí mal algun dato
insert into PERMISOS values('17','Eliminar cambios a requerimientos','Permite eliminar cambios realizados del historial de cambios'); --no deberia poder borrar los cambios realizados del historial de cambios
insert into PERMISOS values('18','Asignar/Desasignar desarrolladores','Permite asignar o desasignar desarrolladores a un proyecto');--asignar/desasignar desarrolladores a un proyecto
