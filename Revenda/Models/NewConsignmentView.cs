using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Revenda.Models
{
    public class NewConsignmentView
    {
        public int ConsignmentId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Vendedor")]
        public int SellerId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Destino")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //[DataBrasil(ErrorMessage = "Data inválida", DataRequerida = false)]
        [Display(Name = "Data")]
        public DateTime Data { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[DataBrasil(ErrorMessage = "Data Inválida", DataRequerida = false)]
        [Display(Name = "Data Acerto")]
        public Nullable<DateTime> HitDate { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Pagto.")]
        public int ConditionId { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Coment.")]
        public string Remarks { get; set; }

        public List<ConsignmentsDetailTmp> Details { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        public decimal TotalQuantity { get { return Details == null ? 0 : Details.Sum(d => d.Quantity); } }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        public decimal TotalValue { get { return Details == null ? 0 : Details.Sum(d => d.Value); } }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Qtde Venda")]
        public decimal? TotalSaleQuantity { get { return Details == null ? 0 : Details.Sum(d => d.SaleQuantity); } }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Valor Venda")]
        public decimal? TotalSaleValue { get { return Details == null ? 0 : Details.Sum(d => d.SaleValue); } }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Qtde Retorno")]
        public decimal? TotalReturnQuantity { get { return Details == null ? 0 : Details.Sum(d => d.ReturnQuantity); } }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Valor Retorno")]
        public decimal? TotalReturnValue { get { return Details == null ? 0 : Details.Sum(d => d.ReturnValue); } }

        [JsonIgnore]
        public virtual Condition Condition { get; set; }

        [JsonIgnore]
        public virtual Seller Seller { get; set; }

        [JsonIgnore]
        public virtual Warehouse Warehouse { get; set; }

    }
}