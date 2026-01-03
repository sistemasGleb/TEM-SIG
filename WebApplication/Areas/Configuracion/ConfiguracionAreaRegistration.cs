using System.Web.Mvc;

namespace WebApplication.Areas.Configuracion
{
    public class MapaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Configuracion";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Configuracion_default",
                "Configuracion/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}