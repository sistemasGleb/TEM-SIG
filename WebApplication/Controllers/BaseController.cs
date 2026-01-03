using System.Web.Mvc;


namespace WebApplication.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            ViewBag.BaseUrl = System.Configuration.ConfigurationManager.AppSettings["BaseUrl"];
        }
        #region IP y direcciones de RED
        public string GetCustomerIP()
        {
            var CustomerIP = "";
            try
            {
                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                }
            }
            catch
            {

            }
            return CustomerIP;
        }
        #endregion
    }
}