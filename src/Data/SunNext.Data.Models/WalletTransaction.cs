using System;
using SunNext.Data.Common.Models;

namespace SunNext.Data.Models;

public class WalletTransaction : BaseDeletableModel<string>
{
    public WalletTransaction()
    {
        this.Id = Guid.NewGuid().ToString();
    }
    
    public DateTime Timestamp { get; set; }

    public double AmountKWh { get; set; }

    public string Type { get; set; }

    public string Source { get; set; } = string.Empty;
}