using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class Account
    {
        [Key]
        [Display(Name = "Conta")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [Display(Name = "Grupo")]
        [Index("Account_Class_SubClass_AccountCode_Index", 1, IsUnique = true)]
        public int AccountClassId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [Display(Name = "Sub-Grupo")]
        [Index("Account_Class_SubClass_AccountCode_Index", 2, IsUnique = true)]
        public int AccountSubClassId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [MaxLength(20, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Código Conta")]
        [Index("Account_Class_SubClass_AccountCode_Index", 3, IsUnique = true)]
        [Index("Account_AccountCode", IsUnique = true)]
        public string AccountCode { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório..")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Nome Conta")]
        public string AccountName { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Saldo")]
        public decimal Balance { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Débito")]
        public decimal DebitBalance { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Crédito")]
        public decimal CreditBalance { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Saldo Total")]
        public decimal TotalBalance { get { return GetTotalBalance(); } }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Débito Total")]
        public decimal TotalDebitBalance { get { return GetTotalDebit(); } }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [NumeroBrasil(DecimalRequerido = true, PontoMilharPermitido = true)]
        [Display(Name = "Crédito Total")]
        public decimal TotalCreditBalance { get { return GetTotalCredit(); } }

        public IList<Account> ChildAccounts { get; set; }

        [JsonIgnore]
        public virtual AccountClass AccountClass { get; set; }

        [JsonIgnore]
        public virtual AccountSubClass AccountSubClass { get; set;  }

        [JsonIgnore]
        public virtual ICollection<Entry> Entries { get; set; }

        [JsonIgnore]
        public virtual ICollection<Movement> Movements { get; set; }

        public Account()
        {
            ChildAccounts = new List<Account>();
        }


        private decimal GetTotalBalance()
        {
            decimal sum = 0;
            ComputeBalance(ChildAccounts, ref sum);
            return sum;
        }

        private void ComputeBalance(IList<Account> accounts, ref decimal sum)
        {
            foreach (var account in accounts)
            {
                sum += account.Balance;
                if (account.ChildAccounts.Count > 0)
                {
                    ComputeBalance(account.ChildAccounts, ref sum);
                }
            }
        }

        private decimal GetTotalDebit()
        {
            decimal sum = 0;
            ComputeDebit(ChildAccounts, ref sum);
            return sum;
        }

        private void ComputeDebit(IList<Account> accounts, ref decimal sum)
        {
            foreach (var account in accounts)
            {
                sum += account.DebitBalance;

                if (account.ChildAccounts.Count > 0)
                {
                    ComputeDebit(account.ChildAccounts, ref sum);
                }
            }
        }

        private decimal GetTotalCredit()
        {
            decimal sum = 0;
            ComputeCredit(ChildAccounts, ref sum);
            return sum;
        }

        private void ComputeCredit(IList<Account> accounts, ref decimal sum)
        {
            foreach (var account in accounts)
            {
                sum += account.CreditBalance;

                if (account.ChildAccounts.Count > 0)
                {
                    ComputeCredit(account.ChildAccounts, ref sum);
                }
            }
        }

    }
}