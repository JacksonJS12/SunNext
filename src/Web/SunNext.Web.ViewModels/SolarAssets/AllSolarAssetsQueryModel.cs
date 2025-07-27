using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SunNext.Common.Enums;
using SunNext.Data.Common.Models;
using static SunNext.Common.GlobalConstants;
namespace SunNext.Web.ViewModels.SolarAssets
{
    public class AllSolarAssetsQueryModel : BaseDeletableModel<string>
    {
        public AllSolarAssetsQueryModel()
        {
            SolarAssets = new HashSet<SolarAssetListItemViewModel>();
            CurrentPage = DefaultPage;
            SolarAssetsPerPage = EntitiesPerPage;
        }
        public string Type { get; set; } 

        [Display(Name = "Search by Name or Location")]
        public string? SearchString { get; set; }

        [Display(Name = "Installation Date From")]
        public DateTime? InstallationDateFrom { get; set; }

        [Display(Name = "Installation Date To")]
        public DateTime? InstallationDateTo { get; set; }

        [Display(Name = "Sort By")]
        public SolarAssetSorting SolarAssetSorting { get; set; }

        [Display(Name = "Items Per Page")]
        public int SolarAssetsPerPage { get; set; }

        public int CurrentPage { get; set; }

        public int TotalSolarAssets { get; set; }

        public IEnumerable<SolarAssetListItemViewModel> SolarAssets { get; set; }

    }
}