using System.Collections.Generic;

namespace SunNext.Services.Data.Prototypes.SolarSystem
{
    public class AllSolarSystemsFilteredAndPagedPrototype
    {
        public AllSolarSystemsFilteredAndPagedPrototype()
        {
            SolarSystems = new HashSet<SolarSystemListItemPrototype>();
        }

        public int TotalSolarSystemsCount { get; set; }

        public IEnumerable<SolarSystemListItemPrototype> SolarSystems { get; set; }
    }
}