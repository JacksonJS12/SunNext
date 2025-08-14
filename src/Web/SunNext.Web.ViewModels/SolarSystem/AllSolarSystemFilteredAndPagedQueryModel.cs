using System.Collections.Generic;

namespace SunNext.Web.ViewModels.SolarSystem
{
    public class AllSolarSystemFilteredAndPagedQueryModel
    {
        public AllSolarSystemFilteredAndPagedQueryModel()
        {
            SolarSystems = new HashSet<AllSolarSystemQueryModel>();
        }

        public int TotalSolarSystemCount { get; set; }

        public IEnumerable<AllSolarSystemQueryModel> SolarSystems { get; set; }
    }
}