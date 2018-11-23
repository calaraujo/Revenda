using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Revenda.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Nome Cidade")]
        [Index("City_EstadoId_Name_Index", 2, IsUnique = true)]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Nome Estado")]
        [Index("City_EstadoId_Name_Index", 1, IsUnique = true)]
        public int EstadoId { get; set; }

        // Devemos criar este atributo esta propriedade virtual para representar o lado 1 de uma relação 1:N
        [JsonIgnore]
        public virtual Estado Estado { get; set; }

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