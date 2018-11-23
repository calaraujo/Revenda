using Revenda.Classes;
using Revenda.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Mvc;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static Revenda.Models.Movement;

namespace Revenda.Controllers
{
    [Authorize(Roles = "User, Admin")]
    public class ConsignmentsController : Controller
    {
        private RevendaContext db = new RevendaContext();

        public ActionResult HitConsignment(int id)
        {
            var consignment = db.Consignments.Find(id);

            var view = new EditConsignmentView
            {                
                ConsignmentId = consignment.ConsignmentId,
                ConditionId = consignment.ConditionId,
                Remarks = consignment.Remarks,
                SellerId = consignment.SellerId,
                WarehouseId = consignment.WarehouseId,
                Condition = consignment.Condition,
                Warehouse = consignment.Warehouse,
                Seller = consignment.Seller,
                Status = consignment.Status,
                Data = consignment.Data,
                HitDate = consignment.HitDate,
                Details = db.ConsignmentsDetails.Where(cdt => cdt.ConsignmentId == id).ToList(),
            };

            return View(view);
        }

        public ActionResult ViewerEvent()
        {
            return StiMvcViewer.ViewerEventResult();
        }

        public ActionResult DesignerEvent()
        {
            return StiMvcDesigner.DesignerEventResult();
        }

        public ActionResult ReportForm(int? id, string source)
        {
            // Create the report object
            StiReport report = new StiReport();
            if (source == "Consignments")
            { 
                report.Load(Server.MapPath("~/Reports/Consignments.mrt"));
            }
            if (source == "HitConsignments")
            {
                report.Load(Server.MapPath("~/Reports/HitConsignments.mrt"));
            }
            report.Dictionary.Databases.Clear();
            StiSqlDatabase db = new StiSqlDatabase("DefaultConnection", "Data Source=10.61.44.196;Initial Catalog=cadeara1_semijoias;Persist Security Info=True;User ID=cadeara1_admin;Password=!Admin1956.");
            report.Dictionary.Databases.Add(db);
            report.CacheAllData = true;
            report.Dictionary.Synchronize();
            report.Compile();
            report["consignmentId"] = id;
            return StiMvcViewer.GetReportResult(report);
        }

        // GET : Delete Product
        public ActionResult DeleteProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var consignmentsDetailTmp = db.ConsignmentsDetailTmps.Where(sdt => sdt.UserName == User.Identity.Name &&
                sdt.ProductId == id).FirstOrDefault();

            if (consignmentsDetailTmp == null)
            {
                return HttpNotFound();
            }

            db.ConsignmentsDetailTmps.Remove(consignmentsDetailTmp);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                return RedirectToAction("Create");
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View(consignmentsDetailTmp);
        }

        // GET : Search Receivable
        public ActionResult CopyConsignment(int? id)
        {
            int consignmentId = Convert.ToInt32(id);
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var username = user.UserName;
            var response = MovementsHelper.CopyConsignment(consignmentId, username);
            if (response.Succeeded)
            {
                TempData["Message"] = "Registro copiado com sucesso. \n Clique em nova para recuperar os dados.";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
            }
            return PartialView();
        }

        // GET : Search Receivable
        public ActionResult SearchReceivable(int? id)
        {

            int consignmentId = Convert.ToInt32(id);
            var consignments = db.Consignments.Find(consignmentId);
 
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var consignmentReceivable = db.ConsignmentReceivables.Where(r => r.ConsignmentId == id).FirstOrDefault();

            if (consignmentReceivable == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var view1 = new NewConsignmentReceivableView
            {
                ConditionId = consignmentReceivable.ConditionId,
                SellerId = consignmentReceivable.SellerId,
                ConsignmentId = consignmentReceivable.ConsignmentId,
                Status = consignmentReceivable.Status,
                Payment = consignmentReceivable.Payment,
                Date = consignmentReceivable.Date,
                Details =
                    db.ConsignmentReceivableDetails.
                        Where(r => r.ConsignmentReceivableId == consignmentReceivable.ConsignmentReceivableId &&
                        r.Balance != 0)
                        .ToList(),
            };

            ViewBag.ConsignmentReceivableId = new SelectList(CombosHelper.GetConsignmentReceivables(consignmentId, true), "ConsignmentReceivableId", "ConsignmentReceivableId");
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", consignmentReceivable.ConditionId);
            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName", consignmentReceivable.SellerId);

            return View(view1);
        }

        // GET : Receipt
        public ActionResult Receipt(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var consignmentreceivableDetails = db.ConsignmentReceivableDetails.Where(r => r.ConsignmentReceivableDetailId == id).FirstOrDefault();

            var receipt = new Receipt
            {
                Date = DateTime.Now,
                Value = consignmentreceivableDetails.Balance,
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
                var consignmentReceivableDetails = db.ConsignmentReceivableDetails.Where(r => r.ConsignmentReceivableDetailId == id).FirstOrDefault();

                if (consignmentReceivableDetails != null)
                {
                    difer = consignmentReceivableDetails.Value - consignmentReceivableDetails.ValuePaid - view.Value;
                    consignmentReceivableDetails.ValuePaid += view.Value;
                    if (difer != 0)
                    {
                        consignmentReceivableDetails.Balance = Math.Round(difer, 2);
                    }
                    else
                    {
                        consignmentReceivableDetails.Balance = difer;
                    }

                    consignmentReceivableDetails.ReceiptDate = view.Date;
                    db.Entry(consignmentReceivableDetails).State = EntityState.Modified;

                    // Atualiza Fluxo de Caixa
                    var param = db.Parameters.FirstOrDefault(p => p.Identity == "RVCO");
                    var id1 = param.ParameterId;
                    var account = db.Accounts.Where(a => a.AccountCode == param.Value).FirstOrDefault();
                    var description = "Recebido Parcela - Venda Consignada";
                    var value = consignmentReceivableDetails.ValuePaid;
                    var data = consignmentReceivableDetails.ReceiptDate.Value;
                    var statementtype = TypeOfStatement.Realizado;
                    MovementsHelper.UpdateCashFlow(id1, description, value, statementtype, data, account.AccountId);

                    // Atualizar Table Receivables

                    var consignmentReceivables = db.ConsignmentReceivables.Find(consignmentReceivableDetails.ConsignmentReceivableId);

                    var consigreceivablesDetails = db.ConsignmentReceivableDetails.Where(r => r.ConsignmentReceivableId == consignmentReceivables.ConsignmentReceivableId && r.Balance != 0).ToList();

                    foreach (var detail in consigreceivablesDetails)
                    {
                        if (detail.DueDate < DateTime.Now)
                        {
                            consignmentReceivables.Payment = "Vencido";
                        }
                        soma += detail.Balance;
                    }

                    if (soma == 0)
                    {
                        consignmentReceivables.Status = "Liquidado";
                        consignmentReceivables.Payment = "Liquidado";
                        var consignment = db.Consignments.Where(s => s.ConsignmentId == consignmentReceivables.ConsignmentId).FirstOrDefault();
                        consignment.Status = "Liquidado";
                        db.Entry(consignment).State = EntityState.Modified;
                    }

                    db.Entry(consignmentReceivables).State = EntityState.Modified;
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

        // GET : Add ReturnConsignment
        public ActionResult AddReturn(int id)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var consignmentsDetails = db.ConsignmentsDetails.FirstOrDefault(c => c.ConsignmentsDetailId == id);
            TempData["IdOri"] = consignmentsDetails.ConsignmentId;
            var consignmentsDetail = new ConsignmentsDetail
            {
                ConsignmentId = consignmentsDetails.ConsignmentId,
                ConsignmentsDetailId = consignmentsDetails.ConsignmentsDetailId,
                ReturnQuantity = 0,                              
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView("AddReturn", consignmentsDetail);
            }
            return PartialView(consignmentsDetail);
        }

        // POST : Add ReturnConsignment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReturn(ConsignmentsDetail view)
        {
            var response = MovementsHelper.ReturnConsignment(view);
            if (response.Succeeded)
            {
                //var id = view.ConsignmentsDetailId;
                return RedirectToAction("HitConsignment", new { id = TempData["IdOri"] });
            }
            else  
            {
                ModelState.AddModelError(string.Empty, response.Message);
            }
            return PartialView(view);
        }

        // Get : Get UpdateConsignment
        public ActionResult UpdateConsignment(int id)
        {
            TempData["IdOri"] = id;
            var response = MovementsHelper.UpdateConsignment(id);
            if (response.Succeeded)
            {
                return RedirectToAction("HitConsignment", new { id = TempData["IdOri"] });
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
            }
            return PartialView("HitConsignment", new { id = TempData["IdOri"] });
        }

        // Get : Get CancelConsignment
        public ActionResult CancelConsignment(int id)
        {
            TempData["IdOri"] = id;
            var response = MovementsHelper.CancelConsignment(id);
            if (response.Succeeded)
            {
                var id2 = TempData["IdOri"].GetHashCode();
                TempData["Message"] = response.Message;
                return RedirectToAction("HitConsignment", new { id = id2 });
            }
            else
            {
               // ModelState.AddModelError(string.Empty, response.Message);
            }
            var id1 = TempData["IdOri"].GetHashCode();
            return PartialView("HitConsignment", new { id = id1 });
        }

        // Get : Get CreateReceivables
        public ActionResult CreateReceivables(int id)
        {
            TempData["IdOri"] = id;
            var response = MovementsHelper.CreateReceivables(id);
            if (response.Succeeded)
            {
                return RedirectToAction("HitConsignment", new { id = TempData["IdOri"] });
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
            }
            return PartialView("HitConsignment", new { id = TempData["IdOri"] });
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
        public ActionResult AddProduct(AddConsignmentView view)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var consignmentsDetailTmp = db.ConsignmentsDetailTmps.Where(odt => odt.UserName == User.Identity.Name &&
                    odt.ProductId == view.ProductId).FirstOrDefault();
                if (consignmentsDetailTmp == null)
                {
                    var product = db.Products.Find(view.ProductId);
                    consignmentsDetailTmp = new ConsignmentsDetailTmp
                    {
                        Description = product.Description,
                        Price = product.Price,
                        ProductId = product.ProductId,
                        Quantity = view.Quantity,
                        UserName = User.Identity.Name,
                        ReturnQuantity = 0,
                        SaleQuantity = 0,
                    };

                    db.ConsignmentsDetailTmps.Add(consignmentsDetailTmp);
                }
                else
                {
                    consignmentsDetailTmp.Quantity += view.Quantity;
                    consignmentsDetailTmp.ReturnQuantity = 0;
                    consignmentsDetailTmp.SaleQuantity = 0;
                    db.Entry(consignmentsDetailTmp).State = EntityState.Modified;
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

        // GET: Consignments
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            var consignments = db.Consignments.Where(s => s.CompanyId == user.CompanyId)
                .Include(s => s.Company)
                .Include(s => s.Condition)
                .Include(s => s.Warehouse)
                .Include(s => s.Seller)
                .OrderBy(s => s.ConsignmentId).ToList();
            
            return View(consignments);
        }

        //GET: Consignments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consignment Consignment = db.Consignments.Find(id);
            if (Consignment == null)
            {
                return HttpNotFound();
            }
            return View(Consignment);
        }

        // GET: Consignments/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name");
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description");
            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName");

            var view = new NewConsignmentView
            {
                Data = DateTime.Now,
                Details = db.ConsignmentsDetailTmps.Where(sdt => sdt.UserName == User.Identity.Name).ToList(),
            };

            return View(view);
        }

        // POST: Consignments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewConsignmentView view)
        {
            if (ModelState.IsValid)
            {
                var response = MovementsHelper.NewConsignment(view, User.Identity.Name);
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
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", view.WarehouseId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", view.ConditionId);
            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName", view.SellerId);
            return View(view);
        }

        // GET: Consignments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consignment Consignment = db.Consignments.Find(id);

            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", Consignment.WarehouseId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", Consignment.ConditionId);
            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName", Consignment.SellerId);

            return View(Consignment);
        }

        // POST: Consignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Consignment Consignment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Consignment).State = EntityState.Modified;
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }

            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", Consignment.WarehouseId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", Consignment.ConditionId);
            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName", Consignment.SellerId);

            return View(Consignment);
        }

        // GET: Consignments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consignment Consignment = db.Consignments.Find(id);
            if (Consignment == null)
            {
                return HttpNotFound();
            }
            return View(Consignment);

        }

        // POST: Consignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Consignment Consignment = db.Consignments.Find(id);
            var response = MovementsHelper.DeleteConsignment(Consignment.ConsignmentId, User.Identity.Name);
            if (response.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View(Consignment);
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
