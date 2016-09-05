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
            var configViewModel = new ConfigViewModel();
            return View(configViewModel);
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
