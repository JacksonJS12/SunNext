using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SunNext.Common.Enums;
using SunNext.Data.Common.Models;
using static SunNext.Common.GlobalConstants;

namespace SunNext.Web.ViewModels.SolarSystem
{
    public class AllSolarSystemQueryModel : BaseDeletableModel<string>
    {
        public AllSolarSystemQueryModel()
        {
            SolarSystems = new HashSet<SolarSystemListItemViewModel>();
            CurrentPage = DefaultPage;
            SolarSystemsPerPage = EntitiesPerPage;
        }

        [Display(Name = "System Type")]
        public string Type { get; set; } = null!;

        [Display(Name = "Search by Name or Location")]
        public string? SearchString { get; set; }

        [Display(Name = "Installation Date From")]
        [DataType(DataType.Date)]
        public DateTime? InstallationDateFrom { get; set; }

        [Display(Name = "Installation Date To")]
        [DataType(DataType.Date)]
        public DateTime? InstallationDateTo { get; set; }

        [Display(Name = "Sort By")]
        public SolarSystemSorting SolarSystemSorting { get; set; }

        [Display(Name = "Items Per Page")]
        [Range(1, 100)]
        public int SolarSystemsPerPage { get; set; }

        [Range(1, int.MaxValue)]
        public int CurrentPage { get; set; }

        public int TotalSolarSystems { get; set; }

        public IEnumerable<SolarSystemListItemViewModel> SolarSystems { get; set; }
    }
}