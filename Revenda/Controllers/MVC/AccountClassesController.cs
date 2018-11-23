using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Revenda.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class AccountClassesController : Controller
    {
        private RevendaContext db = new RevendaContext();


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

            report.Load(Server.MapPath("~/Reports/AccountClasses.mrt"));

            return StiMvcViewer.GetReportResult(report);

        }

        // GET: AccountClasses
        public ActionResult Index()
        {
            return View(db.AccountClasses.ToList());
        }

        // GET: AccountClasses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountClass accountClass = db.AccountClasses.Find(id);
            if (accountClass == null)
            {
                return HttpNotFound();
            }
            return View(accountClass);
        }

        // GET: AccountClasses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountClass accountClass)
        {
            if (ModelState.IsValid)
            {
                db.AccountClasses.Add(accountClass);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(accountClass);
        }

        // GET: AccountClasses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountClass accountClass = db.AccountClasses.Find(id);
            if (accountClass == null)
            {
                return HttpNotFound();
            }
            return View(accountClass);
        }

        // POST: AccountClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AccountClass accountClass)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accountClass).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(accountClass);
        }

        // GET: AccountClasses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountClass accountClass = db.AccountClasses.Find(id);
            if (accountClass == null)
            {
                return HttpNotFound();
            }
            return View(accountClass);
        }

        // POST: AccountClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccountClass accountClass = db.AccountClasses.Find(id);
            db.AccountClasses.Remove(accountClass);
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
