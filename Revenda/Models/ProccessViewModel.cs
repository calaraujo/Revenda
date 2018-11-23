using System;
using System.ComponentModel.DataAnnotations;

namespace Revenda.Models
{
    public class ProccessViewModel
    {
        [Display(Name = "Código")]
        public int Code { get; set; }

        [Display(Name = "Parceiro")]
        public string Partner { get; set; }

        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Valor Total")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        public Decimal Value { get; set; }

        [Display(Name = "Qtde Total")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        public Decimal Quantity { get; set; }
    }
}