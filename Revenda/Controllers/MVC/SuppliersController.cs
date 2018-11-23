using PagedList;
using Revenda.Classes;
using Revenda.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class SuppliersController : Controller
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

            report.Load(Server.MapPath("~/Reports/Suppliers.mrt"));

            return StiMvcViewer.GetReportResult(report);

        }

        private void SuppliersReport()
        {
            //ReportViewer reportViewer = new ReportViewer();
            //reportViewer.ProcessingMode = ProcessingMode.Local;
            //reportViewer.SizeToReportContent = true;
            //reportViewer.Width = Unit.Percentage(100);
            //reportViewer.Height = Unit.Percentage(100);
            //reportViewer.ZoomMode = ZoomMode.PageWidth;

            //FillDataSet();

            //reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\Suppliers.rdlc";
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
            return "SELECT Suppliers.SupplierId, Suppliers.FirstName AS Nome, Suppliers.LastName AS Sobrenome, Suppliers.BirthDate AS Nascimento," +
                    " Suppliers.SocialNumber AS CPF, Suppliers.Phone AS Telefone, Suppliers.Address AS Endereço, " +
                    " Suppliers.Complement AS Complemento, Suppliers.Neighborhood AS Bairro, Cities.Name AS Cidade, Estadoes.Name AS Estado " +
                    " FROM Suppliers INNER JOIN " +
                    "      Cities ON Suppliers.CityId = Cities.CityId INNER JOIN " +
                    "      Estadoes ON Suppliers.EstadoId = Estadoes.EstadoId AND Cities.EstadoId = Estadoes.EstadoId " +
                    " ORDER BY Suppliers.SupplierId";
        }

        //private ReportParameter[] GetParametersLocal()
        //{
        //    ReportParameter p1 = new ReportParameter("ReportTitle", "Fornecedores");
        //    return new ReportParameter[] { p1 };
        //}

        // GET: Suppliers
        public ActionResult Index(int? page = null)
        {
            page = (page ?? 1);

            var suppliers = db.Suppliers
                .Include(s => s.City).Include(s => s.Estado)
                .OrderBy(s => s.FirstName).ThenBy(s => s.LastName);
            return View(suppliers.ToPagedList((int)page, 5));
        }

        // GET: Suppliers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: Suppliers/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name");
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(), "CityId", "Name");
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Suppliers.Add(supplier);
                        var response = DBHelper.SaveChanges(db);
                        if (response.Succeeded)
                        {
                            if (supplier.PhotoFile != null)
                            {
                                var folder = "~/Content/Suppliers";
                                var file = string.Format("{0}.jpg", supplier.SupplierId);
                                var response1 = FileHelper.UploadPhoto(supplier.PhotoFile, folder, file);
                                if (response1)
                                {
                                    var pic = string.Format("{0}/{1}", folder, file);
                                    supplier.Photo = pic;
                                    db.Entry(supplier).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }

                            transaction.Commit();
                            return RedirectToAction("Index");
                        }
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }

            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", supplier.EstadoId);
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(supplier.EstadoId), "CityId", "Name", supplier.CityId);

            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(supplier.EstadoId), "CityId", "Name", supplier.CityId);
            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", supplier.EstadoId);
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = EntityState.Modified;
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    if (supplier.PhotoFile != null)
                    {
                        var folder = "~/Content/Suppliers";
                        var file = string.Format("{0}.jpg", supplier.SupplierId);
                        var response1 = FileHelper.UploadPhoto(supplier.PhotoFile, folder, file);
                        supplier.Photo = string.Format("{0}/{1}", folder, file);
                        db.Entry(supplier).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(supplier.EstadoId), "CityId", "Name", supplier.CityId);
            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", supplier.EstadoId);
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Supplier supplier = db.Suppliers.Find(id);
            db.Suppliers.Remove(supplier);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View(supplier);
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
