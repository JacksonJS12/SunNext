using System;

namespace SunNext.Services.Data.Prototypes.SolarAsset
{
    public class SolarAssetListItemPrototype
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string? Location { get; set; }

        public double PowerKw { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsOnline { get; set; }

        public bool IsDeleted { get; set; }
    }
}