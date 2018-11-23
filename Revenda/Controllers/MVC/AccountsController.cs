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
    public class AccountsController : Controller
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

            report.Load(Server.MapPath("~/Reports/Accounts.mrt"));

            return StiMvcViewer.GetReportResult(report);

        }

        // GET: Accounts
        public ActionResult Index()
        {
            var accounts = db.Accounts.Include(a => a.AccountClass).Include(a => a.AccountSubClass);
            return View(accounts.ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.AccountClassId = new SelectList(CombosHelper.GetAccountClasses(), "AccountClassId", "GroupName");
            ViewBag.AccountSubClassId = new SelectList(CombosHelper.GetAccountSubClasses(), "AccountSubClassId", "SubGroupName");
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.AccountClassId = new SelectList(CombosHelper.GetAccountClasses(), "AccountClassId", "GroupName", account.AccountClassId);
            ViewBag.AccountSubClassId = new SelectList(CombosHelper.GetAccountSubClasses(), "AccountSubClassId", "SubGroupName", account.AccountSubClassId);
            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.AccountClassId = new SelectList(CombosHelper.GetAccountClasses(), "AccountClassId", "GroupName", account.AccountClassId);
            ViewBag.AccountSubClassId = new SelectList(CombosHelper.GetAccountSubClasses(), "AccountSubClassId", "SubGroupName", account.AccountSubClassId);
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.AccountClassId = new SelectList(CombosHelper.GetAccountClasses(), "AccountClassId", "GroupName", account.AccountClassId);
            ViewBag.AccountSubClassId = new SelectList(CombosHelper.GetAccountSubClasses(), "AccountSubClassId", "SubGroupName", account.AccountSubClassId);
            return View(account);
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
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
