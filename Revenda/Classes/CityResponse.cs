using Revenda.Models;
using System.Collections.Generic;

namespace Revenda.Classes
{
    public class CityResponse
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public Estado Estado { get; set; }
        public ICollection<Customer> Customers { get; set; }
        public ICollection<Seller> Sellers { get; set; }
        public ICollection<Supplier> Suppliers { get; set; }
    }
}