﻿using Newtonsoft.Json;
using Revenda.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class Purchase
    {
        public Purchase()
        {
            this.PurchaseDetails = new List<PurchaseDetail>();
        }

        [Key]
        [Display(Name = "Ped.Compra")]
        public int PurchaseId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Empresa")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Fornecedor")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Mostruário")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //[DataBrasil(ErrorMessage = "Data Inválida", DataRequerida = false)]
        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Cond.Pgto.")]
        public int ConditionId { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentários")]
        public string Remarks { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Custo Total")]
        public Nullable<decimal> TotalCost { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Qtde.Total")]
        public Nullable<decimal> TotalQuantity { get; set; }

        public string Status { get; set; }
        
        [JsonIgnore]
        public virtual Supplier Supplier { get; set; }

        [JsonIgnore]
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }

        [JsonIgnore]
        public virtual Warehouse Warehouse { get; set; }

        [JsonIgnore]
        public virtual Company Company { get; set; }

        [JsonIgnore]
        public virtual Condition Condition { get; set; }

        [JsonIgnore]
        public virtual ICollection<Settlement> Settlements { get; set; }

        [JsonIgnore]
        public virtual ICollection<Payable> Payables { get; set; }

    }
}