using Revenda.Models;

namespace Revenda.Classes
{
    public class UserResponse
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Complement { get; set; }

        public string Neighborhood { get; set; }

        public int EstadoId { get; set; }

        public string EstadoName { get; set; }

        public int CityId { get; set; }

        public string CityName { get; set; }

        public string Photo { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

        public Company Company { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsUser { get; set; }

        public bool IsCustomer { get; set; }

        public bool IsSupplier { get; set; }

    }
}