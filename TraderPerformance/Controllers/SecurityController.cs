using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TraderPerformance.Data;
using TraderPerformance.Models;

namespace TraderPerformance.Controllers
{
    public class SecurityController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SecurityController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Security
        public async Task<IActionResult> Index(Guid portfolioId)
        {
            var securities = await _context.Securities.ToListAsync();
            ViewData["PortfolioId"] = portfolioId;
            return View(securities);
        }

        // GET: Security/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var security = await _context.Securities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (security == null)
            {
                return NotFound();
            }
			ViewData["PortfolioId"] = id;

			return View(security);
        }

        // GET: Security/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Security/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Symbol,Name")] Security security)
        {
            if (ModelState.IsValid)
            {
                _context.Add(security);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(security);
        }

        // GET: Security/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var security = await _context.Securities.FindAsync(id);
            if (security == null)
            {
                return NotFound();
            }
			ViewData["PortfolioId"] = id;

			return View(security);
        }

        // POST: Security/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Symbol,Name")] Security security)
        {
            if (id != security.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(security);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SecurityExists(security.Id))
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
			ViewData["PortfolioId"] = id;

			return View(security);
        }

        // GET: Security/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var security = await _context.Securities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (security == null)
            {
                return NotFound();
            }

			ViewData["PortfolioId"] = id;

			return View(security);
        }

        // POST: Security/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var security = await _context.Securities.FindAsync(id);
            _context.Securities.Remove(security);
            await _context.SaveChangesAsync();
			ViewData["PortfolioId"] = id;

			return RedirectToAction(nameof(Index));

        }

        private bool SecurityExists(Guid id)
        {
            return _context.Securities.Any(e => e.Id == id);
        }
    }
}
