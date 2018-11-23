using System.ComponentModel.DataAnnotations;

namespace Revenda.Models
{
    public class ProductOrCustomerViewModel
    {
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Display(Name = "Descrição ou Código")]
        public string TypeOrCountryOrAccount { get; set; }
        [Display(Name = "Categoria")]
        public string Type { get; set; }
    }
}