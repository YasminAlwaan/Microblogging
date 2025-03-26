using Microblogging.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Microblogging.Web.Controllers
{
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
        public async Task<IActionResult> Login(LoginModel model)
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
                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, model.Username)
            };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return RedirectToAction("Index", "Posts");
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }
    }
}
