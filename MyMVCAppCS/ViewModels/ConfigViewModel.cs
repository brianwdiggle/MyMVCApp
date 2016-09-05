
namespace MyMVCAppCS.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using MyMVCApp.DAL;
    using MyMVCApp.Model;

    public class ConfigViewModel
    {
        // Backing variables for the configuration options
        private readonly List<AtHomeOption> atWorkOptions;
        private readonly List<DataTierTargetOption> dataTierTargetOptions; 
 
        /// <summary>
        /// Default constructor:
        /// Instatiate public properties which make up the configuration options avaialable
        /// </summary>
        public ConfigViewModel()
        {
            atWorkOptions = new List<AtHomeOption>
                            {
                                new AtHomeOption { Id = WalkingConstants.AT_HOME, Name = WalkingConstants.AT_HOME },
                                new AtHomeOption { Id = WalkingConstants.AT_WORK, Name = WalkingConstants.AT_WORK }
                            };

            dataTierTargetOptions = new List<DataTierTargetOption>
                                    {
                                        new DataTierTargetOption {Id = WalkingConstants.LIVE_DB_TIER, Name = WalkingConstants.LIVE_DB_TIER},
                                        new DataTierTargetOption {Id = WalkingConstants.TEST_DB_TIER, Name = WalkingConstants.TEST_DB_TIER}
                                    };

            ConfigUpdateMessage = "";
        }

        /// <summary>
        /// Selected value of at work setting
        /// </summary>
        [Display(Name = "Usage Location")]
        public string AtWorkSetting { get; set; }

        /// <summary>
        /// Selected value of data tier target
        /// </summary>
        [Display(Name = "Data Tier Target")]
        public string DataTierTargetSetting { get; set; }

        /// <summary>
        /// Initial drop down list set with selected value of current usage location which is defined in the session
        /// </summary>
        public IEnumerable<SelectListItem> UsageLocationOptions
        {
            get
            {
                return new SelectList(atWorkOptions, "Id", "Name", SessionSingleton.Current.UsageLocation);
            }
        }


        /// <summary>
        /// Initial drop down list set with selected value of data tier target which is defined in the session
        /// </summary>
        public IEnumerable<SelectListItem> DataTierSelectionOptions
        {
            get
            {
                return new SelectList(dataTierTargetOptions, "Id", "Name", SessionSingleton.Current.DataTierTarget);
            }
        }

        public string ConfigUpdateMessage { get; set; }

    }

    public class AtHomeOption
    {
        public string Id {get; set;}
        public string Name { get; set; }
    }

    public class DataTierTargetOption
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}