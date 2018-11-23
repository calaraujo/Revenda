using System;
using System.ComponentModel.DataAnnotations;

namespace Revenda.Models
{
    public class Receipt
    {
        [Display(Name = "Parcela")]
        public int ReceivableDetailsId { get; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataBrasil(ErrorMessage = "Data Inválida", DataRequerida = false)]
        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Valor")]
        public decimal Value { get; set; }
    }
}