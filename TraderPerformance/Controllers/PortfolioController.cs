using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TraderPerformance.Data;
using TraderPerformance.Models;

namespace TraderPerformance.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PortfolioController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var portfolios = await _context.Portfolios
                .Where(p => p.UserId == currentUser.Id)
                .ToListAsync();

            return View(portfolios);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var portfolio = await _context.Portfolios.FindAsync(id);

            if (portfolio == null)
            {
                return NotFound();
            }

            return View(portfolio);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Portfolio portfolio)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                portfolio.UserId = currentUser.Id;

                _context.Add(portfolio);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(portfolio);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var portfolio = await _context.Portfolios.FindAsync(id);

            if (portfolio == null)
            {
                return NotFound();
            }

            return View(portfolio);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, Portfolio portfolio)
        {
            if (id != portfolio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(portfolio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PortfolioExists(portfolio.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(portfolio);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var portfolio = await _context.Portfolios.FindAsync(id);

            if (portfolio == null)
            {
                return NotFound();
            }

            _context.Portfolios.Remove(portfolio);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool PortfolioExists(Guid id)
        {
            return _context.Portfolios.Any(e => e.Id == id);
        }
    }
}
