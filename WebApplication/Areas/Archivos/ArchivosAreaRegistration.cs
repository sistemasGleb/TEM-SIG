using System.Web.Mvc;

namespace WebApplication.Areas.Archivos
{
    public class ArchivosAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Archivos";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Archivos_default",
                "Archivos/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}