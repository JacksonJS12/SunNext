using System.Collections.Generic;

namespace SunNext.Web.ViewModels.SolarAssets
{
    public class AllSolarAssetsFilteredAndPagedQueryModel
    {
        public AllSolarAssetsFilteredAndPagedQueryModel()
        {
            SolarAssets = new HashSet<AllSolarAssetsQueryModel>();
        }

        public int TotalSolarAssetsCount { get; set; }

        public IEnumerable<AllSolarAssetsQueryModel> SolarAssets { get; set; }
    }
}