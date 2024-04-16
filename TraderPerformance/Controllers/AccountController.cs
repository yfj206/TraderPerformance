using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TraderPerformance.Models;

namespace TraderPerformance.Controllers;

[Authorize]
public class AccountController : Controller
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SignInManager<IdentityUser> _signInManager;

	public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	[HttpGet]
	public async Task<IActionResult> UpdateProfile()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user == null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		var model = new UpdateProfileViewModel
		{
			//FirstName = user.FirstName,
			//LastName = user.LastName,
			PhoneNumber = user.PhoneNumber
		};

		return View(model);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		var user = await _userManager.GetUserAsync(User);
		if (user == null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		//user.FirstName = model.FirstName;
		//user.LastName = model.LastName;
		user.PhoneNumber = model.PhoneNumber;

		var result = await _userManager.UpdateAsync(user);
		if (result.Succeeded)
		{
			await _signInManager.RefreshSignInAsync(user);
			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		foreach (var error in result.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}

		return View(model);
	}
}
