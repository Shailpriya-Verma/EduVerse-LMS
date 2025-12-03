using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EduVerse
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //// Enable CORS globally
            //var cors = new EnableCorsAttribute("https://localhost:44349", "*", "*");
            //config.EnableCors(cors);

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
