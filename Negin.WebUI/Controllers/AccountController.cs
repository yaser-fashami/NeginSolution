using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Entities;
using Negin.Framework.Utilities;
using Negin.WebUI.Models.ViewModels;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Authorization;
using Negin.Infra.Data.Sql.Migrations;

namespace Negin.WebUI.Controllers;

public class AccountController : Controller
{
	private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
	{
		_userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult LogIn()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> LogIn(string userName, string password, bool remmemberMe)
	{
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(userName.Trim(), password, remmemberMe, false);
            if (result.Succeeded)
            {
				var currentUser = _userManager.FindByNameAsync(userName).Result;
				currentUser.LastLogInDate = DateTime.Now;
				await _userManager.UpdateAsync(currentUser);
				return Redirect("/Dashboard/List");
            }
            ModelState.AddModelError("", "Invalid user name or password");
        }

        return View();
	}

	public async Task<IActionResult> LogOut()
	{
		await _signInManager.SignOutAsync();
		return RedirectToAction("LogIn");
	}

	[Authorize(Roles ="admin")]
    [Breadcrumb("UserManagement")]
    public async Task<IActionResult> List()
	{
		IList<User> allUsers = await _userManager.Users.Include(c => c.Roles).ToListAsync();
		return View(allUsers);
	}

	[Authorize(Roles = "admin")]
	[Breadcrumb("CreateUser", FromAction = "List", FromController = typeof(AccountController))]
	public ActionResult CreateUser()
	{
		return View();
	}

	[HttpPost]
	public async Task<ActionResult> CreateUser(UserViewModel newUser)
	{
		if (ModelState.IsValid)
		{
			var user = new User()
			{
				FirstName = newUser.User.FirstName?.Trim(),
				LastName = newUser.User.LastName?.Trim(),
				UserName = newUser.User.UserName.Trim(),
				//PasswordHash = newUser.User.PasswordHash,
				//Util.GetHashString(newUser.User.PasswordHash.Trim()),
				PhoneNumber = newUser.User.PhoneNumber?.Trim(),
				Email = newUser.User.Email?.Trim(),
				IsActived = newUser.User.IsActived,
				CreateDate = DateTime.Now,
			};
			await CreateUserAvatar(newUser, user);

			IdentityResult result1 = await _userManager.CreateAsync(user, newUser.User.PasswordHash);
			if (result1.Succeeded)
			{
				IdentityResult result2 = await _userManager.AddToRolesAsync(user, newUser.Role);
				if (result2.Succeeded)
				{
					return RedirectToAction("List", "Account");
				}
				else
				{
					foreach (var item in result2.Errors)
					{
						ModelState.AddModelError("", item.Description);
					}
				}
			}
			else
			{
				foreach (var item in result1.Errors)
				{
					ModelState.AddModelError("", item.Description);
				}
			}

		}
		return View();
	}

	[HttpPost]
	[Authorize(Roles = "admin")]
	public async Task ToggleActiveUser(string id)
	{
		var user = await _userManager.FindByIdAsync(id);
		if (user != null)
		{
			user.IsActived = !user.IsActived;
			await _userManager.UpdateAsync(user);
		}
	}

	[Authorize(Roles = "admin")]
	[Breadcrumb("EditUser", FromAction = "List", FromController = typeof(AccountController))]
	public async Task<IActionResult> Edit(string id)
	{
		var user = await _userManager.FindByIdAsync(id);
		var roles = await _userManager.GetRolesAsync(user);
		UserViewModel model = new UserViewModel()
		{
			User = user,
			Role = roles.ToArray()
		};
		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(UserViewModel input)
	{
		if (ModelState.IsValid)
		{
			var user = await _userManager.FindByIdAsync(input.User.Id);
			user.FirstName = input.User.FirstName?.Trim();
			user.LastName = input.User.LastName?.Trim();
			user.Email = input.User.Email?.Trim();
			if (input.User.PasswordHash != null)
			{
				user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, input.User.PasswordHash);
			}
			user.PhoneNumber = input.User.PhoneNumber?.Trim();
			user.IsActived = input.User.IsActived;
            if (input.Avatar != null && input.Avatar.Length > 0)
            {
				await CreateUserAvatar(input, user);
			}
			if (input.avatar_remove == 1)
            {
				RemoveUserAvatar(user);
				user.UserAvatar = "blank.png";
			}

			var result1 = await _userManager.UpdateAsync(user);
			IdentityResult result2 = null;

			if (result1.Succeeded)
			{
				foreach (var role in input.Role)
				{
					if ((await _userManager.IsInRoleAsync(user, role)) == false)
					{
						result2 = await _userManager.AddToRoleAsync(user, role);
					}
				}
				foreach (var role in await _userManager.GetRolesAsync(user))
				{
					if (input.Role.Contains(role) == false)
					{
						result2 = await _userManager.RemoveFromRoleAsync(user, role);
					}
				}
				if (result2 == null || result2.Succeeded)
				{
					return RedirectToAction("List", "Account");
				}
				else
				{
					foreach (var item in result2.Errors)
					{
						ModelState.AddModelError("", item.Description);
					}
				}
			}
			else
			{
				foreach (var item in result1.Errors)
				{
					ModelState.AddModelError("", item.Description);
				}
			}	
		}
		return View(input);
	}

	public IActionResult AccessDenied()
	{
		return View();
	}

	private static async Task CreateUserAvatar(UserViewModel newUser, User user)
	{
		RemoveUserAvatar(user);

		if (newUser.Avatar != null && newUser.Avatar.Length > 0)
		{
			string imagePath = string.Empty;
			user.UserAvatar = newUser.User.UserName + Path.GetExtension(newUser.Avatar.FileName);
			imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/media/avatars", user.UserAvatar);
			using (var stream = System.IO.File.Create(imagePath))
			{
				await newUser.Avatar.CopyToAsync(stream);
			}

		}
	}

	private static void RemoveUserAvatar(User user)
	{
		if (user.UserAvatar != "blank.png" && System.IO.File.Exists("wwwroot/assets/media/avatars/" + user.UserAvatar))
		{
			System.IO.File.Delete("wwwroot/assets/media/avatars/" + user.UserAvatar);
		}
	}
}
