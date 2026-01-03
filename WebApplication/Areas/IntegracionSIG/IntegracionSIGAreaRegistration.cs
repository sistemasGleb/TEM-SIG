using System.Web.Mvc;

namespace WebApplication.Areas.IntegracionSIG
{
    public class IntegracionSIGAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "IntegracionSIG";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "IntegracionSIG_default",
                "IntegracionSIG/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}