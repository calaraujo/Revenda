﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Revenda.Models
{
    public class OrderDetailTmp
    {
        [Key]
        public int OrderDetailTmpId { get; set; }

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
        [Range(0, double.MaxValue, ErrorMessage = "Você deve informar valores em {0} entre {1} e {2}")]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Preço")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = false)]
        [Range(0, double.MaxValue, ErrorMessage = "Você deve informar valores em {0} entre {1} e {2}")]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Quantidade")]
        public decimal Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        public decimal Value { get { return Price * Quantity; } }

        [JsonIgnore]
        public virtual Product Product { get; set; }

    }
}