using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class PurchaseDetail
    {
        [Key]
        public int PurchaseDetailId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public int PurchaseId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Produto")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [Range(0, double.MaxValue, ErrorMessage = "Você deve informar valores em {0} entre {1} e {2}")]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Preço")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = false)]
        [Range(0, double.MaxValue, ErrorMessage = "Você deve informar valores em {0} entre {1} e {2}")]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Quantidade")]
        public decimal Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Valor")]
        public decimal Value { get { return Cost * (decimal)Quantity; } }

        [JsonIgnore]
        public virtual Purchase Purchase { get; set; }

        [JsonIgnore]
        public virtual Product Product { get; set; }

    }
}