using Revenda.Classes;
using Revenda.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static Revenda.Models.StockLedger;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class ChangeConsignmentsController : Controller
    {

        private RevendaContext db = new RevendaContext();


        // GET : Change Product
        public ActionResult ChangeProduct(int? id, int? Pedido)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var consignmentDetailTmp = db.ConsignmentsDetailTmps.Where(pdt => pdt.UserName == User.Identity.Name &&
                pdt.ProductId == id).FirstOrDefault();

            if (consignmentDetailTmp == null)
            {
                return HttpNotFound();
            }

            var consignment = db.Consignments.Find(Pedido);
            consignment.TotalValue = 0;
            consignment.TotalQuantity = 0;

            var detalhes = db.ConsignmentsDetails.Where(pd => pd.ConsignmentId == consignment.ConsignmentId &&
                            pd.ProductId == id).FirstOrDefault();
            if (detalhes != null)
            { 
                //var inventory1 = db.Inventories.Where(i => i.WarehouseId == consignment.WarehouseId
                //        && i.ProductId == detalhes.ProductId).FirstOrDefault();
                //if (inventory1 != null)
                //{
                //    inventory1.Stock -= detalhes.Quantity;
                //    db.Entry(inventory1).State = EntityState.Modified;
                //}
                // Retira Qtde do Produto do Mostruário Consignado
                var typeDocument = TypeOfDocument.CANC;
                MovementsHelper.UpdateInventories(consignment.WarehouseId, "SUBTRACT", detalhes.ProductId, detalhes.Quantity, false, typeDocument, consignment.ConsignmentId);
                // Aloca Qtde do Produto do Mostruário Principal
                MovementsHelper.UpdateInventories(consignment.WarehouseId, "ADD", detalhes.ProductId, detalhes.Quantity, true, typeDocument, consignment.ConsignmentId);
                db.ConsignmentsDetails.Remove(detalhes);

                /////////////////////////////////////////////
                // colocar aqui a rotina UpdateInventories //
                ////////////////////////////////////////////
            }
            db.ConsignmentsDetailTmps.Remove(consignmentDetailTmp);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                return RedirectToAction("ChangeConsignments", new { id = consignmentDetailTmp.ConsignmentId});
            }
            ModelState.AddModelError(string.Empty, response.Message);
            return View(consignmentDetailTmp);
        }

        [HttpGet]
        // GET: consignments/Changes
        public ActionResult ChangeConsignments(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consignment consignment = db.Consignments.Find(id);

            var p1tmp = db.ConsignmentsDetailTmps.ToList();
            if (p1tmp.Count == 0)
            {

                var pDetails = db.ConsignmentsDetails.Where(pdt => pdt.ConsignmentId == consignment.ConsignmentId).ToList();

                foreach (var pdetail in pDetails)
                {
                    var ptmp = new ConsignmentsDetailTmp
                    {
                        Price = pdetail.Price,
                        ProductId = pdetail.ProductId,
                        Description = pdetail.Description,
                        Quantity = pdetail.Quantity,
                        UserName = User.Identity.Name,
                        Product = pdetail.Product,
                        ConsignmentId = consignment.ConsignmentId,
                    };
                    db.ConsignmentsDetailTmps.Add(ptmp);
                }

                db.SaveChanges();
            }
            var view = new ChangeConsignmentView
            {                
                Data = consignment.Data,
                ConditionId = consignment.ConditionId,      
                Remarks = consignment.Remarks,
                SellerId = consignment.SellerId,
                ConsignmentId = consignment.ConsignmentId,
                WarehouseId = consignment.WarehouseId,

                Details = db.ConsignmentsDetailTmps.Where(pdt => pdt.UserName == User.Identity.Name).ToList(),
            };

            if (consignment == null)
            {
                return HttpNotFound();
            }

            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.consignmentId = consignment.ConsignmentId;
            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName", consignment.SellerId);
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", consignment.WarehouseId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", consignment.ConditionId);
            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeConsignments(ChangeConsignmentView view)
        {
            if (ModelState.IsValid)
            {
                //var response = new Response();

                var response = MovementsHelper.ChangeConsignment(view, User.Identity.Name);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index", "Consignments");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.SellerId = new SelectList(CombosHelper.GetSeller(), "SellerId", "FullName", view.SellerId);
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", view.WarehouseId);
            ViewBag.ConditionId = new SelectList(CombosHelper.GetCondition(), "ConditionId", "Description", view.ConditionId);
            view.Details = db.ConsignmentsDetailTmps.Where(pdt => pdt.UserName == User.Identity.Name).ToList();
            return View(view);
        }

        // GET : Add Product
        public ActionResult AddProductConsignment()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.ProductId = new SelectList(CombosHelper.GetProducts(user.CompanyId, true), "ProductId", "ProductCode");
            return PartialView();
        }

        // POST : Add Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProductConsignment(AddProductConsignment view, int id)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var pedido = 0;

            if (ModelState.IsValid)
            {
                var consignmentDetailTmp = db.ConsignmentsDetailTmps.Where(odt => odt.UserName == User.Identity.Name &&
                    odt.ProductId == view.ProductId).FirstOrDefault();
                pedido = id;
                if (consignmentDetailTmp == null)
                {
                    var product = db.Products.Find(view.ProductId);
                    consignmentDetailTmp = new ConsignmentsDetailTmp
                    {
                        Description = product.Description,
                        Price = product.Price,
                        ProductId = product.ProductId,
                        Quantity = view.Quantity,
                        UserName = User.Identity.Name,
                        ConsignmentId = pedido,
                    };

                    db.ConsignmentsDetailTmps.Add(consignmentDetailTmp);
                }
                else
                {
                    consignmentDetailTmp.Quantity += view.Quantity;
                    db.Entry(consignmentDetailTmp).State = EntityState.Modified;
                }

                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    //if (pedido != 0)
                    //{
                    //    return RedirectToAction("Changes", new { id = pedido });
                    //}

                    return RedirectToAction("ChangeConsignments", new { id = pedido });
                }

                ModelState.AddModelError(string.Empty, response.Message);

            }

            ViewBag.ProductId = new SelectList(CombosHelper.GetProducts(user.CompanyId), "ProductId", "Description");
            return PartialView(view);
        }
    }
}