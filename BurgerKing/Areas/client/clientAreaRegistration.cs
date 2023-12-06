using System.Web.Mvc;

namespace BurgerKing.Areas.client
{
    public class clientAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "client";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "client_default",
                "client/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}