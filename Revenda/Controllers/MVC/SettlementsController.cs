using PagedList;
using Revenda.Classes;
using Revenda.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class SettlementsController : Controller
    {
        private RevendaContext db = new RevendaContext();

        // GET : Search Payable
        public ActionResult SearchPayable(int? id)
        {
            int settlemendId = Convert.ToInt32(id);
            var settlement = db.Settlements.Where(s => s.SettlementId == settlemendId).FirstOrDefault();            
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var payable = db.Payables.Where(r => r.PurchaseId == settlement.PurchaseId).FirstOrDefault();

            var view1 = new NewPayableView
            {
                ConditionId = payable.ConditionId,
                SupplierId = payable.SupplierId,
                PurchaseId = payable.PurchaseId,
                Status = payable.Status,
                Payment = payable.Payment,
                Date = payable.Date,
                Details = db.PayableDetails.Where(r => r.PayableId == payable.PayableId && r.Balance != 0).ToList(),
            };

            ViewBag.PayableId = new SelectList(CombosHelper.GetPayables(payable.PayableId, true), "PayableId", "PayableId", payable.PayableId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", payable.ConditionId);
            ViewBag.SupplierId = new SelectList(CombosHelper.GetSuppliers(), "SupplierId", "FullName", payable.SupplierId);

            return View(view1);

        }

        // GET : Payment
        public ActionResult Payment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var payablesDetails = db.PayableDetails.Where(p => p.PayableDetailId == id).FirstOrDefault();
            var payables = db.Payables.Where(p => p.PayableId == payablesDetails.PayableId).FirstOrDefault();
            var payment = new Payment
            {
                Date = DateTime.Now,
                Value = payablesDetails.Balance,
                ConditionId = Convert.ToInt32(payables.ConditionId),
            };
            ViewBag.ConditionId = new SelectList(CombosHelper.GetConditions(), "ConditionId", "Description");
            return PartialView(payment);
        }

        // POST : Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(Payment view, int id)
        {
            var paymentview = new Payment
            {
                ConditionId = view.ConditionId,
                Date = view.Date,
                Value = view.Value,
            };
            var payableDetailid = id;

            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            decimal difer = 0;
            decimal soma = 0;
            var settlement = new Settlement();

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

                    // Atualizar Table Payables

                    var payables = db.Payables.Find(payableDetails.PayableId);

                    var payablesDetails = db.PayableDetails.Where(r => r.PayableId == payables.PayableId && r.Balance != 0).ToList();

                    foreach (var detail in payablesDetails)
                    {
                        if (detail.DueDate < DateTime.Now)
                        {
                            payables.Payment = "Vencido";
                        }
                        soma += detail.Balance;
                    }

                    if (soma == 0)
                    {
                        payables.Status = "Liquidado";
                        payables.Payment = "Liquidado";
                        settlement = db.Settlements.Where(s => s.PurchaseId == payables.PurchaseId).FirstOrDefault();
                        settlement.Status = "Liquidado";                       
                    }

                    db.Entry(payables).State = EntityState.Modified;
                    db.Entry(settlement).State = EntityState.Modified;

                    MovementsHelper.UpdatePayable(paymentview, payableDetailid, settlement.SettlementId);
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

        // GET: Calculate
        public ActionResult Calculate(int? id, int? pid)
        {
            var settlements = db.Settlements.Find(id);

            int purchaseId = settlements.PurchaseId;
            purchaseId = Convert.ToInt32(purchaseId);
            int id1 = Convert.ToInt32(id);

            MovementsHelper.Calculus(id1, purchaseId);
            
            return RedirectToAction("Index");
     
        }

        // GET: Settlements
        public ActionResult Index(int? page = null)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            page = (page ?? 1);
            
            return View(db.Settlements
                    //.Include(s => s.Commission)
                    //.Include(s => s.Payable)
                    .Include(s => s.Purchase)                                 
                    .OrderByDescending(s => s.SettlementId)
                    .ToPagedList((int)page, 4));
        }
        // GET: Settlements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Settlement settlement = db.Settlements.Find(id);
            if (settlement == null)
            {
                return HttpNotFound();
            }
            return View(settlement);
        }

        // GET: Settlements/Create
        public ActionResult Create()
        {
            ViewBag.CommissionId = new SelectList(db.Commissions, "CommissionId", "Description");
            ViewBag.PayableId = new SelectList(db.Payables, "PayableId", "PayableId");
            ViewBag.PurchaseId = new SelectList(db.Purchases, "PurchaseId", "PurchaseId");
            return View();
        }

        // POST: Settlements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Settlement settlement)
        {
            settlement.CommissionId = 1;
            settlement.Status = "Em Aberto";
            //settlement.PayableId = 2;
            if (ModelState.IsValid)
            {
                db.Settlements.Add(settlement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.CommissionId = new SelectList(db.Commissions, "CommissionId", "Description", settlement.CommissionId);
            //ViewBag.PayableId = new SelectList(db.Payables, "PayableId", "Status", settlement.PayableId);
            ViewBag.PurchaseId = new SelectList(db.Purchases, "PurchaseId", "PurchaseId", settlement.PurchaseId);
            return View(settlement);
        }

        // GET: Settlements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Settlement settlement = db.Settlements.Find(id);
            if (settlement == null)
            {
                return HttpNotFound();
            }

            ViewBag.CommissionId = new SelectList(CombosHelper.GetCommission(), "CommissionId", "Description", settlement.CommissionId);            
            ViewBag.PurchaseId = new SelectList(CombosHelper.GetPurchases(settlement.PurchaseId, true), "PurchaseId", "PurchaseId", settlement.PurchaseId);
            return View(settlement);
        }

        // POST: Settlements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Settlement settlement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(settlement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CommissionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", settlement.CommissionId);
            ViewBag.PurchaseId = new SelectList(CombosHelper.GetPurchases(settlement.PurchaseId, true), "PurchaseId", "PurchaseId", settlement.PurchaseId);
            return View(settlement);
        }

        // GET: Settlements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Settlement settlement = db.Settlements.Find(id);
            if (settlement == null)
            {
                return HttpNotFound();
            }
            return View(settlement);
        }

        // POST: Settlements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Settlement settlement = db.Settlements.Find(id);
            db.Settlements.Remove(settlement);
            db.SaveChanges();
            return RedirectToAction("Index");
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
