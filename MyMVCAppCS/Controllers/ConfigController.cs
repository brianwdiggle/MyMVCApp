using System.Web.Mvc;

namespace MyMVCAppCS.Controllers
{
    using System.Configuration;

    using MyMVCAppCS.ViewModels;

    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    [HandleError]
    public class ConfigController : Controller
    {

        public ActionResult Index()
        {
            if (SessionSingleton.Current.DataTierTarget.Equals(MyMVCApp.Model.WalkingConstants.LIVE_DB_TIER))
            {
                SessionSingleton.Current.DataTierTarget = MyMVCApp.Model.WalkingConstants.TEST_DB_TIER;
                SessionSingleton.Current.ConnectionString = ConfigurationManager.ConnectionStrings[MyMVCApp.Model.WalkingConstants.TEST_DB_TIER].ConnectionString;
            }
            else
            {
                SessionSingleton.Current.DataTierTarget = MyMVCApp.Model.WalkingConstants.LIVE_DB_TIER;
                SessionSingleton.Current.ConnectionString = ConfigurationManager.ConnectionStrings[MyMVCApp.Model.WalkingConstants.LIVE_DB_TIER].ConnectionString;
            }

            return Redirect(Request.UrlReferrer.ToString());

            //---2021: Removed form to enable change and replaced with data tier switch logic above, followed by redirect to referring page.
            //-- So the POST method is defunct, but left in place in case I need the "usage location" in future.
            //var configViewModel = new ConfigViewModel();
            //return View(configViewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(ConfigViewModel configModel)
        {
            //Update the session configuration with the values found in the config view model.
            SessionSingleton.Current.UsageLocation = configModel.AtWorkSetting;
            SessionSingleton.Current.DataTierTarget = configModel.DataTierTargetSetting;
            SessionSingleton.Current.ConnectionString  =ConfigurationManager.ConnectionStrings[configModel.DataTierTargetSetting].ConnectionString;
      
            configModel.ConfigUpdateMessage = "Usage Location has been updated to [" + configModel.AtWorkSetting + "]</br/>" + 
                                              "Data Tier target location is now ["+ configModel.DataTierTargetSetting + "]";

            return View(configModel);
        }
    }
}
