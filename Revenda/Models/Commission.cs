using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class Commission
    {
        [Key]
        public int CommissionId { get; set; }
        
        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [MaxLength(255, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Descrição")]
        [Index("Commission_Description_Index", IsUnique = true)]
        public string Description { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
       // [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Limite Inferior")]
        public decimal LowerLimit { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
       // [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Limite Superior")]
        public decimal UpperLimit { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(0, 1, ErrorMessage = "O {0} precisa estar entre {1} e {2}")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "% Comissão")]
        public decimal Percent { get; set; }

        [JsonIgnore]
        public virtual ICollection<Settlement> Settlements { get; set; }
    }
}