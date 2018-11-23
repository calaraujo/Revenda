using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class AddProductPurchase
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Produto", Prompt = "[Selecione um Produto ...]")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Range(0, double.MaxValue, ErrorMessage = "Você precisar informar valor maior do que {1} em {0}")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Range(0, double.MaxValue, ErrorMessage = "Você precisar informar valor maior do que {1} em {0}")]
        [Display(Name = "Quantidade", Prompt = "[Informe a quantidade ...]")]
        public decimal Quantity { get; set; }

    }
}