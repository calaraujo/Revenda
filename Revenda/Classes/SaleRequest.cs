using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Revenda.Classes
{
    public class SaleRequest
    {
        public string UserName { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime Date { get; set; }
        public int ConditionId { get; set; }
        public int SellerId { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public List<SaleDetailRequest> SaleDetails { get; set; }
    }
}