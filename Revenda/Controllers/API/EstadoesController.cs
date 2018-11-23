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
    [RoutePrefix("api/Estadoes")]
    public class EstadoesController : ApiController
    {
        private RevendaContext db = new RevendaContext();

        // GET: api/Estadoes
        public IHttpActionResult GetEstadoes()
        {
            var estados = db.Estadoes.ToList();
            var estadosResponse = new List<EstadoResponse>();
            foreach (var estado in estados)
            {
                var estadoResponse = new EstadoResponse
                {
                    Cities = estado.Cities,
                    Customers = estado.Customers,
                    EstadoId = estado.EstadoId,
                    Name = estado.Name,
                    Sellers = estado.Sellers,
                    Suppliers   = estado.Suppliers,
                };

                estadosResponse.Add(estadoResponse);
            }
            return Ok(estadosResponse);
        }

        // GET: api/Estadoes/5
        [ResponseType(typeof(Estado))]
        public async Task<IHttpActionResult> GetEstado(int id)
        {
            Estado estado = await db.Estadoes.FindAsync(id);
            if (estado == null)
            {
                return NotFound();
            }

            return Ok(estado);
        }

        // PUT: api/Estadoes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutEstado(int id, Estado estado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != estado.EstadoId)
            {
                return BadRequest();
            }

            db.Entry(estado).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EstadoExists(id))
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

        // POST: api/Estadoes
        [ResponseType(typeof(Estado))]
        public async Task<IHttpActionResult> PostEstado(Estado estado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Estadoes.Add(estado);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = estado.EstadoId }, estado);
        }

        // DELETE: api/Estadoes/5
        [ResponseType(typeof(Estado))]
        public async Task<IHttpActionResult> DeleteEstado(int id)
        {
            Estado estado = await db.Estadoes.FindAsync(id);
            if (estado == null)
            {
                return NotFound();
            }

            db.Estadoes.Remove(estado);
            await db.SaveChangesAsync();

            return Ok(estado);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EstadoExists(int id)
        {
            return db.Estadoes.Count(e => e.EstadoId == id) > 0;
        }
    }
}