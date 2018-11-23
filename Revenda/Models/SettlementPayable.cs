using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class SettlementPayable
    {
        [Key]
        public int Id { get; set; }

        public int SettlementId { get; set; }

        public int PayableId { get; set; }

        public Payable Payable { get; set; }

        public Settlement Settlement { get; set; }
    }
}