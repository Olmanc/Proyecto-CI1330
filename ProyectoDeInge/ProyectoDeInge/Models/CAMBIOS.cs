//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProyectoDeInge.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CAMBIOS
    {
        public string ID { get; set; }
        public string CEDULA { get; set; }
        public Nullable<System.DateTime> FECHA { get; set; }
        public string DESCRIPCION { get; set; }
        public string JUSTIFICACION { get; set; }
        public string REQUERIMIENTO_ID { get; set; }
        public Nullable<int> VERSION_ID { get; set; }
    
        public virtual REQUERIMIENTOS REQUERIMIENTOS { get; set; }
        public virtual USUARIOS USUARIOS { get; set; }
    }
}