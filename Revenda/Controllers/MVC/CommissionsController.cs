////using Microsoft.Reporting.WebForms;
using PagedList;
using Revenda.Classes;
using Revenda.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class CommissionsController : Controller
    {
        private RevendaContext db = new RevendaContext();

        //private dsLocalReport tds = new dsLocalReport();

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

            report.Load(Server.MapPath("~/Reports/Commissions.mrt"));

            return StiMvcViewer.GetReportResult(report);

        }

        private void CommissionsReport()
        {
            //ReportViewer reportViewer = new ReportViewer();
            //reportViewer.ProcessingMode = ProcessingMode.Local;
            //reportViewer.SizeToReportContent = true;
            //reportViewer.Width = Unit.Percentage(100);
            //reportViewer.Height = Unit.Percentage(100);
            //reportViewer.ZoomMode = ZoomMode.PageWidth;

            //FillDataSet();

            //reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\Commissions.rdlc";
            //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", tds.Tables[0]));
            //reportViewer.LocalReport.SetParameters(GetParametersLocal());

            //ViewBag.ReportViewer = reportViewer;
        }

        private void FillDataSet()
        {

            //string connectionString = GetConnectionString();

            //using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            //{
            //    string queryString = GetQueryString();

            //    SqlDataAdapter sqlDataAapter = new SqlDataAdapter(queryString, sqlConnection);

            //    sqlDataAapter.Fill(tds, tds.DataTable1.TableName);
            //}
        }

        private string GetConnectionString()
        {
            return "Data Source=.;Initial Catalog=Revenda;Integrated Security=True";
        }

        private string GetQueryString()
        {
            return "SELECT Description AS Descrição, LowerLimit AS Inferior, UpperLimit AS Superior, [Percent] AS Porcentagem " +
                    " FROM Commissions ORDER BY CommissionId";
        }

        //private ReportParameter[] GetParametersLocal()
        //{
        //    ReportParameter p1 = new ReportParameter("ReportTitle", "Lista de Percentuais de Comissão");
        //    return new ReportParameter[] { p1 };
        //}

        // GET: Commissions
        public ActionResult Index(int? page = null)
        {
            page = (page ?? 1);
            var commissions = db.Commissions.OrderBy(p => p.CommissionId);
            return View(commissions.ToPagedList((int)page, 5));
        }

        // GET: Commissions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Commission commission = db.Commissions.Find(id);
            if (commission == null)
            {
                return HttpNotFound();
            }
            return View(commission);
        }

        // GET: Commissions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Commissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Commission commission)
        {
             if (ModelState.IsValid)
            {
                db.Commissions.Add(commission);
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }
            return View(commission);
        }

        // GET: Commissions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Commission commission = db.Commissions.Find(id);
            if (commission == null)
            {
                return HttpNotFound();
            }
            return View(commission);
        }

        // POST: Commissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Commission commission)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commission).State = EntityState.Modified;
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }
            return View(commission);
        }

        // GET: Commissions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Commission commission = db.Commissions.Find(id);
            if (commission == null)
            {
                return HttpNotFound();
            }
            return View(commission);
        }

        // POST: Commissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Commission commission = db.Commissions.Find(id);
            db.Commissions.Remove(commission);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, response.Message);
            return View(commission);
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
