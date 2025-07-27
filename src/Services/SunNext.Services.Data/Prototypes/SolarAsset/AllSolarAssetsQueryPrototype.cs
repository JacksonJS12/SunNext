using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SunNext.Common.Enums;
using static SunNext.Common.GlobalConstants;
namespace SunNext.Services.Data.Prototypes.SolarAsset
{
    public class AllSolarAssetsQueryPrototype
    {
        public AllSolarAssetsQueryPrototype()
        {
            SolarAssets = new HashSet<SolarAssetListItemPrototype>();
            SolarAssetTypes = new HashSet<string>();
            CurrentPage = DefaultPage;
            SolarAssetsPerPage = EntitiesPerPage;
        }

        [Display(Name = "Search by Name or Location")]
        public string? SearchString { get; set; }

        [Display(Name = "Solar Asset Type")]
        public string? SolarAssetType { get; set; }

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

        public IEnumerable<SolarAssetListItemPrototype> SolarAssets { get; set; }


        public IEnumerable<string> SolarAssetTypes { get; set; }
    }
}