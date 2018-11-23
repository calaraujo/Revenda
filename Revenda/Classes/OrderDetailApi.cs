using Revenda.Models;

namespace Revenda.Classes
{
    public class OrderDetailApi
    {
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }

        public virtual Product Product { get; set; }
    }
}
