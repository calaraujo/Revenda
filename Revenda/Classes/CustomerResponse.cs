using Revenda.Models;
using System.Collections.Generic;

namespace Revenda.Classes
{
    public class CustomerResponse
    {
        public int CustomerId { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string BirthDate { get; set; }

        public string SocialNumber { get; set; }

        public string Phone { get; set; }

        public string Photo { get; set; }

        public string Address { get; set; }

        public string Complement { get; set; }

        public string Neighborhood { get; set; }

        public string ZipCode { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int EstadoId { get; set; }

        public int CityId { get; set; }

        public Estado Estado { get; set; }

        public City City { get; set; }

        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

        public string PhotoFullPath
        {
            get
            {
                return Photo == null ? string.Empty : string.Format("http://cadearaujo.com/{0}", Photo.Substring(2));
            }
        }

        public ICollection<Order> Orders { get; set; }

        public ICollection<Sale> Sales { get; set; }

    }
}