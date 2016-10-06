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

        // tomado del ejemplo hecho por Gaudy
        [Required(ErrorMessage = "La c�dula es un campo requerido.")]
        [StringLength(11)]
        [Display(Name = "C�dula:")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "La c�dula solo puede estar compuesta por n�meros")]
        public string CEDULA { get; set; }

        // tomado del ejemplo hecho por Gaudy
        [StringLength(20)]
        [Required(ErrorMessage = "El nombre es un campo requerido.")]
        [Display(Name = "Nombre:")]
        [RegularExpression(@"^[a-zA-Z''-'\s]+$", ErrorMessage = "El nombre solo puede estar compuesto por letras")]
        public string NOMBRE { get; set; }

        [Display(Name = "Identificador del proyecto:")]
        public string PRYCTOID { get; set; }

        [Display(Name = "�Es l�der?:")]
        public Nullable<bool> LIDER { get; set; }

        // tomado del ejemplo hecho por Gaudy
        [StringLength(20)]
        [Required(ErrorMessage = "El primer apellido es un campo requerido.")]
        [Display(Name = "Primer apellido:")]
        [RegularExpression(@"^[a-zA-Z''-'\s]+$", ErrorMessage = "El primer apellido solo puede estar compuesto por letras")]
        public string APELLIDO1 { get; set; }

        // tomado del ejemplo hecho por Gaudy
        [StringLength(20)]
        [Required(ErrorMessage = "El segundo apellido es un campo requerido.")]
        [Display(Name = "Segundo apellido:")]
        [RegularExpression(@"^[a-zA-Z''-'\s]+$", ErrorMessage = "El segundo apellido solo puede estar compuesto por letras")]
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
