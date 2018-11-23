using Revenda.Models;

namespace Revenda.Classes
{
    public class InventoryResponse
    {
        public int InventoryId { get; set; }
        public decimal Stock { get; set; }
        public Warehouse Warehouse { get; set; }
        public Product Product { get; set; }
    }
}