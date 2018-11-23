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
    public class CustomersController : Controller
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

            report.Load(Server.MapPath("~/Reports/Customers.mrt"));

            return StiMvcViewer.GetReportResult(report);

        }

        private void CustomersReport()
        {
            //ReportViewer reportViewer = new ReportViewer();
            //reportViewer.ProcessingMode = ProcessingMode.Local;
            //reportViewer.SizeToReportContent = true;
            //reportViewer.Width = Unit.Percentage(100);
            //reportViewer.Height = Unit.Percentage(100);
            //reportViewer.ZoomMode = ZoomMode.PageWidth;

            //FillDataSet();

            //reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\Customers.rdlc";
            //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", tds.Tables[0]));
            //reportViewer.LocalReport.SetParameters(GetParametersLocal());

            //ViewBag.ReportViewer = reportViewer;
        }

        private void FillDataSet()
        {

        //    string connectionString = GetConnectionString();

        //    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        //    {
        //        string queryString = GetQueryString();

        //        SqlDataAdapter sqlDataAapter = new SqlDataAdapter(queryString, sqlConnection);

        //        sqlDataAapter.Fill(tds, tds.DataTable1.TableName);
        //    }
        }

        //private string GetConnectionString()
        //{
        //    return "Data Source=.;Initial Catalog=Revenda;Integrated Security=True";
        //}

        //private string GetQueryString()
        //{
        //    return  "SELECT Customers.Photo AS Foto, Customers.FirstName AS Nome, Customers.LastName AS Sobrenome," +
        //            " Customers.BirthDate AS [Nascimento], Customers.SocialNumber AS [CPF], " +
        //            " Customers.Phone AS Telefone, Customers.Address AS Endereço, Customers.Complement AS Complemento, " +
        //            " Customers.Neighborhood AS Bairro, Customers.ZipCode AS CEP, Cities.Name AS Cidade, Estadoes.Name AS Estado " +
        //            " FROM Customers INNER JOIN " +
        //            " Cities ON Customers.CityId = Cities.CityId INNER JOIN " +
        //            " Estadoes ON Customers.EstadoId = Estadoes.EstadoId AND Cities.EstadoId = Estadoes.EstadoId " +
        //            " ORDER BY Nome, Sobrenome ";
        //}

        //private ReportParameter[] GetParametersLocal()
        //{
        //    ReportParameter p1 = new ReportParameter("ReportTitle", "Lista de Cidades");
        //    return new ReportParameter[] { p1 };
        //}

        // GET: Customers
        public ActionResult Index(int? page = null)
        {
            //var customers = db.Customers.Include(c => c.City).Include(c => c.Company).Include(c => c.Department);

            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            page = (page ?? 1);

            var customers = db.Customers
                .Include(c => c.City)
                .Include(c => c.Estado)
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName);

            return View(customers.ToPagedList((int)page, 5));
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name");
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(), "CityId", "Name");

            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Customers.Add(customer);
                        var response = DBHelper.SaveChanges(db);
                        if (response.Succeeded)
                        {
                            if (customer.PhotoFile != null)
                            {
                                var folder = "~/Content/Customers";
                                var file = string.Format("{0}.jpg", customer.CustomerId);
                                var response1 = FileHelper.UploadPhoto(customer.PhotoFile, folder, file);
                                if (response1)
                                {
                                    var pic = string.Format("{0}/{1}", folder, file);
                                    customer.Photo = pic;
                                    db.Entry(customer).State = EntityState.Modified;
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

            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", customer.EstadoId);
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(customer.EstadoId), "CityId", "Name", customer.CityId);

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", customer.EstadoId);
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(customer.EstadoId), "CityId", "Name", customer.CityId);

            return View(customer);

        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    if (customer.PhotoFile != null)
                    {
                        var folder = "~/Content/Customers";
                        var file = string.Format("{0}.jpg", customer.CustomerId);
                        var response1 = FileHelper.UploadPhoto(customer.PhotoFile, folder, file);
                        customer.Photo = string.Format("{0}/{1}", folder, file);
                        db.Entry(customer).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }
            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", customer.EstadoId);
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(customer.EstadoId), "CityId", "Name", customer.CityId);

            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            db.Customers.Remove(customer);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, response.Message);
            return View(customer);
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
