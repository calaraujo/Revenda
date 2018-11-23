namespace Revenda.Classes
{
    public class OrderDetailRequest
    {
        public int ProductId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string UserName { get; set; }
    }
}