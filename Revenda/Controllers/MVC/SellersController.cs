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
    public class SellersController : Controller
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

            report.Load(Server.MapPath("~/Reports/Sellers.mrt"));

            return StiMvcViewer.GetReportResult(report);

        }

        private void SellersReport()
        {
            //ReportViewer reportViewer = new ReportViewer();
            //reportViewer.ProcessingMode = ProcessingMode.Local;
            //reportViewer.SizeToReportContent = true;
            //reportViewer.Width = Unit.Percentage(100);
            //reportViewer.Height = Unit.Percentage(100);
            //reportViewer.ZoomMode = ZoomMode.PageWidth;

            //FillDataSet();

            //reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\Sellers.rdlc";
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
            return "SELECT Sellers.SellerId AS Código, Sellers.FirstName AS Nome, Sellers.LastName AS Sobrenome, Sellers.BirthDate AS Nascimento, " +
                    " Sellers.SocialNumber AS CPF, Sellers.Phone AS Telefone, Sellers.Address AS Endereço, " +
                    " Sellers.Complement AS Compl, Sellers.Neighborhood AS Bairro, Sellers.ZipCode AS CEP, Cities.Name AS Cidade, Estadoes.Name AS Estado" +
                    " FROM Sellers INNER JOIN" +
                    "      Cities ON Sellers.CityId = Cities.CityId INNER JOIN" +
                    "      Estadoes ON Sellers.EstadoId = Estadoes.EstadoId AND Cities.EstadoId = Estadoes.EstadoId" +
                    " ORDER BY Código";
        }

        //private ReportParameter[] GetParametersLocal()
        //{
        //    ReportParameter p1 = new ReportParameter("ReportTitle", "Vendedores");
        //    return new ReportParameter[] { p1 };
        //}

        // GET: Sellers
        public ActionResult Index(int? page = null)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            page = (page ?? 1);

            var sellers = db.Sellers
                .Include(c => c.City)
                .Include(c => c.Estado)
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName);

            return View(sellers.ToPagedList((int)page, 5));
        }

        // GET: Sellers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seller seller = db.Sellers.Find(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            return View(seller);
        }

        // GET: Sellers/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name");
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(), "CityId", "Name");

            return View();
        }

        // POST: Sellers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Seller seller)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Sellers.Add(seller);
                        var response = DBHelper.SaveChanges(db);
                        if (response.Succeeded)
                        {
                            if (seller.PhotoFile != null)
                            {
                                var folder = "~/Content/Sellers";
                                var file = string.Format("{0}.jpg", seller.SellerId);
                                var response1 = FileHelper.UploadPhoto(seller.PhotoFile, folder, file);
                                if (response1)
                                {
                                    var pic = string.Format("{0}/{1}", folder, file);
                                    seller.Photo = pic;
                                    db.Entry(seller).State = EntityState.Modified;
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

            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", seller.EstadoId);
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(seller.EstadoId), "CityId", "Name", seller.CityId);

            return View(seller);
        }

        // GET: Sellers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seller seller = db.Sellers.Find(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", seller.EstadoId);
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(seller.EstadoId), "CityId", "Name", seller.CityId);

            return View(seller);

        }

        // POST: Sellers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Seller seller)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seller).State = EntityState.Modified;
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    if (seller.PhotoFile != null)
                    {
                        var folder = "~/Content/Sellers";
                        var file = string.Format("{0}.jpg", seller.SellerId);
                        var response1 = FileHelper.UploadPhoto(seller.PhotoFile, folder, file);
                        seller.Photo = string.Format("{0}/{1}", folder, file);
                        db.Entry(seller).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }
            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", seller.EstadoId);
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(seller.EstadoId), "CityId", "Name", seller.CityId);

            return View(seller);
        }

        // GET: Sellers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seller seller = db.Sellers.Find(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            return View(seller);
        }

        // POST: Sellers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Seller seller = db.Sellers.Find(id);
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            db.Sellers.Remove(seller);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, response.Message);
            return View(seller);
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
