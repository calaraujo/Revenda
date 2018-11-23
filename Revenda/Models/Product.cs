using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Empresa")]
        [Index("Product_CampanyId_Description_Index", 1, IsUnique = true)]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Código")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Produto")]
        [Index("Product_CampanyId_Description_Index", 2, IsUnique = true)]
        public string Description { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Categoria")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(ErrorMessage = "Valor Inválido.", DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Custo")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(ErrorMessage = "Valor Inválido.", DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Preço")]
        public decimal Price { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Foto")]
        public string Image { get; set; }

        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Estoque")]
        public decimal Stock { get { return Inventories == null ? 0 : Inventories.Sum(i => i.Stock); } }
                                    
        [JsonIgnore]
        public virtual Company Company { get; set; }

        [JsonIgnore]
        public virtual Category Category { get; set; }

        [JsonIgnore]
        public virtual ICollection<Inventory> Inventories { get; set; }

        [JsonIgnore]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [JsonIgnore]
        public virtual ICollection<OrderDetailTmp> OrderDetailTmps { get; set; }

        [JsonIgnore]
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }

        [JsonIgnore]
        public virtual ICollection<PurchaseDetailTmp> PurchaseDetailTmps { get; set; }

        [JsonIgnore]
        public virtual ICollection<SalesDetail> SalesDetails { get; set; }

        [JsonIgnore]
        public virtual ICollection<SalesDetailTmp> SalesDetailTmps { get; set; }

        [JsonIgnore]
        public virtual ICollection<StockLedger> StockLedgers { get; set; }

    }
}