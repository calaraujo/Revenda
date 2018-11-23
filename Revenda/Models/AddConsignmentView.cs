using System.ComponentModel.DataAnnotations;

namespace Revenda.Models
{
    public class AddConsignmentView
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Produto", Prompt = "[Selecione um produto ...]")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Range(0, double.MaxValue, ErrorMessage = "Você precisar informar valor maior do que {1} em {0}")]
        [Display(Name = "Quantidade", Prompt = "[Informe a quantidade ...]")]
        public decimal Quantity { get; set; }

    }
}