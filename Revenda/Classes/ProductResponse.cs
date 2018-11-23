using Revenda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Revenda.Classes
{
    public class ProductResponse
    {
        public int ProductId { get; set; }

        public string Description { get; set; }

        public string ProductCode { get; set; }

        public decimal Cost { get; set; }

        public decimal Price { get; set; }

        public string Image { get; set; }

        public string Remarks { get; set; }

        public decimal Stock { get; set; }

        public List<InventoryResponse> Inventories { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

    }
}