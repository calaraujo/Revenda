using Revenda.Models;
using System.Collections.Generic;

namespace Revenda.Classes
{
    public class ConditionsApi
    {

        public int ConditionId { get; set; }

        public string Description { get; set; }

        public int Interval { get; set; }

        public int Quantity { get; set; }

        public bool SupplierCondition { get; set; }

        public List<Order> Orders { get; set; }

        //public List<Purchase> Purchases { get; set; }

        //public List<Sale> Sales { get; set; }

        //public List<Receivable> Receivables { get; set; }

        //public List<Payable> Payables { get; set; }
    }
}