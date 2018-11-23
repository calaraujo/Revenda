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
using Revenda.Classes;

namespace Revenda.Controllers.API
{
    [RoutePrefix("api/Sales")]
    public class SalesController : ApiController
    {
        private RevendaContext db = new RevendaContext();

        // GET: api/Sales
        public IHttpActionResult GetSales()
        {
            var sales = db.Sales.ToList();
            var salesAPI = new List<SalesApi>();
            foreach (var sale in sales)
            {
                if (sale.Remarks == null)
                {
                    sale.Remarks = "";
                }
                var salesDetailsAPI = new List<SaleDetailApi>();
                foreach (var saleDetail in sale.SalesDetails)
                {
                    salesDetailsAPI.Add(new SaleDetailApi
                    {
                        Description = saleDetail.Description,                        
                        SaleDetailId = saleDetail.SaleDetailId,
                        SaleId = saleDetail.SaleId,
                        Price = saleDetail.Price,
                        Product = saleDetail.Product,
                        ProductId = saleDetail.ProductId,
                        Quantity = saleDetail.Quantity,
                    });
                }
                salesAPI.Add(new SalesApi
                {
                    Customer = sale.Customer,
                    CustomerId = sale.CustomerId,
                    Company = sale.Company,
                    CompanyId = sale.CompanyId,
                    Date = sale.Date,
                    OrderId = sale.OrderId,
                    Remarks = sale.Remarks,
                    SaleDetails = salesDetailsAPI,
                    ConditionId = sale.ConditionId,
                    Condition = sale.Condition,
                    Seller = sale.Seller,
                    SellerId = sale.SellerId,
                    SaleId = sale.SaleId,
                    Status = sale.Status,
                    WarehouseId = sale.WarehouseId,
                    Warehouse = sale.Warehouse,
                });
            }
            return Ok(salesAPI);
        }

        // GET: api/Sales/5
        [ResponseType(typeof(Sale))]
        public async Task<IHttpActionResult> GetSale(int id)
        {
            Sale sale = await db.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            return Ok(sale);
        }

        // PUT: api/Sales/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSale(int id, Sale sale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sale.SaleId)
            {
                return BadRequest();
            }

            db.Entry(sale).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleExists(id))
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

        // POST: api/Sales
        [ResponseType(typeof(Sale))]
        public async Task<IHttpActionResult> PostSale(SaleRequest saleRequest)
        {
            var details = new List<SalesDetailTmp>();
            foreach (var detail in saleRequest.SaleDetails)
            {
                var product = db.Products.Find(detail.ProductId);
                if (product != null)
                {
                    details.Add(new SalesDetailTmp
                    {
                        Description = product.Description,
                        Price = product.Price,
                        ProductId = product.ProductId,
                        Quantity = detail.Quantity,
                        UserName = saleRequest.UserName,
                        OrderId = saleRequest.OrderId,
                        
                    });
                }
            }
            var newSaleView = new NewSaleView
            {
                ConditionId = saleRequest.ConditionId,
                CustomerId = saleRequest.CustomerId,
                Date = saleRequest.Date,
                Remarks = saleRequest.Remarks,
                SellerId = saleRequest.SellerId,
                Details = details,
                OrderId = saleRequest.OrderId,
                WarehouseId = saleRequest.WarehouseId,
            };

            var response = MovementsHelper.NewSale(newSaleView, saleRequest.UserName, true);
            if (response.Succeeded)
            {
                return Ok("Venda foi criada.");
            }
            return BadRequest(response.Message);
        }

        // DELETE: api/Sales/5
        [ResponseType(typeof(Sale))]
        public async Task<IHttpActionResult> DeleteSale(int id)
        {
            Sale sale = await db.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            db.Sales.Remove(sale);
            await db.SaveChangesAsync();

            return Ok(sale);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SaleExists(int id)
        {
            return db.Sales.Count(e => e.SaleId == id) > 0;
        }
    }
}