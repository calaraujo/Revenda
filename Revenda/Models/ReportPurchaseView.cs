using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class ReportPurchaseView
    {

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataBrasil(ErrorMessage = "Data inválida.", DataRequerida = false)]
        [Display(Name = "Data Inicial")]
        public DateTime InitialDate { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataBrasil(ErrorMessage = "Data inválida.", DataRequerida = false)]
        [Display(Name = "Data Final")]
        public DateTime EndDate { get; set; }
    }
}