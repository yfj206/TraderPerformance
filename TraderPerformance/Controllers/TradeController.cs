using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TraderPerformance.Data;
using TraderPerformance.Models;

namespace TraderPerformance.Controllers;

public class TradeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public TradeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(Guid portfolioId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var trades = await _context.Trades
            .Where(t => t.UserId == currentUser.Id && t.PortfolioID == portfolioId)
            .Include(t => t.Security)
            .ToListAsync();
		ViewData["PortfolioId"] = portfolioId;

		return View(trades);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var trade = await _context.Trades
            .Include(t => t.Security)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trade == null)
        {
            return NotFound();
        }

        return View(trade);
    }

    [HttpGet]
    public IActionResult Create(Guid portfolioId)
    {
        ViewBag.PortfolioID = portfolioId;
        ViewBag.SecurityID = new SelectList(_context.Securities, "Id", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Trade trade, Guid portfolioId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        trade.UserId = currentUser.Id;
        trade.PortfolioID = portfolioId;

        //if (ModelState.IsValid)
        //{ 
            _context.Add(trade);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { portfolioId = trade.PortfolioID });
        //}
        ViewBag.PortfolioID = portfolioId;
        ViewBag.SecurityID = new SelectList(_context.Securities, "Id", "Name", trade.SecurityID);
        return View(trade);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, Guid portfolioId)
    {
        var trade = await _context.Trades.FindAsync(id);

        if (trade == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        trade.UserId = currentUser.Id;

        ViewBag.PortfolioID = portfolioId;
        ViewBag.SecurityID = new SelectList(_context.Securities, "Id", "Name", trade.SecurityID);
        return View(trade);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, Trade trade, Guid portfolioId)
    {
        if (id != trade.Id)
        {
            return NotFound();
        }

        trade.PortfolioID = portfolioId;

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(trade);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TradeExists(trade.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index), new { portfolioId = trade.PortfolioID });
        }
        ViewBag.PortfolioID = portfolioId;
        ViewBag.SecurityID = new SelectList(_context.Securities, "Id", "Name", trade.SecurityID);
        return View(trade);
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var trade = await _context.Trades.FindAsync(id);

        if (trade == null)
        {
            return NotFound();
        }

        _context.Trades.Remove(trade);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { portfolioId = trade.PortfolioID });
    }

    private bool TradeExists(Guid id)
    {
        return _context.Trades.Any(e => e.Id == id);
    }
}
