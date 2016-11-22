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
    using System.ComponentModel.DataAnnotations;

    public partial class REQUERIMIENTOS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public REQUERIMIENTOS()
        {
            this.CAMBIOS = new HashSet<CAMBIOS>();
            this.CAMBIOS1 = new HashSet<CAMBIOS>();
            this.CRIT_ACEPTACION = new HashSet<CRIT_ACEPTACION>();
        }
    
        public string ID { get; set; }
        public string NOMBRE { get; set; }
        public Nullable<int> ESFUERZO { get; set; }
        public byte[] IMAGEN { get; set; }
        public string DESCRIPCION { get; set; }
        public Nullable<int> PRIORIDAD { get; set; }
        public string rutaImagen { get; set; }
        public string OBSERVACIONES { get; set; }
        public string MODULO { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FECHAINCIO { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FECHAFINAL { get; set; }
        public string ESTADO { get; set; }
        public string ENCARGADO { get; set; }
        public string PRYCTOID { get; set; }
        public int VERSION_ID { get; set; }
        public string ESTADO_CAMBIOS { get; set; }
        public Nullable<int> SPRINT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAMBIOS> CAMBIOS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAMBIOS> CAMBIOS1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRIT_ACEPTACION> CRIT_ACEPTACION { get; set; }
        public virtual PROYECTO PROYECTO { get; set; }
        public virtual USUARIOS USUARIOS { get; set; }
        public HashSet<string> verificaPermisos = new HashSet<string>();
        internal void crearCriterios(int c = 1)
        {
            for (int i = 0; i < c; i++)
            {
                CRIT_ACEPTACION.Add(new CRIT_ACEPTACION());
            }
        }
    }
}
