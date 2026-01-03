using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class ImageController : BaseController
    {
        // GET: Image
        [Authorize]
        public ActionResult UserImage(string id)
        {
            var relativePath = $"~/content/img/placeholders/{id}.png";
            var absolutePath = HttpContext.Server.MapPath(relativePath);

            if (System.IO.File.Exists(absolutePath))
                return File(absolutePath, "image/png");
            else
                return File($"~/content/img/placeholders/user.png", "image/png");
        }

        public ActionResult MiscelaneusImage(string id)
        {
            var relativePath = $"~/content/img/miscelaneous/{id}";
            var absolutePath = HttpContext.Server.MapPath(relativePath);

            if (System.IO.File.Exists(absolutePath))
                return File(absolutePath, "image/png");
            else
                return null;
        }
    }
}