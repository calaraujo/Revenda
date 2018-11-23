using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Revenda.Models
{
    public class Warehouse
    {
        [Key]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Index("Warehouse_CompanyId_Name_Index", 1, IsUnique = true)]
        [Display(Name = "Empresa")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Nome")]
        [Index("Warehouse_CompanyId_Name_Index", 2, IsUnique = true)]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual Company Company { get; set; }

        [JsonIgnore]
        public virtual ICollection<Inventory> Inventories { get; set; }

        [JsonIgnore]
        public virtual ICollection<Purchase> Purchases { get; set; }

        [JsonIgnore]
        public virtual ICollection<Sale> Sales { get; set; }

        [JsonIgnore]
        public virtual ICollection<Consignment> Consignments { get; set; }

        [JsonIgnore]
        public virtual ICollection<StockLedger> StockLedgers { get; set; }

    }
}