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
    public class WarehousesController : ApiController
    {
        private RevendaContext db = new RevendaContext();

        // GET: api/Warehouses
        public IQueryable<Warehouse> GetWarehouses()
        {
            return db.Warehouses.Include(w => w.Inventories).Include(w => w.Sales);
        }

        // GET: api/Warehouses/5
        [ResponseType(typeof(Warehouse))]
        public async Task<IHttpActionResult> GetWarehouse(int id)
        {
            Warehouse warehouse = await db.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return Ok(warehouse);
        }

        // PUT: api/Warehouses/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutWarehouse(int id, Warehouse warehouse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != warehouse.WarehouseId)
            {
                return BadRequest();
            }

            db.Entry(warehouse).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseExists(id))
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

        // POST: api/Warehouses
        [ResponseType(typeof(Warehouse))]
        public async Task<IHttpActionResult> PostWarehouse(Warehouse warehouse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Warehouses.Add(warehouse);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = warehouse.WarehouseId }, warehouse);
        }

        // DELETE: api/Warehouses/5
        [ResponseType(typeof(Warehouse))]
        public async Task<IHttpActionResult> DeleteWarehouse(int id)
        {
            Warehouse warehouse = await db.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            db.Warehouses.Remove(warehouse);
            await db.SaveChangesAsync();

            return Ok(warehouse);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WarehouseExists(int id)
        {
            return db.Warehouses.Count(e => e.WarehouseId == id) > 0;
        }
    }
}