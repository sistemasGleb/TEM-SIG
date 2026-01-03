using System.Web.Mvc;

namespace WebApplication.Areas.Mapa
{
    public class MapaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Mapa";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Mapa_default",
                "Mapa/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}