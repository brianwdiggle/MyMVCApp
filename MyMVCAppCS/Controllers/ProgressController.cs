using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyMVCAppCS.Controllers
{
    using MyMVCApp.DAL;

    using MyMVCAppCS.Models;

    public class ProgressController : Controller
    {
        private IWalkingRepository repository;

        public ProgressController()
        {
            this.repository = new SqlWalkingRepository(SessionSingleton.Current.ConnectionString);
        }

        //
        // GET: /Progress/

        public ActionResult Index(string classType = "f")
        {
            char classTypeChar = classType.First();

            List<MyProgress> oProgress = this.repository.GetMyProgressByClassType(classTypeChar);
            ViewData["oProgress"] = oProgress;
            return View();
        }

    }
}
