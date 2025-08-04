namespace SunNext.Services.Data.Prototypes.Market;

public class MarketPriceRawPrototype
{
    public string Date { get; set; }  
    public int Hour { get; set; }
    public decimal PricePerMWh { get; set; }
}