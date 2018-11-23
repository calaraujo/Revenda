using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class SalesDetail
    {
        [Key]
        public int SaleDetailId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public int SaleId { get; set; }

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
        public decimal Price { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = false)]
        [Range(0, double.MaxValue, ErrorMessage = "Você precisar informar valor maior do que {1} em {0}")]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Quantidade")]
        public  decimal Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Valor")]
        public decimal? Value { get { return Price * (decimal)Quantity; } }

        [JsonIgnore]
        public virtual Sale Sale { get; set; }

        [JsonIgnore]
        public virtual Product Product { get; set; }

    }
}