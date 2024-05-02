using System;
using TraderPerformance.Repositories;
using TraderPerformance.ViewModels;

namespace TraderPerformance.Services;

public interface IDashboardService
{
    DashboardViewModel GetDashboardViewModel(Guid portfolioId);
    decimal CalculateGainLossForPortfolio(Guid portfolioId);
}

public class DashboardService : IDashboardService
{
    private readonly ITradeRepository _tradeRepository;

    public DashboardService(ITradeRepository tradeRepository)
    {
        _tradeRepository = tradeRepository;
    }

    public DashboardViewModel GetDashboardViewModel(Guid portfolioId)
    {
        var groupedTrades = _tradeRepository.GetTradesForPortfolio(portfolioId)
            .GroupBy(t => t.SecurityID)
            .Select(g => new GroupedTradeViewModel
            {
                SecurityID = g.Key,
                TickerSymbol = g.First().Security.TickerSymbol,
                TotalCost = g.Where(t => t.Type == "Buy").Sum(t => t.Quantity * t.Price),
                TotalSale = g.Where(t => t.Type == "Sell").Sum(t => t.Quantity * t.Price),
                GainLoss = g.Where(t => t.Type == "Sell").Sum(t => t.Quantity * t.Price) - g.Where(t => t.Type == "Buy").Sum(t => t.Quantity * t.Price),
                ProgressWidth = (g.Where(t => t.Type == "Sell").Sum(t => t.Quantity * t.Price) - g.Where(t => t.Type == "Buy").Sum(t => t.Quantity * t.Price)) / g.Where(t => t.Type == "Buy").Sum(t => t.Quantity * t.Price) * 100
            })
            .ToList();

        var gainLossToday = CalculateGainLossForPortfolio(portfolioId);
        var gainLossAllTime = groupedTrades.Sum(g => g.GainLoss);
        var gainLossMonthly = groupedTrades.Where(g => g.Type == "Sell" && g.Date >= DateTime.Now.AddMonths(-1)).Sum(g => g.Quantity * g.Price) - groupedTrades.Where(g => g.Type == "Buy" && g.Date >= DateTime.Now.AddMonths(-1)).Sum(g => g.Quantity * g.Price);
        var gainLossWeekly = groupedTrades.Where(g => g.Type == "Sell" && g.Date >= DateTime.Now.AddDays(-7)).Sum(g => g.Quantity * g.Price) - groupedTrades.Where(g => g.Type == "Buy" && g.Date >= DateTime.Now.AddDays(-7)).Sum(g => g.Quantity * g.Price);

        return new DashboardViewModel
        {
            Portfolio = new PortfolioViewModel
            {
                Id = portfolioId,
                Name = _tradeRepository.GetPortfolioName(portfolioId)
            },
            GroupedTrades = groupedTrades,
            GainLossToday = gainLossToday,
            GainLossAllTime = gainLossAllTime,
            GainLossMonthly = gainLossMonthly,
            GainLossWeekly = gainLossWeekly
        };
    }

    public decimal CalculateGainLossForPortfolio(Guid portfolioId)
    {
        var trades = _tradeRepository.GetTradesForPortfolio(portfolioId);
        var totalSale = trades.Where(t => t.Type == "Sell").Sum(t => t.Quantity * t.Price);
        var totalCost = trades.Where(t => t.Type == "Buy").Sum(t => t.Quantity * t.Price);

        return totalSale - totalCost;
    }
}

