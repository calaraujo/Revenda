using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Revenda.Classes;
using Revenda.Models;
using static Revenda.Models.StockLedger;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class InventoriesController : Controller
    {
        private RevendaContext db = new RevendaContext();
        private decimal OldStock { get; set; }

        // GET: Inventories
        public ActionResult Index()
        {
            var inventories = db.Inventories
                .Include(i => i.Product)
                .Include(i => i.Warehouse)
                .OrderBy(i => i.ProductId);
            return View(inventories.ToList());
        }

        // GET: Inventories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // GET: Inventories/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(CombosHelper.GetProduct(), "ProductId", "ProductCode");
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name");
            return View();
        }

        // POST: Inventories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                //var inventorys = db.Inventories.Where(i => i.ProductId == inventory.ProductId &&
                //                                      i.WarehouseId == inventory.WarehouseId).FirstOrDefault();
                //if (inventorys == null)
                //{
                //    db.Inventories.Add(inventory);
                //} else
                //{
                //    inventorys.Stock = inventory.Stock;
                //    db.Entry(inventorys).State = EntityState.Modified;
                //}

                //db.SaveChanges();
                var typeDocument = TypeOfDocument.AJES;
                // Aloca Qtde do Produto no Mostruário Destino
                MovementsHelper.UpdateInventories(inventory.WarehouseId, "ADD", inventory.ProductId, inventory.Stock, false, typeDocument, 1);

                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(CombosHelper.GetProduct(), "ProductId", "ProductCode", inventory.ProductId);
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", inventory.WarehouseId);
            return View(inventory);
        }

        // GET: Inventories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            OldStock = inventory.Stock;
            if (inventory == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(CombosHelper.GetProduct(), "ProductId", "ProductCode", inventory.ProductId);
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", inventory.WarehouseId);
            return View(inventory);
        }

        // POST: Inventories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                var typeDocument = TypeOfDocument.AJES;
                // Aloca Qtde do Produto no Mostruário Destino
                MovementsHelper.UpdateInventories(inventory.WarehouseId, "ADD", inventory.ProductId, inventory.Stock, false, typeDocument, inventory.InventoryId);

                //db.Entry(inventory).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(CombosHelper.GetProduct(), "ProductId", "ProductCode", inventory.ProductId);
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(), "WarehouseId", "Name", inventory.WarehouseId);
            return View(inventory);
        }

        // GET: Inventories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inventory inventory = db.Inventories.Find(id);
            var typeDocument = TypeOfDocument.AJES;
            MovementsHelper.UpdateInventories(inventory.WarehouseId, "ADD", inventory.ProductId, inventory.Stock, false, typeDocument, inventory.InventoryId);
            db.Inventories.Remove(inventory);
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
