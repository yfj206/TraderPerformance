using CsvHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using System.Diagnostics;
using System.Globalization;
using TraderPerformance.Data;
using TraderPerformance.Models;
using TraderPerformance.ViewModels;

namespace TraderPerformance.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;  // Replace 'YourDbContext' with your DbContext name

    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var traderId = currentUser.Id;

        var portfolios = _context.Portfolios.Where(p => p.UserId == traderId).ToList();

        var viewModelList = new List<HomeViewModel>();

        foreach (var portfolio in portfolios)
        {
            var portfolioTrades = _context.Trades.Where(t => t.PortfolioID == portfolio.Id).ToList();

            decimal totalCost = 0;
            decimal totalSale = 0;

            foreach (var trade in portfolioTrades)
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
            string gainLossColor = gainLoss >= 0 ? "success" : "danger";
            decimal progressWidth = (totalCost + totalSale) != 0 ? (Math.Abs(gainLoss) / (totalCost + totalSale)) * 100 : 0;

            var viewModel = new HomeViewModel
            {
                Id = portfolio.Id,
                Name = portfolio.Name,
                GainLoss = gainLoss,
                GainLossColor = gainLossColor,
                ProgressWidth = progressWidth
            };

            viewModelList.Add(viewModel);
        }

        return View(viewModelList);
    }

    public async Task<IActionResult> CalculateGainLossForPortfolio(Guid portfolioId)
    {
        var portfolio = await _context.Portfolios.FindAsync(portfolioId);
        if (portfolio == null)
        {
            return NotFound();
        }

        var portfolioTrades = _context.Trades.Where(t => t.PortfolioID == portfolioId).ToList();

        decimal totalCost = 0;
        decimal totalSale = 0;

        foreach (var trade in portfolioTrades)
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
        string gainLossColor = gainLoss >= 0 ? "success" : "danger";
        decimal progressWidth = (totalCost + totalSale) != 0 ? (Math.Abs(gainLoss) / (totalCost + totalSale)) * 100 : 0;

        return Json(new
        {
            GainLoss = gainLoss,
            GainLossColor = gainLossColor,
            ProgressWidth = progressWidth
        });
    }

    [HttpPost]
    public async Task<IActionResult> Import(IFormFile csvFile)
    {
        if (csvFile == null || csvFile.Length == 0)
        {
            ModelState.AddModelError("CsvFile", "Please upload a CSV file.");
            return RedirectToAction("Index", "Home");
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var traderId = currentUser.Id;


        using (var reader = new StreamReader(csvFile.OpenReadStream()))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<TradeMap>();

            var records = csv.GetRecords<TradeImportModel>().ToList();

    
            var tradeRecords = records.Select(r => new Trade
            {
                UserId = traderId,
                Date = r.Date,
                Type = r.Type,
                Quantity = r.Quantity,
                Price = r.Price,
                SecurityID = r.SecurityID,
                PortfolioID = r.PortfolioID
            }).ToList();

            _context.Trades.AddRange(tradeRecords);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Home");  

    }

    public async Task<IActionResult> Import(IFormFile csvFile, Guid portfolioId)
    {
        if (csvFile == null || csvFile.Length == 0)
        {
            ModelState.AddModelError("CsvFile", "Please upload a CSV file.");
            return View("Dashboard");
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var traderId = currentUser.Id;


        using (var reader = new StreamReader(csvFile.OpenReadStream()))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<TradeMap>();

            var records = csv.GetRecords<TradeImportModel>().ToList();

            foreach (var record in records)
            {
                portfolioId = record.PortfolioID == Guid.Empty ? portfolioId : record.PortfolioID;
            }


            var tradeRecords = records.Select(r => new Trade
            {
                UserId = traderId,
                Date = r.Date,
                Type = r.Type,
                Quantity = r.Quantity,
                Price = r.Price,
                SecurityID = r.SecurityID,
                PortfolioID = portfolioId
            }).ToList();

            _context.Trades.AddRange(tradeRecords);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Dashboard", new { id = portfolioId });  // Redirect to dashboard page after import

    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
