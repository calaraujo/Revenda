using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class Entry
    {
        [Key]
        public int EntryId { get; set; }

        [Display(Name = "Data")]
        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = false)]
        //[DataBrasil(ErrorMessage = "Data Inválida", DataRequerida = false)]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [Display(Name = "Conta")]
        public int AccountId { get; set; }

        [Display(Name = "Código Conta")]
        public string AccountCode { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Valor")]
        public Decimal Value { get; set; }

        [JsonIgnore]
        public virtual Account Account { get; set; }
    }
}