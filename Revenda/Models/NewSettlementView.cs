using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class NewSettlementView
    {
        public int SettlementId { get; set; }

        public int ComissionId { get; set; }

        public int PayableId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataBrasil(ErrorMessage = "Data inválida.", DataRequerida = false)]
        [Display(Name = "Data Inicial")]
        public DateTime LowerDate { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataBrasil(ErrorMessage = "Data inválida.", DataRequerida = false)]
        [Display(Name = "Data Final")]
        public DateTime UpperDate { get; set; }

       // public List<SettlementDetail> Details { get; set; }

        [JsonIgnore]
        public virtual Commission Commission { get; set; }

    }
}