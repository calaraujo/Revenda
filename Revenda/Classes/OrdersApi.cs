using Revenda.Models;
using System;
using System.Collections.Generic;

namespace Revenda.Classes
{
    public class OrdersApi
    {
        public int OrderId { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public int ConditionId { get; set; }
        public int SellerId { get; set; }
        public string Remarks { get; set; }
        public Customer Customer { get; set; }         
        public Company Company { get; set; }
        public Condition Condition { get; set; }
        public Seller Seller { get; set; }
        public List<OrderDetailApi> OrderDetails { get; set; }
    }
}