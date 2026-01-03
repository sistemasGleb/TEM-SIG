using System.Web.Mvc;

namespace WebApplication.Areas.PlanDeMantencion
{
    public class PlanDeMantencionAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PlanDeMantencion";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PlanDeMantencion_default",
                "PlanDeMantencion/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}