using PagedList;
using Revenda.Classes;
using Revenda.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static Revenda.Models.Movement;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class SalesController : Controller
    {
        private RevendaContext db = new RevendaContext();
        public int receivableId;

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
            report.Load(Server.MapPath("~/Reports/Sales.mrt"));
            report.Dictionary.Databases.Clear();
            StiSqlDatabase db = new StiSqlDatabase("DefaultConnection", "Data Source=10.61.44.196;Initial Catalog=cadeara1_semijoias;Persist Security Info=True;User ID=cadeara1_admin;Password=!Admin1956.");
            report.Dictionary.Databases.Add(db);
            report.CacheAllData = true;
            report.Dictionary.Synchronize();
            report.Compile();
            report["saleId"] = id;
            return StiMvcViewer.GetReportResult(report);
        }

        public ActionResult ReportFormSales()
        {
            // Create the report object
            StiReport report = new StiReport();

            report.Load(Server.MapPath("~/Reports/SalesTotal.mrt"));

            return StiMvcViewer.GetReportResult(report);
        }

        // GET : Search Order
        public ActionResult SearchOrder()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.OrderId = new SelectList(CombosHelper.GetOrders(user.CompanyId, true), "OrderId", "OrderId");
            return PartialView();
        }

        public ActionResult GetReceivables(int? id)
        {
            var receivableId = Convert.ToInt32(id);

            var receivableDetails = db.ReceivableDetails.Where(p => p.ReceivableId == receivableId && p.Balance != 0).ToList();

            return PartialView(receivableDetails);
        }

        // GET : Search Receivable
        public ActionResult SearchReceivable(int? id)
        {

            int saleId = Convert.ToInt32(id);
            int receivableId = 0;
            TempData["saleId"] = saleId;
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var receivable = db.Receivables.Where(r => r.SaleId == saleId)
                    .Include(r => r.Customer)
                    .Include(r => r.Condition)
                    .ToList();
            // Se o contas a receber não existir deve-se criá-lo.
            if (receivable == null)
            {
                MovementsHelper.CreateReceivable(saleId, "Venda");
                receivable = db.Receivables.Where(r => r.SaleId == saleId)
                        .Include(r => r.Customer)
                        .Include(r => r.Condition)
                        .ToList();
            }

            List<NewReceivableView> view = new List<NewReceivableView>();

            for (int i = 0; i < receivable.Count(); i++)
            {
                NewReceivableView obj1 = new NewReceivableView();
                obj1.ConditionId = receivable[i].ConditionId;
                obj1.Date = receivable[i].Date;
                obj1.ReceivableId = receivable[i].ReceivableId;
                receivableId = receivable[i].ReceivableId;
                obj1.Payment = receivable[i].Payment;
                obj1.Status = receivable[i].Status;
                obj1.CustomerId = receivable[i].CustomerId;
                obj1.Customer = receivable[i].Customer;
                obj1.Condition = receivable[i].Condition;
                view.Add(obj1);
            }

            return View(view);
        }

        // POST : Search Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchOrder(SearchOrderView view)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var order = db.Orders.Find(view.OrderId);
                var details = db.OrderDetails.Where(od => od.OrderId == order.OrderId).OrderBy(od => od.OrderDetailId).ToList();

                foreach (var detail in details)
                {
                    var salesDetailTmp = db.SalesDetailTmps.Where(sdt => sdt.OrderId == detail.OrderId &&
                        sdt.ProductId == detail.ProductId).FirstOrDefault();
                    if (salesDetailTmp == null)
                    {
                        salesDetailTmp = new SalesDetailTmp
                        {
                            Description = detail.Description,
                            OrderId = detail.OrderId,
                            Price = detail.Price,
                            ProductId = detail.ProductId,
                            Quantity = detail.Quantity,
                            UserName = User.Identity.Name,
                        };
                        db.SalesDetailTmps.Add(salesDetailTmp);
                    }
                    else
                    {
                        salesDetailTmp.Quantity += detail.Quantity;
                        db.Entry(salesDetailTmp).State = EntityState.Modified;
                    }

                    var response = DBHelper.SaveChanges(db);
                    if (!response.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, response.Message);

                    }
                }
                return RedirectToAction("Create");
            }

            ViewBag.OrderId = new SelectList(CombosHelper.GetOrders(user.CompanyId, true), "OrderId", "OrderId");
            return PartialView(view);
        }

        // GET : Delete Product
        public ActionResult DeleteProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var salesDetailTmp = db.SalesDetailTmps.Where(sdt => sdt.UserName == User.Identity.Name &&
                sdt.ProductId == id).FirstOrDefault();

            if (salesDetailTmp == null)
            {
                return HttpNotFound();
            }

            db.SalesDetailTmps.Remove(salesDetailTmp);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                return RedirectToAction("Create");
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View(salesDetailTmp);
        }

        // GET : Receipt
        public ActionResult Receipt(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var receivableDetails = db.ReceivableDetails.Where(r => r.ReceivableDetailId == id).FirstOrDefault();

            var receipt = new Receipt
            {
                Date = DateTime.Now,
                Value = receivableDetails.Balance,
            };

            return PartialView(receipt);
        }

        // POST : Receipt
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Receipt(Receipt view, int id)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            decimal difer = 0;
            decimal soma = 0;

            if (ModelState.IsValid)
            {
                var receivableDetails = db.ReceivableDetails.Where(r => r.ReceivableDetailId == id).FirstOrDefault();

                if (receivableDetails != null)
                {
                    difer = receivableDetails.Value - receivableDetails.ValuePaid - view.Value;
                    receivableDetails.ValuePaid += view.Value;
                    if (difer != 0)
                    {
                        receivableDetails.Balance = Math.Round(difer, 2);
                    }
                    else
                    {
                        receivableDetails.Balance = difer;
                    }

                    receivableDetails.ReceiptDate = view.Date;
                    db.Entry(receivableDetails).State = EntityState.Modified;

                    // Atualiza Fluxo de Caixa
                    var param = db.Parameters.FirstOrDefault(p => p.Identity == "RVDI");
                    var id1 = param.ParameterId;
                    var account = db.Accounts.Where(a => a.AccountCode == param.Value).FirstOrDefault();
                    var description = "Recebido Parcela - Venda Direta";
                    var value = receivableDetails.ValuePaid;
                    var data = receivableDetails.ReceiptDate.Value;
                    var statementtype = TypeOfStatement.Realizado;
                    MovementsHelper.UpdateCashFlow(id1, description, value, statementtype, data, account.AccountId);

                    // Atualizar Table Receivables

                    var receivables = db.Receivables.Find(receivableDetails.ReceivableId);

                    var receivablesDetails = db.ReceivableDetails.Where(r => r.ReceivableId == receivables.ReceivableId && r.Balance != 0).ToList();

                    foreach (var detail in receivablesDetails)
                    {
                        if (detail.DueDate < DateTime.Now)
                        {
                            receivables.Payment = "Vencido";
                        }
                        soma += detail.Balance;
                    }

                    if (soma == 0)
                    {
                        receivables.Status = "Liquidado";
                        receivables.Payment = "Liquidado";
                        var sale = db.Sales.Where(s => s.SaleId == receivables.SaleId).FirstOrDefault();
                        sale.Status = "Liquidado";
                        db.Entry(sale).State = EntityState.Modified;
                    }

                    db.Entry(receivables).State = EntityState.Modified;
                }

                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            return PartialView();

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
                var salesDetailTmp = db.SalesDetailTmps.Where(odt => odt.UserName == User.Identity.Name &&
                    odt.ProductId == view.ProductId).FirstOrDefault();
                if (salesDetailTmp == null)
                {
                    var product = db.Products.Find(view.ProductId);
                    salesDetailTmp = new SalesDetailTmp
                    {
                        Description = product.Description,
                        Price = product.Price,
                        ProductId = product.ProductId,
                        Quantity = view.Quantity,
                        UserName = User.Identity.Name,
                        OrderId = 0,
                    };

                    db.SalesDetailTmps.Add(salesDetailTmp);
                }
                else
                {
                    salesDetailTmp.Quantity += view.Quantity;
                    db.Entry(salesDetailTmp).State = EntityState.Modified;
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

        // GET: Sales
        public ActionResult Index(int? page = null)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            var sales = db.Sales.Where(s => s.CompanyId == user.CompanyId)
                .Include(s => s.Company)
                .Include(s => s.Customer)
                .Include(s => s.Condition)
                .Include(s => s.Warehouse)
                .Include(s => s.Seller)
                .OrderBy(s => s.SaleId);

            page = (page ?? 1);
            return View(sales.ToPagedList((int)page, 5));
        }

        // GET: Sales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = db.Sales.Find(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            return View(sale);
        }

        // GET: Sales/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.CustomerId = new SelectList(CombosHelper.GetCustomers(), "CustomerId", "FullName");
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name");
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description");
            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName");

            var view = new NewSaleView
            {
                Date = DateTime.Now,
                Details = db.SalesDetailTmps.Where(sdt => sdt.UserName == User.Identity.Name).ToList(),
            };

            return View(view);
        }

        // POST: Sales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewSaleView view)
        {
            if (ModelState.IsValid)
            {
                var response = MovementsHelper.NewSale(view, User.Identity.Name, false);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
                //ModelState.AddModelError(string.Empty, response.Message);
            }

            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.CustomerId = new SelectList(CombosHelper.GetCustomers(), "CustomerId", "UserName", view.CustomerId);
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", view.WarehouseId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", view.ConditionId);
            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName", view.SellerId);
            return View(view);
        }

        // GET: Sales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = db.Sales.Find(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.CustomerId = new SelectList(CombosHelper.GetCustomers(), "CustomerId", "UserName", sale.CustomerId);
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", sale.WarehouseId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", sale.ConditionId);
            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName", sale.SellerId);

            return View(sale);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Sale sale)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sale).State = EntityState.Modified;
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }

            ViewBag.CustomerId = new SelectList(CombosHelper.GetCustomers(), "CustomerId", "UserName", sale.CustomerId);
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", sale.WarehouseId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", sale.ConditionId);
            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName", sale.SellerId);

            return View(sale);
        }

        // GET: Sales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sale = db.Sales.Find(id);
            if (sale == null)
            {
                return HttpNotFound();
            }
            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sale sale = db.Sales.Find(id);
            var response = MovementsHelper.DeleteSale(sale.SaleId, User.Identity.Name);
            if (response.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View(sale);
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
