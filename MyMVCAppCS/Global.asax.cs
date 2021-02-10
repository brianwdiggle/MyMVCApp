
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyMVCAppCS
{
    using System;
    using System.Configuration;

    using MyMVCApp.Model;

    public class MvcApplication : HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

     
            routes.MapRoute(
                "HillsInclassification",
                "Walks/HillsInClassification/{id}/{page}",
                new { controller = "Walks", action = "HillsInclassification", page = UrlParameter.Optional });

            routes.MapRoute(
                "HillsByArea",
                "Walks/HillsByArea/{id}/{page}",
                new { controller = "Walks", action = "HillsByArea", page = UrlParameter.Optional });

            routes.MapRoute(
                "ProgressByClass",
                "Progress/Index/{classtype}",
                new { controller = "Progress", action = "Index", classtype = UrlParameter.Optional });

            routes.MapRoute(
                     "Default",
                     "{controller}/{action}/{id}",
                     new { controller = "Walks", action = "Index", id= UrlParameter.Optional });

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            // Clears all previously registered view engines.
            ViewEngines.Engines.Clear();

            // Register the razor view engine only
            ViewEngines.Engines.Add(new RazorViewEngine());
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // event is raised each time a new session is created     

            string usageLocation = System.Web.Configuration.WebConfigurationManager.AppSettings["atwork"];

            if (usageLocation.Equals("true"))
            {
                SessionSingleton.Current.UsageLocation = WalkingConstants.AT_WORK;
            }
            else
            {
                SessionSingleton.Current.UsageLocation = WalkingConstants.AT_HOME;
            }
 
            string dataTierTarget = System.Web.Configuration.WebConfigurationManager.AppSettings["DataTierTarget"];

            if (dataTierTarget.Equals(WalkingConstants.LIVE_DB_TIER) || dataTierTarget.Equals(WalkingConstants.TEST_DB_TIER))
            {
                SessionSingleton.Current.ConnectionString =
                    ConfigurationManager.ConnectionStrings[dataTierTarget].ConnectionString;

                SessionSingleton.Current.DataTierTarget = dataTierTarget;
            }
            else {
                throw new ConfigurationErrorsException(
                    "Value of appKey DataTierTarget must be either " + WalkingConstants.LIVE_DB_TIER + " or "
                    + WalkingConstants.TEST_DB_TIER);
            }

        }

    }
}
