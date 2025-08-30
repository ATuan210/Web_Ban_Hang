using System.Web.Mvc;

namespace LapTrinhWeb.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
            "Admin_area_default", // tên mới, không trùng nữa
            "Admin/{controller}/{action}/{id}",
            new { action = "Index", id = UrlParameter.Optional }
);
        }
    }
}