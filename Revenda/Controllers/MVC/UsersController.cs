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
    public class UsersController : Controller
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

            report.Load(Server.MapPath("~/Reports/Users.mrt"));

            return StiMvcViewer.GetReportResult(report);

        }

        private void UsersReport()
        {
            //ReportViewer reportViewer = new ReportViewer();
            //reportViewer.ProcessingMode = ProcessingMode.Local;
            //reportViewer.SizeToReportContent = true;
            //reportViewer.Width = Unit.Percentage(100);
            //reportViewer.Height = Unit.Percentage(100);
            //reportViewer.ZoomMode = ZoomMode.PageWidth;

            //FillDataSet();

            //reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\Users.rdlc";
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
            return "SELECT Users.UserId AS Código, Users.UserName AS Usuário, Users.FirstName AS Nome, " +
                    "       Users.LastName AS Sobrenome, Users.Phone AS Telefone, Users.Address AS Endereço, Users.Complement AS Complemento, " +
                    "       Users.Neighborhood AS Bairro, Companies.Name AS Empresa, Cities.Name AS Cidade, Estadoes.Name AS Estado " +
                    " FROM   Users, Companies, Cities, Estadoes" +
                    " WHERE  Users.CompanyId = Companies.CompanyId" +
                    " AND    Users.CityId = Cities.CityId" +
                    " AND    Users.EstadoId = Estadoes.EstadoId" +
                    " ORDER BY Empresa, Código";
        }

        //private ReportParameter[] GetParametersLocal()
        //{
        //    ReportParameter p1 = new ReportParameter("ReportTitle", "Usuários");
        //    return new ReportParameter[] { p1 };
        //}

        // GET: Users
        public ActionResult Index(int? page = null)
        {
            page = (page ?? 1);
            var users = db.Users.Include(u => u.City).Include(u => u.Company).Include(u => u.Estado)
                .OrderBy(u => u.CompanyId).ThenBy(u => u.UserName);
            return View(users.ToPagedList((int)page, 4));
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(), "CityId", "Name");
            ViewBag.CompanyId = new SelectList(CombosHelper.GetCompanies(), "CompanyId", "Name");
            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    UsersHelper.CreateUserASP(user.UserName, "User");
                    
                    if (user.PhotoFile != null)
                    {
                        var folder = "~/Content/Users";
                        var file = string.Format("{0}.jpg", user.UserId);
                        var response1 = FileHelper.UploadPhoto(user.PhotoFile, folder, file);
                        if (response1)
                        {
                            var pic = string.Format("{0}/{1}", folder, file);
                            user.Photo = pic;
                            db.Entry(user).State = EntityState.Modified;
                            
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
            ViewBag.CityID = new SelectList(CombosHelper.GetCities(user.EstadoId), "CityId", "Name", user.CityId);
            ViewBag.CompanyID = new SelectList(CombosHelper.GetCompanies(), "CompanyId", "Name", user.CompanyId);
            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", user.EstadoId);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(user.EstadoId), "CityId", "Name", user.CityId);
            ViewBag.CompanyId = new SelectList(CombosHelper.GetCompanies(), "CompanyId", "Name", user.CompanyId);
            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", user.EstadoId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                if (user.PhotoFile != null)
                {
                    var file = string.Format("{0}.jpg", user.UserId);
                    var folder = "~/Content/Users";
                    var response1 = FileHelper.UploadPhoto(user.PhotoFile, folder, file);
                    user.Photo = string.Format("{0}/{1}", folder, file);
                }

                // Como estamos editando o usuario e possivel que o email tenha sido alterado. Para saber isto e
                // efetuarmos as atualizações devemos recuperar o usuario anterior. Para isto definir um novo 
                // contexto de dados e recuperar o usuario. Fazer as comparacoes. Se forem diferente atualizar o 
                // o usuario e seu email e fechar a conexão temporária criada para acessar o BD.

                var db2 = new RevendaContext();
                var currentUser = db2.Users.Find(user.UserId);
                if (currentUser.UserName != user.UserName)
                {
                    UsersHelper.UpdateUserName(currentUser.UserName, user.UserName);
                }
                db2.Dispose();

                db.Entry(user).State = EntityState.Modified;
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);

            }
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(user.EstadoId), "CityId", "Name", user.CityId);
            ViewBag.CompanyId = new SelectList(CombosHelper.GetCompanies(), "CompanyId", "Name", user.CompanyId);
            ViewBag.EstadoId = new SelectList(CombosHelper.GetEstadoes(), "EstadoId", "Name", user.EstadoId);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
                UsersHelper.DeleteUser(user.UserName, "User");
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, response.Message);
            return View(user);
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
