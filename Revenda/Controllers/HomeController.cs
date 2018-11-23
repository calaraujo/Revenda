using Revenda.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Revenda.Controllers
{
    public class HomeController : Controller
    {
        private RevendaContext db = new RevendaContext();
        private RevendaEntities context = new RevendaEntities();

        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            return View(user);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}