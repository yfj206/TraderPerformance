using CsvHelper.Configuration;

namespace TraderPerformance.Models;

public class TradeMap : ClassMap<TradeImportModel>
{
    public TradeMap()
    {
        Map(m => m.Date).Name("Date");
        Map(m => m.Type).Name("Type");
        Map(m => m.Quantity).Name("Quantity");
        Map(m => m.Price).Name("Price");
        Map(m => m.SecurityID).Name("SecurityID");
        Map(m => m.PortfolioID).Name("PortfolioID");
    }
}