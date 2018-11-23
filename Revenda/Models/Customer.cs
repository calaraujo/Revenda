using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Revenda.Models
{
    public class Customer 
    {
        [Key]
        public int CustomerId { get; set; }

        //[Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(256, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "E-Mail")]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Index("Customer_FullName_Index", 1, IsUnique = true)]
        [Display(Name = "Nome")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Index("Customer_FullName_Index", 2, IsUnique = true)]
        [Display(Name = "Sobrenome")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[DataBrasil(ErrorMessage = "Data inválida.", DataRequerida = false)]
        [Display(Name = "Data Nascimento")]
        public string BirthDate { get; set; }

        //[Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        //[Index("Customer_CPF_Index", IsUnique = true)]
        [Display(Name = "C.P.F.")]
        public string SocialNumber { get; set; }

        //[Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(20, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Fone")]
        public string Phone { get; set; }

        //[Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Endereço")]
        public string Address { get; set; }

        //[Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Complemento")]
        public string Complement { get; set; }

        //[Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "Bairro")]
        public string Neighborhood { get; set; }

        //[Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MaxLength(9, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres.")]
        [Display(Name = "C.E.P")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Estado")]
        public int EstadoId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, double.MaxValue, ErrorMessage = "Você precisa selecionar o(a) {0}")]
        [Display(Name = "Cidade")]
        public int CityId { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Foto")]
        public string Photo { get; set; }

        [Display(Name = "Cliente")]
        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }

        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }

        [NotMapped]
        [Display(Name = "Foto")]
        public HttpPostedFileBase PhotoFile { get; set; }

        [JsonIgnore]
        public virtual Estado Estado { get; set; }

        [JsonIgnore]
        public virtual City City { get; set; }

        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; }

        [JsonIgnore]
        public virtual ICollection<Sale> Sales { get; set; }

        [JsonIgnore]
        public virtual ICollection<Receivable> Receivables { get; set; }
    }
}