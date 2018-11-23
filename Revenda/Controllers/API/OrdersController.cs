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
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        private RevendaContext db = new RevendaContext();

        // GET: api/Orders
        public IHttpActionResult GetOrders()
        {
            var orders = db.Orders.ToList();
            var ordersAPI = new List<OrdersApi>();
            foreach (var order in orders)
            {
                var ordersDetailsAPI = new List<OrderDetailApi>();
                foreach (var orderDetail in order.OrderDetails)
                {
                    ordersDetailsAPI.Add(new OrderDetailApi
                    {
                        Description = orderDetail.Description,
                        OrderDetailId = orderDetail.OrderDetailId,
                        OrderId = orderDetail.OrderId,
                        Price = orderDetail.Price,
                        Product = orderDetail.Product,
                        ProductId = orderDetail.ProductId,
                        Quantity = orderDetail.Quantity,                        
                    });
                 }
                ordersAPI.Add( new OrdersApi
                {
                    Customer = order.Customer,
                    CustomerId = order.CustomerId,
                    Company = order.Company,
                    CompanyId = order.CompanyId,
                    OrderDate = order.OrderDate,
                    OrderId = order.OrderId,
                    Remarks = order.Remarks,
                    OrderDetails = ordersDetailsAPI,
                    ConditionId = order.ConditionId,
                    Condition = order.Condition,
                    Seller = order.Seller,
                    SellerId = order.SellerId,
                });
            }
            return Ok(ordersAPI);
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> GetOrder(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.OrderId)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(OrderRequest orderRequest)
        {
            var details = new List<OrderDetailTmp>();
            foreach (var detail in orderRequest.Details)
            {
                var product = db.Products.Find(detail.ProductId);
                if (product != null)
                {
                    details.Add(new OrderDetailTmp
                    {
                        Description = product.Description,
                        Price = product.Price,
                        ProductId = product.ProductId,
                        Quantity = detail.Quantity,
                        UserName = orderRequest.UserName,
                        
                    });
                }
            }
            var newOrderView = new NewOrderView
            {
                ConditionId = orderRequest.ConditionId,
                CustomerId = orderRequest.CustomerId,
                OrderDate = orderRequest.OrderDate,
                Remarks = orderRequest.Remarks,
                SellerId = orderRequest.SellerId,  
                Details = details,
               
            };

            var response = MovementsHelper.NewOrder(newOrderView, orderRequest.UserName, true);
            if (response.Succeeded)
            {
                return Ok("Pedido foi criado.");
            }
            return BadRequest(response.Message);
        }

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> DeleteOrder(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.OrderId == id) > 0;
        }
    }
}