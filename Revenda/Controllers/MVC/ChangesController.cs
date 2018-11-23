using Revenda.Classes;
using Revenda.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class ChangesController : Controller
    {

        private RevendaContext db = new RevendaContext();


        // GET : Change Product
        public ActionResult ChangeProduct(int? id, int? Pedido)
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

            var purchase = db.Purchases.Find(Pedido);
            purchase.TotalCost = 0;
            purchase.TotalQuantity = 0;

            var detalhes = db.PurchaseDetails.Where(pd => pd.PurchaseId == purchase.PurchaseId &&
                            pd.ProductId == id).FirstOrDefault();

            var inventory1 = db.Inventories.Where(i => i.WarehouseId == purchase.WarehouseId
                    && i.ProductId == detalhes.ProductId).FirstOrDefault();
            if (inventory1 != null)
            {
                inventory1.Stock -= detalhes.Quantity;
                db.Entry(inventory1).State = EntityState.Modified;
            }
            db.PurchaseDetails.Remove(detalhes);

            db.PurchaseDetailTmps.Remove(purchaseDetailTmp);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                return RedirectToAction("Changes", new { id = purchaseDetailTmp.PurchaseId });
            }
            ModelState.AddModelError(string.Empty, response.Message);
            return View(purchaseDetailTmp);
        }

        [HttpGet]
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
            var view = new ChangePurchaseView
            {                
                Date = purchase.Date,
                ConditionId = purchase.ConditionId,
                Remarks = purchase.Remarks,
                SupplierId = purchase.SupplierId,
                PurchaseId = purchase.PurchaseId,
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Changes(ChangePurchaseView view)
        {
            if (ModelState.IsValid)
            {
                //var response = new Response();

                var response = MovementsHelper.ChangePurchase(view, User.Identity.Name);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index", "Purchases");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.SupplierId = new SelectList(CombosHelper.GetSuppliers(), "SupplierId", "FullName", view.SupplierId);
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", view.WarehouseId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", view.ConditionId);
            view.Details = db.PurchaseDetailTmps.Where(pdt => pdt.UserName == User.Identity.Name).ToList();
            return View(view);
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
        public ActionResult AddProductPurchase(AddProductPurchase view, int id)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var pedido = 0;

            if (ModelState.IsValid)
            {
                var purchaseDetailTmp = db.PurchaseDetailTmps.Where(odt => odt.UserName == User.Identity.Name &&
                    odt.ProductId == view.ProductId).FirstOrDefault();
                pedido = id;
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
                        PurchaseId = pedido,
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
                    //if (pedido != 0)
                    //{
                    //    return RedirectToAction("Changes", new { id = pedido });
                    //}

                    return RedirectToAction("Changes", new { id = pedido });
                }

                ModelState.AddModelError(string.Empty, response.Message);

            }

            ViewBag.ProductId = new SelectList(CombosHelper.GetProducts(user.CompanyId), "ProductId", "Description");
            return PartialView(view);
        }
    }
}