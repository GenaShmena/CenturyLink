using System.Web.Http;
using ServerTrack.Filters;

namespace ServerTrack
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //Register custom validation filters
            config.Filters.Add(new ValidateModelAttribute());
            config.Filters.Add(new ValidateModelNullAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
