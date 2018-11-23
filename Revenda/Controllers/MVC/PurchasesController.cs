using Revenda.Classes;
using Revenda.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static Revenda.Models.Movement;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class PurchasesController : Controller
    {
        private RevendaContext db = new RevendaContext();
        private RevendaEntities context = new RevendaEntities();
        private List<Payable> payable = new List<Payable>();
        public int payabelId;

        public ActionResult ViewerEvent()
        {
            return StiMvcViewer.ViewerEventResult();
        }

        public ActionResult DesignerEvent()
        {
            return StiMvcDesigner.DesignerEventResult();
        }

        public ActionResult ReportForm()
        {
            // Create the report object
            StiReport report = new StiReport();

            report.Load(Server.MapPath("~/Reports/Purchase.mrt"));

            return StiMvcViewer.GetReportResult(report);

        }

        public ActionResult GetPayables(int? id)
        {
            var payableId = Convert.ToInt32(id);

            var payableDetails = db.PayableDetails.Where(p => p.PayableId == payableId && p.Balance != 0).ToList();

            return PartialView(payableDetails);
        }

        // GET : Search Payable
        public ActionResult SearchPayable(int? id)
        {

            int purchaseId = Convert.ToInt32(id);
            int payableId = 0;
            TempData["purchaseId"] = purchaseId;
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var payable = db.Payables.Where(r => r.PurchaseId == purchaseId)
                    .Include(r => r.Supplier)
                    .Include(r => r.Condition)
                    .ToList();
            // Se o contas a pagar não existir deve-se criá-lo.
            if (payable == null)
            {
                MovementsHelper.CreatePayable(purchaseId, "Compra");
                payable = db.Payables.Where(r => r.PurchaseId == purchaseId)
                    .Include(r => r.Supplier)
                    .Include(r => r.Condition)
                    .ToList();
            }

            List<NewPayableView> view = new List<NewPayableView>();

            for (int i = 0; i < payable.Count(); i++)
            {
                NewPayableView obj1 = new NewPayableView();
                obj1.ConditionId = payable[i].ConditionId;
                obj1.Date = payable[i].Date;
                obj1.PayableId = payable[i].PayableId;
                payableId = payable[i].PayableId;
                obj1.Payment = payable[i].Payment;
                obj1.Status = payable[i].Status;
                obj1.SupplierId = payable[i].SupplierId;
                obj1.Supplier = payable[i].Supplier;
                obj1.Condition = payable[i].Condition;
                //obj1.Details = db.PayableDetails.Where(r => r.PayableId == payableId && r.Balance != 0).ToList();
                view.Add(obj1);
            }

            return View(view);
        }

        // GET : NewPayment
        public ActionResult NewPayment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var payableDetails = db.PayableDetails.Where(r => r.PayableDetailId == id).FirstOrDefault();

            var newpayment = new NewPayment
            {
                Date = DateTime.Now,
                Value = payableDetails.Balance,
            };

            return PartialView(newpayment);
        }

        // POST : Receipt
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPayment(NewPayment view, int id)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            decimal difer = 0;
            decimal soma = 0;

            if (ModelState.IsValid)
            {
                var payableDetails = db.PayableDetails.Where(r => r.PayableDetailId == id).FirstOrDefault();

                if (payableDetails != null)
                {
                    difer = payableDetails.Value - payableDetails.ValuePaid - view.Value;
                    payableDetails.ValuePaid += view.Value;
                    if (difer != 0)
                    {
                        payableDetails.Balance = Math.Round(difer, 2);
                    }
                    else
                    {
                        payableDetails.Balance = difer;
                    }

                    payableDetails.PaymentDate = view.Date;
                    db.Entry(payableDetails).State = EntityState.Modified;

                    // Atualiza Fluxo de Caixa
                    var param = db.Parameters.FirstOrDefault(p => p.Identity == "CPRO");
                    var account = db.Accounts.Where(a => a.AccountCode == param.Value).FirstOrDefault();
                    var id1 = param.ParameterId;
                    var description = "Pagamento Fornecedor";
                    var value = payableDetails.ValuePaid;
                    var data = payableDetails.PaymentDate.Value;
                    var statementtype = TypeOfStatement.Realizado;
                    MovementsHelper.UpdateCashFlow(id1, description, value, statementtype, data, account.AccountId);

                    // Atualizar Table Payables

                    var payables = db.Payables.Find(payableDetails.PayableId);

                    var payablesDetails = db.PayableDetails.Where(r => r.PayableId == payableDetails.PayableId && r.Balance != 0).ToList();

                    foreach (var detail in payablesDetails)
                    {
                        if (detail.PaymentDate < DateTime.Now)
                        {
                            payables.Payment = "Vencido";
                        }
                        soma += detail.Balance;
                    }

                    if (soma == 0)
                    {
                        payables.Status = "Liquidado";
                        payables.Payment = "Liquidado";
                        var purchase = db.Purchases.Where(s => s.PurchaseId == payables.PurchaseId).FirstOrDefault();
                        purchase.Status = "Liquidado";
                        db.Entry(purchase).State = EntityState.Modified;
                    }

                    db.Entry(payables).State = EntityState.Modified;
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

        // GET : Delete Product
        public ActionResult DeleteProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var purchaseDetailTmp = db.PurchaseDetailTmps.Where(pdt => pdt.UserName == User.Identity.Name &&
                pdt.ProductId == id).FirstOrDefault();

            if (purchaseDetailTmp == null)
            {
                return HttpNotFound();
            }

            db.PurchaseDetailTmps.Remove(purchaseDetailTmp);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                return RedirectToAction("Create");
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View(purchaseDetailTmp);
        }

        // GET : Add Product
        public ActionResult AddProductPurchase()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.ProductId = new SelectList(CombosHelper.GetProducts(user.CompanyId, true), "ProductId", "ProductCode");
            return PartialView();
        }

        // POST : Add Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProductPurchase(AddProductPurchase view)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            
            if (ModelState.IsValid)
            {
                var purchaseDetailTmp = db.PurchaseDetailTmps.Where(odt => odt.UserName == User.Identity.Name &&
                    odt.ProductId == view.ProductId).FirstOrDefault();

                if (purchaseDetailTmp == null)
                {
                    var product = db.Products.Find(view.ProductId);
                    purchaseDetailTmp = new PurchaseDetailTmp
                    {
                        Description = product.Description,
                        Cost = product.Cost,
                        ProductId = product.ProductId,
                        Quantity = view.Quantity,
                        UserName = User.Identity.Name,
                        PurchaseId = 0,
                    };

                    db.PurchaseDetailTmps.Add(purchaseDetailTmp);
                }
                else
                {
                    purchaseDetailTmp.Quantity += view.Quantity;
                    db.Entry(purchaseDetailTmp).State = EntityState.Modified;
                }

                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {                 
                    return RedirectToAction("Create");
                }

                ModelState.AddModelError(string.Empty, response.Message);

            }

            ViewBag.ProductId = new SelectList(CombosHelper.GetProducts(user.CompanyId), "ProductId", "Description");
            return PartialView(view);
        }

        // GET: Purchases
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            var purchases = db.Purchases.Where(p => p.CompanyId == user.CompanyId)
                .Include(p => p.Condition)
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .OrderBy(p => p.PurchaseId);
            return View(purchases);
        }

        // GET: Purchases/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            return View(purchase);
        }

        [HttpGet]
        // GET: Purchases/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.SupplierId = new SelectList(CombosHelper.GetSuppliers(), "SupplierId", "FullName");
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name");
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description");

            var view = new NewPurchaseView
            {
                Date = DateTime.Now,
                Details = db.PurchaseDetailTmps.Where(pdt => pdt.UserName == User.Identity.Name).ToList(),
            };
            
            return View(view);
        }
        
        // POST: Purchases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewPurchaseView view)
        {
            if (ModelState.IsValid)
            {
                var response = MovementsHelper.NewPurchase(view, User.Identity.Name);

                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                ModelState.AddModelError(string.Empty, messages);
            }

            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.SupplierId = new SelectList(CombosHelper.GetSuppliers(), "SupplierId", "FullName", view.SupplierId);
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", view.WarehouseId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", view.ConditionId);
            view.Details = db.PurchaseDetailTmps.Where(pdt => pdt.UserName == User.Identity.Name).ToList();
            return View(view);
        }

        // GET: Purchases/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.SupplierId = new SelectList(CombosHelper.GetSuppliers(), "SupplierId", "UserName", purchase.SupplierId);
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", purchase.WarehouseId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", purchase.ConditionId);
            return View(purchase);
        }

        // POST: Purchases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Purchase purchase)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {
                db.Entry(purchase).State = EntityState.Modified;
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }

            ViewBag.SupplierId = new SelectList(CombosHelper.GetSuppliers(), "SupplierId", "UserName", purchase.SupplierId);
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", purchase.WarehouseId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", purchase.ConditionId);
            return View(purchase);
        }

        // GET: Purchases/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            return View(purchase);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Purchase purchase = db.Purchases.Find(id);
            var response = MovementsHelper.DeletePurchase(purchase.PurchaseId, User.Identity.Name);
            if (response.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View(purchase);
        }

        // GET: Purchases/Changes
        public ActionResult Changes(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);

            var p1tmp = db.PurchaseDetailTmps.ToList();
            if (p1tmp.Count == 0)
            {

                var pDetails = db.PurchaseDetails.Where(pdt => pdt.PurchaseId == purchase.PurchaseId).ToList();

                foreach (var pdetail in pDetails)
                {
                    var ptmp = new PurchaseDetailTmp
                    {
                        Cost = pdetail.Cost,
                        ProductId = pdetail.ProductId,
                        Description = pdetail.Description,
                        Quantity = pdetail.Quantity,
                        UserName = User.Identity.Name,
                        Product = pdetail.Product,
                        PurchaseId = purchase.PurchaseId,
                    };
                    db.PurchaseDetailTmps.Add(ptmp);
                }

                db.SaveChanges();
            }
            var view = new NewPurchaseView
            {
                Date = purchase.Date,
                ConditionId = purchase.ConditionId,
                Remarks = purchase.Remarks,
                SupplierId = purchase.SupplierId,
                WarehouseId = purchase.WarehouseId,

                Details = db.PurchaseDetailTmps.Where(pdt => pdt.UserName == User.Identity.Name).ToList(),
            };

            if (purchase == null)
            {
                return HttpNotFound();
            }

            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.PurchaseId = purchase.PurchaseId;
            ViewBag.SupplierId = new SelectList(CombosHelper.GetSuppliers(), "SupplierId", "UserName", purchase.SupplierId);
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", purchase.WarehouseId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", purchase.ConditionId);
            return View(view);
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
