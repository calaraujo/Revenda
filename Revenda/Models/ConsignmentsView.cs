using System;
using System.ComponentModel.DataAnnotations;

namespace Revenda.Models
{
    public class ConsignmentsView
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Código")]
        public int ConsignmentId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataBrasil(ErrorMessage = "Data Inválida", DataRequerida = false)]
        [Display(Name = "Data")]
        public DateTime Data { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Valor Total")]
        public decimal? TotalValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Qtde.Total")]
        public decimal? TotalQuantity { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Item")]
        public int ConsignmentsDetailId { get; set; }

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
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Cond.Pagto.")]
        public int ConditionId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Código")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Nome")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Sobrenome")]
        public string LastName { get; set; }
    }
}