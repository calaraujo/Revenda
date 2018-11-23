using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class AccountSubClass
    {
        [Key]
        public int AccountSubClassId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [Display(Name= "Grupo")]
        public int AccountClassId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Nome Sub-Grupo")]
        [Index("AccountSubClass_Name_Index", IsUnique = true)]
        public string SubGroupName { get; set; }

        [JsonIgnore]
        public virtual AccountClass AccountClasses { get; set; }

        [JsonIgnore]
        public virtual ICollection<Account> Accounts { get; set; }
    }
}