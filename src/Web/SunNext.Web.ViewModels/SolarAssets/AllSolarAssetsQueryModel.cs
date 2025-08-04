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

        [Display(Name = "Asset Type")]
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
        public SolarAssetSorting SolarAssetSorting { get; set; }

        [Display(Name = "Items Per Page")]
        [Range(1, 100)]
        public int SolarAssetsPerPage { get; set; }

        [Range(1, int.MaxValue)]
        public int CurrentPage { get; set; }

        public int TotalSolarAssets { get; set; }

        public IEnumerable<SolarAssetListItemViewModel> SolarAssets { get; set; }
    }
}