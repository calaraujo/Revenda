using Revenda.Models;
using System;
using System.Collections.Generic;

namespace Revenda.Classes
{
    public class SalesApi
    {
        public int SaleId { get; set; }
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime Date { get; set; }
        public int ConditionId { get; set; }
        public int SellerId { get; set; }
        public string Remarks { get; set; }
        public Customer Customer { get; set; }
        public Company Company { get; set; }
        public Condition Condition { get; set; }
        public Seller Seller { get; set; }
        public Warehouse Warehouse { get; set; }
        public string Status { get; set; }
        public List<SaleDetailApi> SaleDetails { get; set; }
    }
}