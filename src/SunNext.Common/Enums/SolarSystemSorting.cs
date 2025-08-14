using System.ComponentModel.DataAnnotations;

namespace SunNext.Common.Enums;

public enum SolarSystemSorting
{
    [Display(Name = "Newest First")]
    Newest = 0,
    [Display(Name = "Oldest First")]
    Oldest = 1,
    [Display(Name = "Power Ascending")]
    PowerAscending = 2,
    [Display(Name = "Power Descending")]
    PowerDescending = 3,
    [Display(Name = "Name A-Z")]
    NameAscending = 4,
    [Display(Name = "Name Z-A")]
    NameDescending = 5
}