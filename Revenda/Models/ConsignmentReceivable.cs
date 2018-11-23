using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Revenda.Models
{
    public class ConsignmentReceivable
    {
        [Key]
        public int ConsignmentReceivableId { get; set; }

        public int ConsignmentId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Vendedor")]
        public int SellerId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataBrasil(ErrorMessage = "Data Inválida", DataRequerida = false)]
        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        public int ConditionId { get; set; }

        public string Status { get; set; }

        public string Payment { get; set; }

        [JsonIgnore]
        public virtual Consignment Sale { get; set; }

        [JsonIgnore]
        public virtual Seller Seller { get; set; }

        [JsonIgnore]
        public virtual Condition Condition { get; set; }

        [JsonIgnore]
        public virtual ICollection<ConsignmentReceivableDetail> ConsignmentReceivableDetails { get; set; }
    }
}