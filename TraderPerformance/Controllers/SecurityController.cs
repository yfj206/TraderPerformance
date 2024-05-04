using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TraderPerformance.Data;
using TraderPerformance.Models;

namespace TraderPerformance.Controllers
{
    public class SecurityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SecurityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Security
        public async Task<IActionResult> Index(Guid portfolioId)
        {
            var securities = await _context.Securities.FromSqlInterpolated($@"
                EXEC Security_ReadAll"
            ).ToListAsync();

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

            var securities = await _context.Securities.FromSqlRaw("EXEC Security_Read {0}", id).ToListAsync();

            // Now, perform further operations on the returned data
            var security = securities.FirstOrDefault();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TickerSymbol,Name")] Security security)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        EXEC Security_Create 
                            @Id = {Guid.NewGuid()},
                            @TickerSymbol = {security.TickerSymbol}, 
                            @Name = {security.Name}"
                    );

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Handle exception
                    ModelState.AddModelError("", "Error occurred while creating the security.");
                    return View(security);
                }
            }
            return View(security);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var securities = await _context.Securities.FromSqlRaw("EXEC Security_Read {0}", id).ToListAsync();

            // Now, perform further operations on the returned data
            var security = securities.FirstOrDefault();

            if (security == null)
            {
                return NotFound();
            }

            ViewData["PortfolioId"] = id;
            return View(security);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Security security)
        {
            if (id != security.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        EXEC Security_Update 
                            @Id = {security.Id}, 
                            @TickerSymbol = {security.TickerSymbol}, 
                            @Name = {security.Name}"
                    );

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Handle exception
                    ModelState.AddModelError("", "Error occurred while updating the security.");
                    return View(security);
                }
            }
            return View(security);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var securities = await _context.Securities.FromSqlRaw("EXEC Security_Read {0}", id).ToListAsync();

            // Now, perform further operations on the returned data
            var security = securities.FirstOrDefault();

            if (security == null)
            {
                return NotFound();
            }

            ViewData["PortfolioId"] = id;
            return View(security);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    EXEC Security_Delete @Id = {id}"
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}
