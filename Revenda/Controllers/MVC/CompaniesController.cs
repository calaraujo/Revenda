using PagedList;
using Revenda.Classes;
using Revenda.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class CompaniesController : Controller
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

            report.Load(Server.MapPath("~/Reports/Companies.mrt"));

            return StiMvcViewer.GetReportResult(report);

        }

        private void CompaniesReport()
        {
            //ReportViewer reportViewer = new ReportViewer();
            //reportViewer.ProcessingMode = ProcessingMode.Local;
            //reportViewer.SizeToReportContent = true;
            //reportViewer.Width = Unit.Percentage(100);
            //reportViewer.Height = Unit.Percentage(100);
            //reportViewer.ZoomMode = ZoomMode.PageWidth;

            //FillDataSet();

            //reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\Companies.rdlc";
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
            return "SELECT Companies.Name AS Nome, Companies.Phone AS Telefone, Companies.Address AS Endereço, Companies.Complement AS Complemento, " +
                    "  Companies.Neighborhood AS Bairro, Cities.Name AS Cidade, Estadoes.Name AS Estado " +
                    "  FROM  Companies INNER JOIN " +
                    "        Estadoes ON Companies.EstadoId = Estadoes.EstadoId INNER JOIN " +
                    "        Cities ON Companies.CityId = Cities.CityId AND Estadoes.EstadoId = Cities.EstadoId " +
                    "  ORDER BY Companies.CompanyId";
        }

        //private ReportParameter[] GetParametersLocal()
        //{
        //    ReportParameter p1 = new ReportParameter("ReportTitle", "Lista de Empresas");
        //    return new ReportParameter[] { p1 };
        //}

        // GET: Companies
        public ActionResult Index(int? page = null)
        {
            page = (page ?? 1);
            var companies = db.Companies.Include(c => c.City).Include(c => c.Estado)
                .OrderBy(c => c.Estado.Name).ThenBy(c => c.City.Name);
            return View(companies.ToPagedList((int)page, 3));

        }

        // GET: Companies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(), "CityId", "Name");
            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name");
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Company company)
        {
            if (ModelState.IsValid)
            {
                db.Companies.Add(company);
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    if (company.LogoFile != null)
                    {
                        var folder = "~/Content/Logos";
                        var file = string.Format("{0}.jpg", company.CompanyId);
                        var response1 = FileHelper.UploadPhoto(company.LogoFile, folder, file);
                        if (response1)
                        {
                            var pic = string.Format("{0}/{1}", folder, file);
                            company.Logo = pic;
                            db.Entry(company).State = EntityState.Modified;
                            //db.SaveChanges();
                            var response2 = DBHelper.SaveChanges(db);
                            if (!response2.Succeeded)
                            {
                                ModelState.AddModelError(string.Empty, response.Message);
                            }
                        }
                    }
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }

            ViewBag.CityId = new SelectList(CombosHelper.GetCities(company.EstadoId), "CityId", "Name", company.CityId);
            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", company.EstadoId);
            return View(company);
        }

        // GET: Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(company.EstadoId), "CityId", "Name", company.CityId);
            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", company.EstadoId);
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompanyId,Name,Phone,Address,Complement,Neighborhood,EstadoId,CityId,Logo")] Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.LogoFile != null)
                {
                    var pic = string.Empty;
                    var folder = "~/Content/Logos";
                    var file = string.Format("{0}.jpg", company.CompanyId);
                    var response1 = FileHelper.UploadPhoto(company.LogoFile, folder, file);
                    if (response1)
                    {
                        pic = string.Format("{0}/{1}", folder, file);
                        company.Logo = pic;
                    }
                }

                db.Entry(company).State = EntityState.Modified;
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }

            ViewBag.CityId = new SelectList(CombosHelper.GetCities(company.EstadoId), "CityId", "Name", company.CityId);
            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", company.EstadoId);
            return View(company);
        }

        // GET: Companies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Companies.Find(id);
            db.Companies.Remove(company);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, response.Message);
            return View(company);
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
