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
    
    public partial class USUARIOS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public USUARIOS()
        {
            this.CAMBIOS = new HashSet<CAMBIOS>();
            this.CORREOS = new HashSet<CORREOS>();
            this.REQUERIMIENTOS = new HashSet<REQUERIMIENTOS>();
            this.TELEFONOS = new HashSet<TELEFONOS>();
        }
    
        public string CEDULA { get; set; }
        public string NOMBRE { get; set; }
        public string PRYCTOID { get; set; }
        public Nullable<bool> LIDER { get; set; }
        public string APELLIDO1 { get; set; }
        public string APELLIDO2 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAMBIOS> CAMBIOS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CORREOS> CORREOS { get; set; }
        public virtual PROYECTO PROYECTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REQUERIMIENTOS> REQUERIMIENTOS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TELEFONOS> TELEFONOS { get; set; }
    }
}