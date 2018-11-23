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
    public class ConditionsController : ApiController
    {
        private RevendaContext db = new RevendaContext();

        // GET: api/Conditions
        public IHttpActionResult GetConditions()
        {
            var conditions = db.Conditions.ToList();
            var conditionsResponse = new List<ConditionResponse>();

            foreach (var condition in conditions)
            {
                var conditionResponse = new ConditionResponse
                {
                    ConditionId = condition.ConditionId,
                    Description = condition.Description,
                    Interval = condition.Interval,
                    Quantity = condition.Quantity,
                    SupplierCondition = condition.SupplierCondition,
                    Orders = condition.Orders.ToList(),
                    Purchases = condition.Purchases.ToList(),
                    Sales = condition.Sales.ToList(),

                };

                conditionsResponse.Add(conditionResponse);
            }
            return Ok(conditionsResponse);
        }

        // GET: api/Conditions/5
        [ResponseType(typeof(Condition))]
        public async Task<IHttpActionResult> GetCondition(int id)
        {
            Condition condition = await db.Conditions.FindAsync(id);
            if (condition == null)
            {
                return NotFound();
            }

            return Ok(condition);
        }

        // PUT: api/Conditions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCondition(int id, Condition condition)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != condition.ConditionId)
            {
                return BadRequest();
            }

            db.Entry(condition).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConditionExists(id))
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

        // POST: api/Conditions
        [ResponseType(typeof(Condition))]
        public async Task<IHttpActionResult> PostCondition(Condition condition)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Conditions.Add(condition);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = condition.ConditionId }, condition);
        }

        // DELETE: api/Conditions/5
        [ResponseType(typeof(Condition))]
        public async Task<IHttpActionResult> DeleteCondition(int id)
        {
            Condition condition = await db.Conditions.FindAsync(id);
            if (condition == null)
            {
                return NotFound();
            }

            db.Conditions.Remove(condition);
            await db.SaveChangesAsync();

            return Ok(condition);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ConditionExists(int id)
        {
            return db.Conditions.Count(e => e.ConditionId == id) > 0;
        }
    }
}