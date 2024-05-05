using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using TraderPerformance.Data;
using TraderPerformance.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using TraderPerformance.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace TraderPerformance.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid portfolioId)
        {
            var portfolio = await _context.Portfolios.FindAsync(portfolioId);

            if (portfolio == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(User);

            var groupedTrades = await GetGroupedTrades(portfolioId);

            var viewModel = new DashboardViewModel
            {
                Portfolio = portfolio,
                GroupedTrades = groupedTrades,
                ShowToday = await CheckTradesForCurrentUser(currentUser.Id, portfolioId, DateTime.Now.Date)
            };

            return View(viewModel);
        }
        private async Task<bool> CheckTradesForCurrentUser(string userId, Guid portfolioId, DateTime currentDate)
        {
            var userIdParam = new SqlParameter("@UserId", userId);
            var portfolioIdParam = new SqlParameter("@PortfolioId", portfolioId);
            var currentDateParam = new SqlParameter("@CurrentDate", currentDate);

            var exists = await _context.Trades
                .FromSqlRaw("SELECT Id, Date, PortfolioID, Price, Quantity, SecurityID, Type, UserId " +
                            "FROM dbo.CheckTradesForCurrentUser(@UserId, @PortfolioId, @CurrentDate)",
                            userIdParam, portfolioIdParam, currentDateParam)
                .AnyAsync();

            return exists;
        }



        private async Task<List<GroupedTradeViewModel>> GetGroupedTrades(Guid portfolioId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var tradesToday = _context.Trades
              .Include(t => t.Security)  // Eagerly load the Security navigation property
              .Where(t => t.UserId == currentUser.Id && t.PortfolioID == portfolioId && t.Date.Date == DateTime.Now.Date)
              .ToList();

            if (!tradesToday.Any())
                tradesToday = _context.Trades
                  .Include(t => t.Security)  // Eagerly load the Security navigation property
                  .Where(t => t.UserId == currentUser.Id && t.PortfolioID == portfolioId)
                  .ToList();

            var groupedTrades = tradesToday.GroupBy(t => t.SecurityID)
                                           .Select(g => new GroupedTradeViewModel
                                           {
                                               SecurityID = g.Key,
                                               TickerSymbol = g.First().Security.TickerSymbol,  // Assuming you have a Security navigation property in your Trade model
                                               TotalCost = g.Where(t => t.Type == "Buy").Sum(t => t.Quantity * t.Price),
                                               TotalSale = g.Where(t => t.Type == "Sell").Sum(t => t.Quantity * t.Price)
                                           })
                                           .ToList();

            foreach (var group in groupedTrades)
            {
                var gainLossData = CalculateGainLossForGroup(group, tradesToday);
                group.GainLoss = gainLossData.GainLoss;
                group.ProgressWidth = gainLossData.ProgressWidth;
            }

            return groupedTrades;
        }

        private (decimal GainLoss, decimal ProgressWidth) CalculateGainLossForGroup(GroupedTradeViewModel group, List<Trade> trades)
        {
            decimal totalCost = 0;
            decimal totalSale = 0;

            foreach (var trade in trades.Where(t => t.SecurityID == group.SecurityID))
            {
                if (trade.Type == "Buy")
                {
                    totalCost += trade.Quantity * trade.Price;
                }
                else if (trade.Type == "Sell")
                {
                    totalSale += trade.Quantity * trade.Price;
                }
            }

            decimal gainLoss = totalSale - totalCost;
            decimal progressWidth = (totalCost + totalSale) != 0 ? (Math.Abs(gainLoss) / (totalCost + totalSale)) * 100 : 0;

            return (gainLoss, progressWidth);
        }
    }
}
