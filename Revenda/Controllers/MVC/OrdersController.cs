using Revenda.Classes;
using Revenda.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class OrdersController : Controller
    {
        private RevendaContext db = new RevendaContext();
        private decimal preco;

        public ActionResult ViewerEvent()
        {
            return StiMvcViewer.ViewerEventResult();
        }

        public ActionResult DesignerEvent()
        {
            return StiMvcDesigner.DesignerEventResult();
        }

        public ActionResult ReportForm(int? id)
        {
            // Create the report object
            StiReport report = new StiReport();
            report.Load(Server.MapPath("~/Reports/Orders.mrt"));
            report.Dictionary.Databases.Clear();
            StiSqlDatabase db = new StiSqlDatabase("DefaultConnection", "Data Source=10.61.44.196;Initial Catalog=cadeara1_semijoias;Persist Security Info=True;User ID=cadeara1_admin;Password=!Admin1956.");
            report.Dictionary.Databases.Add(db);
            report.CacheAllData = true;
            report.Dictionary.Synchronize();
            report.Compile();
            report["orderId"] = id;
            return StiMvcViewer.GetReportResult(report);
        }

        public ActionResult ReportForm1()
        {
            // Create the report object
            StiReport report = new StiReport();

            report.Load(Server.MapPath("~/Reports/OrdersTotal.mrt"));

            return StiMvcViewer.GetReportResult(report);

        }
        // GET : Delete Product
        public ActionResult DeleteProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var orderDetailTmp = db.OrderDetailTmps.Where(odt => odt.UserName == User.Identity.Name &&
                odt.ProductId == id).FirstOrDefault();

            if (orderDetailTmp == null)
            {
                return HttpNotFound();
            }

            db.OrderDetailTmps.Remove(orderDetailTmp);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                return RedirectToAction("Create");
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View(orderDetailTmp);
        }

        // GET : Add Product
        public ActionResult AddProduct()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.ProductId = new SelectList(CombosHelper.GetProductsWithStock(user.CompanyId, true), "ProductId", "ProductCode");
            return PartialView();
        }

        // POST : Add Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(AddProductView view)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var orderDetailTmp = db.OrderDetailTmps.Where(odt => odt.UserName == User.Identity.Name &&
                    odt.ProductId == view.ProductId).FirstOrDefault();
                if (orderDetailTmp == null)
                {
                    var product = db.Products.Find(view.ProductId);
                    if (view.Price == 0)
                    {

                        preco = product.Price;
                    }
                    else
                    {
                        preco = view.Price;
                    }
                    orderDetailTmp = new OrderDetailTmp
                    {
                        Description = product.Description,
                        Price = preco,
                        ProductId = product.ProductId,
                        Quantity = view.Quantity,
                        UserName = User.Identity.Name,
                    };

                    db.OrderDetailTmps.Add(orderDetailTmp);
                }
                else
                {
                    orderDetailTmp.Quantity += view.Quantity;
                    db.Entry(orderDetailTmp).State = EntityState.Modified;
                }

                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Create");
                }

                ModelState.AddModelError(string.Empty, response.Message);

            }

            ViewBag.ProductId = new SelectList(CombosHelper.GetProductsWithStock(user.CompanyId, true), "ProductId", "ProductCode");
            return PartialView(view);
        }

        // GET: Orders
        public ActionResult Index(int? page = null)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            var orders = db.Orders.Where(o => o.CompanyId == user.CompanyId)
                .Include(o => o.Customer)
                .Include(o => o.Condition)
                .Include(o => o.Seller)
                .OrderBy(o => o.OrderId);

            page = (page ?? 1);
            return View(orders.ToPagedList((int)page, 5));
            
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.CustomerId = new SelectList(CombosHelper.GetCustomers(), "CustomerId", "FullName");
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description");
            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName");

            var view = new NewOrderView
            {
                OrderDate = DateTime.Now,
                Details = db.OrderDetailTmps.Where(odt => odt.UserName == User.Identity.Name).ToList(),
            };

            return View(view);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewOrderView view)
        {
            if (ModelState.IsValid)
            {
                var response = MovementsHelper.NewOrder(view, User.Identity.Name, false);

                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.CustomerId = new SelectList(CombosHelper.GetCustomers(), "CustomerId", "FullName");
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description");
            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName");
            view.Details = db.OrderDetailTmps.Where(odt => odt.UserName == User.Identity.Name).ToList();
            return View(view);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            ViewBag.CustomerId = new SelectList(CombosHelper.GetCustomers(), "CustomerId", "FullName", order.CustomerId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", order.ConditionId);
            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName", order.SellerId);
            
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order order)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);

            }
            ViewBag.CustomerId = new SelectList(CombosHelper.GetCustomers(), "CustomerId", "FullName");
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description");
            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName");
            //ViewBag.StateId = new SelectList(db.States, "StateId", "Description", order.StateId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            var response = MovementsHelper.DeleteOrder(order.OrderId, User.Identity.Name);
            if (response.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
