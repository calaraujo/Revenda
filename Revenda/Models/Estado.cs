using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Revenda.Models
{
    public class Estado
    {
        [Key]
        public int EstadoId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Nome Estado")]
        [Index("Estado_Name_Index", IsUnique = true)]
        public string Name { get; set; }

        // Devemos criar este atributo esta propriedade virtual para representar o lado N de uma relação 1:N
        [JsonIgnore]
        public virtual ICollection<City> Cities { get; set; }

        [JsonIgnore]
        public virtual ICollection<Company> Companies { get; set; }

        [JsonIgnore]
        public virtual ICollection<Supplier> Suppliers { get; set; }

        [JsonIgnore]
        public virtual ICollection<Customer> Customers { get; set; }

        [JsonIgnore]
        public virtual ICollection<Seller> Sellers { get; set; }

    }
}