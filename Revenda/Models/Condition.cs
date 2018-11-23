using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Revenda.Models
{
    public class Condition
    {
        [Key]
        public int ConditionId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Cond.Pagamento")]
        [Index("Condition_Description_Index", IsUnique = true)]
        public string Description { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [Display(Name = "Número de Dias")]        
        public int Interval { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [Display(Name = "Número de Parcelas")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [Display(Name = "Cond.Fornecedor")]
        public bool SupplierCondition { get; set; }

        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; }

        [JsonIgnore]
        public virtual ICollection<Purchase> Purchases { get; set; }

        [JsonIgnore]
        public virtual ICollection<Sale> Sales { get; set; }

        [JsonIgnore]
        public virtual ICollection<Receivable> Receivables { get; set; }

        [JsonIgnore]
        public virtual ICollection<Payable> Payables { get; set; }

    }
}