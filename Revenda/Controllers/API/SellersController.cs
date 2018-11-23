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
using Revenda.Models;

namespace Revenda.Controllers.API
{
    public class SellersController : ApiController
    {
        private RevendaContext db = new RevendaContext();

        // GET: api/Sellers
        public IQueryable<Seller> GetSellers()
        {
            return db.Sellers.Include(se => se.Orders);
        }

        // GET: api/Sellers/5
        [ResponseType(typeof(Seller))]
        public async Task<IHttpActionResult> GetSeller(int id)
        {
            Seller seller = await db.Sellers.FindAsync(id);
            if (seller == null)
            {
                return NotFound();
            }

            return Ok(seller);
        }

        // PUT: api/Sellers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSeller(int id, Seller seller)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != seller.SellerId)
            {
                return BadRequest();
            }

            db.Entry(seller).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SellerExists(id))
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

        // POST: api/Sellers
        [ResponseType(typeof(Seller))]
        public async Task<IHttpActionResult> PostSeller(Seller seller)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Sellers.Add(seller);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = seller.SellerId }, seller);
        }

        // DELETE: api/Sellers/5
        [ResponseType(typeof(Seller))]
        public async Task<IHttpActionResult> DeleteSeller(int id)
        {
            Seller seller = await db.Sellers.FindAsync(id);
            if (seller == null)
            {
                return NotFound();
            }

            db.Sellers.Remove(seller);
            await db.SaveChangesAsync();

            return Ok(seller);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SellerExists(int id)
        {
            return db.Sellers.Count(e => e.SellerId == id) > 0;
        }
    }
}