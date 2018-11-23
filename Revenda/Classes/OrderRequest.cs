using System;
using System.Collections.Generic;

namespace Revenda.Classes
{
    public class OrderRequest
    {
        public string UserName { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public int ConditionId { get; set; }
        public int SellerId { get; set; }
        public string Remarks { get; set; }
        public List<OrderDetailRequest> Details { get; set; }
    }
}