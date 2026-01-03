using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}