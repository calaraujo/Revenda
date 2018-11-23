using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class AccountClass
    {
        [Key]
        public int AccountClassId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Nome Grupo")]
        [Index("AccountClass_Name_Index", IsUnique = true)]
        public string GroupName { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Balanço Normal")]
        public string NormalBalance { get; set; }

        [JsonIgnore]
        public virtual ICollection<AccountSubClass> AccountSubClasses { get; set; }

        [JsonIgnore]
        public virtual ICollection<Account> Accounts { get; set; }
    }
}