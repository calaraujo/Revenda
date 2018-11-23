using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Revenda.Classes;
using Revenda.Models;

namespace Revenda.Controllers.API
{
    [RoutePrefix("api/Cities")]
    public class CitiesController : ApiController
    {
        private RevendaContext db = new RevendaContext();

        // GET: api/Cities
        public IHttpActionResult GetCities()
        {
            var cities = db.Cities.ToList();
            var citiesResponse = new List<CityResponse>();
            //Recorremos la consulta 
            foreach (var city in cities)
            {
                var cityResponse = new CityResponse
                {
                    CityId = city.CityId,
                    Customers = city.Customers,
                    Estado = city.Estado,
                    Name = city.Name,
                    Sellers = city.Sellers,
                    Suppliers = city.Suppliers,
                };

                citiesResponse.Add(cityResponse);
            }
            return Ok(citiesResponse);
        }

        // GET: api/Cities/5
        [ResponseType(typeof(City))]
        public async Task<IHttpActionResult> GetCity(int id)
        {
            City city = await db.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            return Ok(city);
        }

        // PUT: api/Cities/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCity(int id, City city)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != city.CityId)
            {
                return BadRequest();
            }

            db.Entry(city).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
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

        // POST: api/Cities
        [ResponseType(typeof(City))]
        public async Task<IHttpActionResult> PostCity(City city)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Cities.Add(city);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = city.CityId }, city);
        }

        // DELETE: api/Cities/5
        [ResponseType(typeof(City))]
        public async Task<IHttpActionResult> DeleteCity(int id)
        {
            City city = await db.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            db.Cities.Remove(city);
            await db.SaveChangesAsync();

            return Ok(city);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CityExists(int id)
        {
            return db.Cities.Count(e => e.CityId == id) > 0;
        }
    }
}