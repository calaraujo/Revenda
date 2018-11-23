using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class ReturnConsignment
    {
        [DisplayName("Item")]
        public int ConsignmentDetailId { get; set; }
        [DisplayName("Qtde.Retorno")]
        public decimal ReturnQuantity { get; set; }
    }
}