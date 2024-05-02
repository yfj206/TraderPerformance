using System;
using System.Collections.Generic;
using System.Linq;
using TraderPerformance.Data;
using TraderPerformance.Models;

namespace TraderPerformance.Repositories;

public interface ITradeRepository
{
    IEnumerable<Trade> GetTradesForPortfolio(Guid portfolioId);
    string GetPortfolioName(Guid portfolioId);
}

public class TradeRepository : ITradeRepository
{
    private readonly ApplicationDbContext _context;

    public TradeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Trade> GetTradesForPortfolio(Guid portfolioId)
    {
        return _context.Trades.Where(t => t.Id == portfolioId).ToList();
    }

    public string GetPortfolioName(Guid portfolioId)
    {
        return _context.Portfolios.FirstOrDefault(p => p.Id == portfolioId)?.Name;
    }
}
