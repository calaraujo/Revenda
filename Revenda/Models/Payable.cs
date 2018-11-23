using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class Payable
    {
        [Key]
        public int PayableId { get; set; }

        public int PurchaseId { get; set; }

        //public int? SettlementId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Fornecedor")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataBrasil(ErrorMessage = "Data Inválida", DataRequerida = false)]
        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        public int ConditionId { get; set; }

        public string Status { get; set; }

        public string Payment { get; set; }

        [JsonIgnore]
        public virtual Purchase Purchase { get; set; }

        [JsonIgnore]
        public virtual Supplier Supplier { get; set; }

        [JsonIgnore]
        public virtual Condition Condition { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<PayableDetail> PayableDetails { get; set; }

        [JsonIgnore]
        public virtual ICollection<SettlementPayable> SettlementPayables { get; set; }

        [JsonIgnore]
        public virtual Settlement Settlement { get; set; }

    }
}