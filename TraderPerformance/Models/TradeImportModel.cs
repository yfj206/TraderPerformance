namespace TraderPerformance.Models;

public class TradeImportModel
{
    public string UserId { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public Guid SecurityID { get; set; }
    public Guid PortfolioID { get; set; }
}
