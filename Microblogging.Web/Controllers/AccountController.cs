// Controllers/AccountController.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microblogging.Web.Models;
using Microblogging.Data;
using Microsoft.AspNetCore.Authorization;

public class AccountController : Controller
{
    private readonly IConfiguration _config;
    private readonly IRepository _repository;

    public AccountController(IConfiguration config, IRepository repository)
    {
        _config = config;
        _repository = repository;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (ModelState.IsValid)
        {
            // Hardcoded users for demonstration
            var users = new Dictionary<string, string>
            {
                {"user1", "password1"},
                {"user2", "password2"}
            };

            if (users.TryGetValue(model.Username, out var password) &&
                password == model.Password)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, model.Username),
                    new Claim(ClaimTypes.Name, model.Username)
                };

                var identity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties);

                return RedirectToAction("Index", "Posts");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }
}