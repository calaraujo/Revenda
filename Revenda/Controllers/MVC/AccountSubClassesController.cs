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
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class AccountSubClassesController : Controller
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

            report.Load(Server.MapPath("~/Reports/AccountSubClasses.mrt"));

            return StiMvcViewer.GetReportResult(report);

        }

        // GET: AccountSubClasses
        public ActionResult Index()
        {
            var accountSubClasses = db.AccountSubClasses.Include(a => a.AccountClasses);
            return View(accountSubClasses.ToList());
        }

        // GET: AccountSubClasses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountSubClass accountSubClass = db.AccountSubClasses.Find(id);
            if (accountSubClass == null)
            {
                return HttpNotFound();
            }
            return View(accountSubClass);
        }

        // GET: AccountSubClasses/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.AccountClassId = new SelectList(CombosHelper.GetAccountClasses(), "AccountClassId", "GroupName");
            return View();
        }

        // POST: AccountSubClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountSubClass accountSubClass)
        {
            if (ModelState.IsValid)
            {
                db.AccountSubClasses.Add(accountSubClass);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.AccountClassId = new SelectList(CombosHelper.GetAccountClasses(), "AccountClassId", "GroupName", accountSubClass.AccountClassId);
            return View(accountSubClass);
        }

        // GET: AccountSubClasses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountSubClass accountSubClass = db.AccountSubClasses.Find(id);
            if (accountSubClass == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.AccountClassId = new SelectList(CombosHelper.GetAccountClasses(), "AccountClassId", "GroupName", accountSubClass.AccountClassId);
            return View(accountSubClass);
        }

        // POST: AccountSubClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AccountSubClass accountSubClass)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accountSubClass).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.AccountClassId = new SelectList(CombosHelper.GetAccountClasses(), "AccountClassId", "GroupName", accountSubClass.AccountClassId);
            return View(accountSubClass);
        }

        // GET: AccountSubClasses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountSubClass accountSubClass = db.AccountSubClasses.Find(id);
            if (accountSubClass == null)
            {
                return HttpNotFound();
            }
            return View(accountSubClass);
        }

        // POST: AccountSubClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccountSubClass accountSubClass = db.AccountSubClasses.Find(id);
            db.AccountSubClasses.Remove(accountSubClass);
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
