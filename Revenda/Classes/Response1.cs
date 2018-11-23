using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Revenda.Classes
{
    public class Response1
    {
        public bool Succeeded { get; set; }

        public string Message { get; set; }

        public int SettlementId { get; set; }

        public DateTime LowerDate { get; set; }

        public DateTime UpperDate { get; set; }

        public int CommissionId { get; set; }
    }
}