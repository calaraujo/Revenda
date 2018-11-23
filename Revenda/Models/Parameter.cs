using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class Parameter
    {
        [Key]
        public int ParameterId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [MaxLength(4, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Identificador")]
        [Index("Parameter_Identity_Index", IsUnique = true)]
        public string Identity { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [MaxLength(255, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Desc.Parâmetro")]
        [Index("Parameter_Description_Index", IsUnique = true)]
        public string Description { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(10, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Valor")]
        public string Value { get; set; }

        [Display(Name = "Fluxo de Caixa")]
        public bool CashFlow { get; set; }

        [JsonIgnore]
        public virtual ICollection<Movement> Movements { get; set; }
    }
}