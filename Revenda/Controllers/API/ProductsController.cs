using Revenda.Classes;
using Revenda.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Revenda.Controllers.API
{
    [RoutePrefix("api/Products")]
    public class ProductsController : ApiController
    {
        private RevendaContext db = new RevendaContext();

        // GET: api/Products
        public IHttpActionResult GetProducts()
        {
            var products = db.Products.OrderBy(pr => pr.ProductCode).ToList();
            var productsResponse = new List<ProductResponse>();
            //Recorremos la consulta 
            foreach (var product in products)
            {
                var inventoriesResponse = new List<InventoryResponse>();
                foreach (var inventory in product.Inventories)
                {
                    inventoriesResponse.Add(new InventoryResponse
                    {
                        InventoryId = inventory.InventoryId,
                        Product = inventory.Product,
                        Stock = inventory.Stock,
                        Warehouse = inventory.Warehouse,
                    });
                }
                productsResponse.Add(new ProductResponse
                {
                    Category = product.Category,
                    Company = product.Company,
                    Description = product.Description,
                    Image = product.Image,
                    Inventories = inventoriesResponse,
                    Price = product.Price,
                    ProductId = product.ProductId,
                    Stock = product.Stock,
                    CategoryId = product.CategoryId,
                    CompanyId = product.CompanyId,
                    Cost = product.Cost,
                    ProductCode = product.ProductCode,
                });
            }
            return Ok(productsResponse);
        }

        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductId)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Products.Add(product);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductId == id) > 0;
        }
    }
}