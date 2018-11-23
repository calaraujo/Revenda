using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class Movement
    {
        [Key]
        [Display(Name = "Item")]
        public int MovementId { get; set; }

        [Display(Name = "Conta")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [Display(Name ="Identificador")]
        public int ParameterId { get; set; }

        [Display(Name ="Previsto/Realizado")]
        public TypeOfStatement StatementType { get;  set;  }

        [Display(Name = "Data")]
        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataBrasil(ErrorMessage = "Data Inválida", DataRequerida = false)]
        public DateTime Data { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Valor")]
        public decimal Value { get; set; }

        [JsonIgnore]
        public virtual Parameter Parameter { get; set; }

        [JsonIgnore]
        public virtual Account Account { get; set; }

        public enum TypeOfStatement
        {
            [Display(Name = "Previsto")]
            Previsto,  
            [Display(Name = "Realizado")]
            Realizado,    
        }
    }
}