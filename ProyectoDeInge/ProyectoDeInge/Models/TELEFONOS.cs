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

    public partial class TELEFONOS
    {
        public string CEDULA { get; set; }

        [StringLength(8)]
        [Display(Name = "Tel�fono:")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "El tel�fono solo puede estar compuesta por n�meros")]
        public string NUMERO { get; set; }

        public virtual USUARIOS USUARIOS { get; set; }
    }
}
