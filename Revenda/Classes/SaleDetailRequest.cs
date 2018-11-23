using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Revenda.Classes
{
    public class SaleDetailRequest
    {
        public int ProductId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string UserName { get; set; }
    }
}