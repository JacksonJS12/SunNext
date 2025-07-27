using System.Collections.Generic;

namespace SunNext.Services.Data.Prototypes.SolarAsset
{
    public class AllSolarAssetsFilteredAndPagedPrototype
    {
        public AllSolarAssetsFilteredAndPagedPrototype()
        {
            SolarAssets = new HashSet<SolarAssetListItemPrototype>();
        }

        public int TotalSolarAssetsCount { get; set; }

        public IEnumerable<SolarAssetListItemPrototype> SolarAssets { get; set; }
    }
}