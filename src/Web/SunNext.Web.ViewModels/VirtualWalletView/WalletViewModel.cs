using System;

namespace SunNext.Web.ViewModels.VirtualWalletView;

public class WalletViewModel
{
    public string UserId { get; set; }
    public double TodayGeneratedKWh { get; set; }
    public double BalanceKWh { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool WasChargedToday { get; set; }
}