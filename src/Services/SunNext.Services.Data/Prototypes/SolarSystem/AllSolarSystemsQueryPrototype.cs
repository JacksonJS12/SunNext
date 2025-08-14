using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SunNext.Common.Enums;
using static SunNext.Common.GlobalConstants;

namespace SunNext.Services.Data.Prototypes.SolarSystem
{
    public class AllSolarSystemsQueryPrototype
    {
        public AllSolarSystemsQueryPrototype()
        {
            SolarSystems = new HashSet<SolarSystemListItemPrototype>();
            SolarSystemTypes = new HashSet<string>();
            CurrentPage = DefaultPage;
            SolarSystemsPerPage = EntitiesPerPage;
        }

        [Display(Name = "Search by Name or Location")]
        public string? SearchString { get; set; }

        [Display(Name = "Solar System Type")]
        public string? SolarSystemType { get; set; }

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

        public IEnumerable<SolarSystemListItemPrototype> SolarSystems { get; set; }

        public IEnumerable<string> SolarSystemTypes { get; set; }
    }
}