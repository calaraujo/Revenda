using Revenda.Classes;
using Revenda.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class ProductsController : Controller
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

            report.Load(Server.MapPath("~/Reports/Products.mrt"));

            return StiMvcViewer.GetReportResult(report);

        }

        private void ProductsReport()
        {
            //ReportViewer reportViewer = new ReportViewer();
            //reportViewer.ProcessingMode = ProcessingMode.Local;
            //reportViewer.SizeToReportContent = true;
            //reportViewer.Width = Unit.Percentage(100);
            //reportViewer.Height = Unit.Percentage(100);
            //reportViewer.ZoomMode = ZoomMode.PageWidth;

            //FillDataSet();

            //reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\Products.rdlc";
            //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", tds.Tables[0]));
            //reportViewer.LocalReport.SetParameters(GetParametersLocal());

            //ViewBag.ReportViewer = reportViewer;
        }

        //private void FillDataSet()
        //{

        //    string connectionString = GetConnectionString();

        //    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        //    {
        //        string queryString = GetQueryString();

        //        SqlDataAdapter sqlDataAapter = new SqlDataAdapter(queryString, sqlConnection);

        //        sqlDataAapter.Fill(tds, tds.DataTable1.TableName);
        //    }
        //}

        //private string GetConnectionString()
        //{
        //    return "Data Source=.;Initial Catalog=Revenda;Integrated Security=True";
        //}

        //private string GetQueryString()
        //{
        //    return "SELECT Products.ProductCode AS CodForn, Products.Description AS Descrição, Products.Price AS Preço, Companies.Name AS Empresa," +
        //            " Categories.Description AS Categoria " +
        //            " FROM Products INNER JOIN" +
        //            "      Companies ON Products.CompanyId = Companies.CompanyId INNER JOIN" +
        //            "      Categories ON Products.CategoryId = Categories.CategoryId AND Companies.CompanyId = Categories.CompanyId" +
        //            " ORDER BY Categoria, Products.ProductId";
        //}

        //private ReportParameter[] GetParametersLocal()
        //{
        //    ReportParameter p1 = new ReportParameter("ReportTitle", "Produtos");
        //    return new ReportParameter[] { p1 };
        //}

        // GET: Products
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "ProductCode" : "";
            //ViewBag.CurrentSort = sortOrder;
            //if (myInput != null)
            //{
            //    page = 1;
            //}
            //else
            //{
            //    myInput = currentFilter;
            //}

            //ViewBag.CurrentFilter = myInput;
            //page = (page ?? 1);
            var products = db.Products
                    .Include(p => p.Category)
                    .Where(p => p.CompanyId == user.CompanyId)
                    .OrderBy(p => p.ProductCode).ToList();
            return View(products);
                    //.ToPagedList((int)page, 4));
        } 

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(user.CompanyId), "CategoryId", "Description");

            var product = new Product { CompanyId = user.CompanyId, };
            return View(product);
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    if (product.ImageFile != null)
                    {
                        var folder = "~/Content/Products";
                        var file = string.Format("{0}.jpg", product.ProductId);
                        var response1 = FileHelper.UploadPhoto(product.ImageFile, folder, file);
                        if (response1)
                        {
                            var pic = string.Format("{0}/{1}", folder, file);
                            product.Image = pic;
                            db.Entry(product).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);

            }

            ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(user.CompanyId), "CategoryId", "Description", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(product.CompanyId), "CategoryId", "Description", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    var file = string.Format("{0}.jpg", product.ProductId);
                    var folder = "~/Content/Products";
                    var response1 = FileHelper.UploadPhoto(product.ImageFile, folder, file);
                    product.Image = string.Format("{0}/{1}", folder, file);
                }

                db.Entry(product).State = EntityState.Modified;
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);

            }
            ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(product.CompanyId), "CategoryId", "Description", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, response.Message);
            return View(product);
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
