using Revenda.Models;
using System.Collections.Generic;

namespace Revenda.Classes
{
    public class EstadoResponse
    {
        public int EstadoId { get; set; }
        public string Name { get; set; }
        public ICollection<City> Cities { get; set; }
        public ICollection<Customer> Customers { get; set; }
        public ICollection<Seller> Sellers { get; set; }
        public ICollection<Supplier> Suppliers { get; set; }
    }
}