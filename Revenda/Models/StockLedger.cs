using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Revenda.Models
{
    public class StockLedger
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Cód.Produto")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Mostruário")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Produto")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Desc.Produto")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Desc.Mostruário")]
        public string WarehouseDescription { get; set; }

        [Display(Name = "Tipo Movto.")]
        public TypeOfMovement TypeMoviment { get; set; }

        [Display(Name = "Tipo Docto.")]
        public TypeOfDocument DocumentType { get; set; }

        [Display(Name = "Cód.Docto")]
        public int DocumentNumber { get; set; }

        [MaxLength(100, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Detalhes")]
        public string DetailDescription { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Valor Anterior")]
        public decimal TotalBefore { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Valor Movto.")]
        public decimal Amount { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Valor Posterior")]
        public decimal TotalAfter { get; set; }

        public enum TypeOfMovement
        {
            [Display(Name = "Entrada")]
            Entrada,
            [Display(Name = "Saída")]
            Saída,
        }

        public enum TypeOfDocument
        {
            [Display(Name = "Compra")]
            COMP,
            [Display(Name = "Venda")]
            VEND,
            [Display(Name = "Dev.Compra")]
            DEVC,
            [Display(Name = "Dev.Venda")]
            DEVV,
            [Display(Name = "Consignação")]
            CONS,
            [Display(Name = "Retorno Consignação")]
            RETC,
            [Display(Name = "Cancela Consignação")]
            CANC,
            [Display(Name = "Atualiza Consignação")]
            UPDC,
            [Display(Name = "Ajuste Estoque")]
            AJES,
            [Display(Name = "Entrada Consignação Destino")]
            CONE,
        }

        [JsonIgnore]
        public virtual Product Product{ get; set; }

        [JsonIgnore]
        public virtual Warehouse Warehouse { get; set; }
    }
}
