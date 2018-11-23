using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class PurchaseDetailTmp
    {
        [Key]
        public int PurchaseDetailTmpId { get; set; }

        public int PurchaseId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(256, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Produto")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [Range(0, double.MaxValue, ErrorMessage = "Você precisar informar valor maior do que {1} em {0}")]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Preço")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = false)]
        [Range(0, double.MaxValue, ErrorMessage = "Você precisar informar valor maior do que {1} em {0}")]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Quantidade")]
        public decimal Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Valor")]
        public decimal Value { get { return Cost * (decimal)Quantity; } }

        [JsonIgnore]
        public virtual Product Product { get; set; }

    }
}