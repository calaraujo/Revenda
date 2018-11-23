using Revenda.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report.Web;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class MovementsController : Controller
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

        public ActionResult ReportForm(int? id, string source)
        {
            // Create the report object
            StiReport report = new StiReport();

            report.Load(Server.MapPath("~/Reports/CashFlow.mrt"));

            return StiMvcViewer.GetReportResult(report);
        }

        public ActionResult Design()
        {
            StiReport report = new StiReport();

            report.Load(Server.MapPath("~/Reports/CashFlow.mrt"));
            
            return StiMvcDesigner.GetReportResult(report);

        }

        public ActionResult ReportDesign()

        {
            StiReport report = StiMvcViewer.GetReportObject();

            ViewBag.ReportName = report.ReportName;

            return View("DesignReport");
        }

        // GET: Movements
        public ActionResult Index()
        {
            var movements = db.Movements
                .Include(m => m.Parameter)
                .Include(m => m.Account)
                .OrderBy(p => p.Data)
                .ThenBy(p => p.StatementType)
                .ThenBy(p => p.MovementId);
            
            return View(movements.ToList());
        }

        // GET: Movements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movement movement = db.Movements.Find(id);
            if (movement == null)
            {
                return HttpNotFound();
            }
            return View(movement);
        }

        // GET: Movements/Create
        public ActionResult Create()
        {
            ViewBag.ParameterId = new SelectList(db.Parameters, "ParameterId", "Identity");
            return View();
        }

        // POST: Movements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MovementId,ParameterId,StatementType,Data,Description,Value")] Movement movement)
        {
            if (ModelState.IsValid)
            {
                db.Movements.Add(movement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ParameterId = new SelectList(db.Parameters, "ParameterId", "Identity", movement.ParameterId);
            return View(movement);
        }

        // GET: Movements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movement movement = db.Movements.Find(id);
            if (movement == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParameterId = new SelectList(db.Parameters, "ParameterId", "Identity", movement.ParameterId);
            return View(movement);
        }

        // POST: Movements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MovementId,ParameterId,StatementType,Data,Description,Value")] Movement movement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParameterId = new SelectList(db.Parameters, "ParameterId", "Identity", movement.ParameterId);
            return View(movement);
        }

        // GET: Movements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movement movement = db.Movements.Find(id);
            if (movement == null)
            {
                return HttpNotFound();
            }
            return View(movement);
        }

        // POST: Movements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movement movement = db.Movements.Find(id);
            db.Movements.Remove(movement);
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
