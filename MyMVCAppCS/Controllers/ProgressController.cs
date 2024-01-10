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

        /// <summary>
        /// Get progress by class type
        /// Class type of "F" (or "f") is a special class type "favourite" which enables me to define a set of favourite class types.
        /// Note - if adding new classes to the "F" type, make sure that SortSeq is not a duplicate
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>

        public ActionResult Index(string classType = "f")
        {
            char classTypeChar = classType.First();

            
            List<MyProgress> oProgress = this.repository.GetMyProgressByClassType(classTypeChar);
            ViewData["oProgress"] = oProgress;
            return View();
        }

    }
}
