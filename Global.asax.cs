using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Task_Management_System2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();

            HttpException httpException = exception as HttpException;
            if (httpException != null)
            {
                RouteData routeData = new RouteData();
                routeData.Values.Add("controller", "Error");
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // page not found
                        routeData.Values.Add("action", "HttpError404");
                        break;
                    case 500:
                        // server error
                        routeData.Values.Add("action", "HttpError500");
                        break;
                    default:
                        routeData.Values.Add("action", "General");
                        break;
                }
                routeData.Values.Add("error", exception);
                // clear error on server
                Server.ClearError();

                // at this point how to properly pass route data to error controller?
            }
        }
    }
}
