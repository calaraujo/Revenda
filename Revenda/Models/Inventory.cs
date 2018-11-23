using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required]
        [Display(Name ="Mostruário")]
        public int WarehouseId { get; set; }

        [Required]
        [Display(Name = "Produto")]
        public int ProductId { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Quantidade")]
        public decimal Stock { get; set; }

        [JsonIgnore]
        public virtual Warehouse Warehouse { get; set; }

        [JsonIgnore]
        public virtual Product Product { get; set; }
    }
}