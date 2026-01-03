using System;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class ExceptionController : Controller
    {
        // GET: Exception
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(string InnerexceptionMessage)
        {
            BusinessEntity.FormModels.FormException formException = new BusinessEntity.FormModels.FormException();

            try
            {
                if (string.IsNullOrEmpty(InnerexceptionMessage))
                    throw new Exception("IsNullOrEmpty");

                formException.inner_message = InnerexceptionMessage;

            }
            catch (Exception ex)
            {
                formException.inner_message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }

            return View(formException);
        }
    }
}