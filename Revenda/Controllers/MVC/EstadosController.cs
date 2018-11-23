using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Revenda.Models;
using PagedList;
using Revenda.Classes;
//using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class EstadosController : Controller
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

            report.Load(Server.MapPath("~/Reports/Estados.mrt"));

            return StiMvcViewer.GetReportResult(report);

        }

        private void EstadosReport()
        {
            //ReportViewer reportViewer = new ReportViewer();
            //reportViewer.ProcessingMode = ProcessingMode.Local;
            //reportViewer.SizeToReportContent = true;
            //reportViewer.Width = Unit.Percentage(100);
            //reportViewer.Height = Unit.Percentage(100);
            //reportViewer.ZoomMode = ZoomMode.PageWidth;

            //FillDataSet();

            //reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\Estados.rdlc";
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
            return "SELECT EstadoId AS Código, Name AS Nome" +
                    " FROM   Estadoes" +
                    " ORDER BY Nome";
        }

        //private ReportParameter[] GetParametersLocal()
        //{
        //    ReportParameter p1 = new ReportParameter("ReportTitle", "Estados");
        //    return new ReportParameter[] { p1 };
        //}

        // GET: Estados
        public ActionResult Index(int? page = null)
        {
            //return View(db.Estadoes.ToList());
            page = (page ?? 1);
            return View(db.Estadoes
                .OrderBy(d => d.Name)
                .ToPagedList((int)page, 5));
        }

        // GET: Estados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado estado = db.Estadoes.Find(id);
            if (estado == null)
            {
                return HttpNotFound();
            }
            return View(estado);
        }

        // GET: Estados/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Estados/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EstadoId,Name")] Estado estado)
        {
            if (ModelState.IsValid)
            {
                db.Estadoes.Add(estado);
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            return View(estado);
        }

        // GET: Estados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado estado = db.Estadoes.Find(id);
            if (estado == null)
            {
                return HttpNotFound();
            }
            return View(estado);
        }

        // POST: Estados/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EstadoId,Name")] Estado estado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(estado).State = EntityState.Modified;
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }
            return View(estado);
        }

        // GET: Estados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado estado = db.Estadoes.Find(id);
            if (estado == null)
            {
                return HttpNotFound();
            }
            return View(estado);
        }

        // POST: Estados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Estado estado = db.Estadoes.Find(id);
            db.Estadoes.Remove(estado);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, response.Message);
            return View(estado);
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
