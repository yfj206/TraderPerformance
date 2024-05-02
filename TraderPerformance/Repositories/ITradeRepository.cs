using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
        return _context.Trades.FromSqlRaw("EXECUTE GetTradesAndPortfolioName @PortfolioId", new SqlParameter("@PortfolioId", portfolioId)).ToList();
    }

    public string GetPortfolioName(Guid portfolioId)
    {
        return _context.Portfolios.FromSqlRaw("EXECUTE GetTradesAndPortfolioName @PortfolioId", new SqlParameter("@PortfolioId", portfolioId)).FirstOrDefault()?.Name;
    }
}

