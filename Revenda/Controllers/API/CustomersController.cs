using Revenda.Classes;
using Revenda.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace Revenda.Controllers.API
{
    [RoutePrefix("api/Customers")]
    public class CustomersController : ApiController
    {
        private RevendaContext db = new RevendaContext();

        [HttpPost]
        [Route("SetPhoto")]
        public IHttpActionResult SetPhoto(PhotoRequest photoRequest)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var customer = db.Customers.Find(photoRequest.Id);
            if (customer == null)
            {
                return BadRequest("Cliente não Existe.");
            }

            var stream = new MemoryStream(photoRequest.Array);

            var file = string.Format("{0}.jpg", photoRequest.Id);
            var folder = "~/Content/Customers";
            var fullPath = string.Format("{0}/{1}", folder, file);
            var response = FileHelper.UploadPhotoApp(stream, folder, file);

            if (!response)
            {
                return BadRequest("Foto não pode ser carregada.");
            }

            customer.Photo = fullPath;
            db.Entry(customer).State = EntityState.Modified;
            var response2 = DBHelper.SaveChanges(db);
            if (!response2.Succeeded)
            {
                return BadRequest(response2.Message);
            }

            return Ok("ok");
        }

        // GET: api/Customers
        public IHttpActionResult GetCustomers()
        {
            //Propiedad necesaria proxy para que las propiedades virtuales funcionen
            //db.Configuration.ProxyCreationEnabled = false;
            var customers = db.Customers.OrderBy(c => c.FirstName).ThenBy(c => c.LastName)
                .ToList();
            var customersResponse = new List<CustomerResponse>();
            double Lat;
            double Long;
            foreach (var customer in customers)
            {
                if (customer.Latitude.HasValue)
                {
                    Lat = customer.Latitude.Value;
                }
                else Lat = 0;
                if (customer.Longitude.HasValue)
                {
                    Long = customer.Longitude.Value;
                }
                else Long = 0;

                var customerResponse = new CustomerResponse
                {
                    Address = customer.Address,
                    City = customer.City,
                    CityId = customer.CityId,
                    CustomerId = customer.CustomerId,
                    EstadoId = customer.EstadoId,
                    Estado = customer.Estado,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Latitude = Lat,
                    Longitude = Long,
                    Orders = customer.Orders,
                    Phone = customer.Phone,
                    UserName = customer.UserName,
                    Sales = customer.Sales,
                    BirthDate = customer.BirthDate,
                    Complement = customer.Complement,
                    Neighborhood = customer.Neighborhood,
                    Photo = customer.Photo,
                    SocialNumber = customer.SocialNumber,
                    ZipCode = customer.ZipCode,
                };
                customersResponse.Add(customerResponse);
            }

            return Ok(customersResponse);
        }

        // GET: api/Customers/5
        [ResponseType(typeof(Customer))]
        public IHttpActionResult GetCustomer(int id)
        {
            //Propiedad necesaria proxy para que las propiedades virtuales funcionen
            db.Configuration.ProxyCreationEnabled = false;
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT: api/Customers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCustomer(int id, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Customers
        [ResponseType(typeof(Customer))]
        public IHttpActionResult PostCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Customers.Add(customer);
            db.SaveChanges();

            UsersHelper.CreateUserASP(customer.UserName, "Customer");

            return CreatedAtRoute("DefaultApi", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customers/5
        [ResponseType(typeof(Customer))]
        public IHttpActionResult DeleteCustomer(int id)
        {
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            db.Customers.Remove(customer);
            db.SaveChanges();

            return Ok(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(int id)
        {
            return db.Customers.Count(e => e.CustomerId == id) > 0;
        }
    }
}