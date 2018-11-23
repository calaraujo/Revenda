using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Revenda.Classes
{
    public class PurchaseView1
    {
        [Required]
        public int Pedido { get; set; }
        [Required]
        public int Item { get; set; }
        [Required]
        public DateTime Data { get; set; }
        [Required]
        public string CondPagto { get; set; }
        [Required]
        public int CodProduto { get; set; }
        [Required]
        public string Descrição { get; set; }
        [Required]
        public decimal CustoU { get; set; }
        [Required]
        public decimal Qtde { get; set; }
        [Required]
        public decimal ItemValue { get; set; }
    }
}