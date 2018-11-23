using Revenda.Models;

namespace Revenda.Classes
{
    public class SaleDetailApi
    {
        public int SaleDetailId { get; set; }

        public int SaleId { get; set; }

        public int ProductId { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }

        public virtual Product Product { get; set; }
    }
}