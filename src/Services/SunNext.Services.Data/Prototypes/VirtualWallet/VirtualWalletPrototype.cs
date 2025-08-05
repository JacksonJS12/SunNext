using System;
using System.Collections.Generic;
using SunNext.Data.Common.Models;
using SunNext.Data.Models;

namespace SunNext.Services.Data.Prototypes.VirtualWallet;

public class VirtualWalletPrototype : BaseDeletableModel<string>
{
    public VirtualWalletPrototype()
    {
        this.Id = Guid.NewGuid().ToString();
    }


    public double EnergyBalanceKWh { get; private set; }

    public List<Battery> LinkedBatteries { get; set; } = new();
    public string OwnerId { get; set; }
    public ApplicationUser Owner { get; set; }
    public bool WasChargedToday { get; set; }

    public List<WalletTransaction> Transactions { get; set; } = new();

    public void AddEnergy(double amount, string source = "Production")
    {
        if (amount <= 0) return;

        EnergyBalanceKWh += amount;
        Transactions.Add(new WalletTransaction
        {
            Timestamp = DateTime.UtcNow,
            AmountKWh = amount,
            Type = "Deposit",
            Source = source
        });
    }

    public bool SellEnergy(double amount, string destination = "Market")
    {
        if (amount <= 0 || amount > EnergyBalanceKWh) return false;

        EnergyBalanceKWh -= amount;
        Transactions.Add(new WalletTransaction
        {
            Timestamp = DateTime.UtcNow,
            AmountKWh = -amount,
            Type = "Withdrawal",
            Source = destination
        });

        return true;
    }

    public double GetBalance() => EnergyBalanceKWh;
}