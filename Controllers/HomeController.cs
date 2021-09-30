
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using freezebee_api.Services;
using freezebee_api.Models;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace freezebee_api.Controllers
{
    [Route("api")]
    public class HomeController : ControllerBase
    {
        private readonly FreezebeeContext _context;

        public HomeController(FreezebeeContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public IActionResult Authenticate([FromBody] User model)
        {
            IQueryable<User> query = _context.Users;
            var user = query.Where(u => u.Email == model.Email).First();

            if (user == null)
                return NotFound(new { message = "Utilisateur inconnu." });

            if (BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                var token = TokenService.CreateToken(user);
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/",
                    Secure = true,
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                };
                Response.Cookies.Append("freezebee_session", token, cookieOptions);
                return Ok();
            }
            else
            {
                return NotFound(new { message = "Mot de passe incorrect." });
            }
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Register([FromBody] User model)
        {
            var user = model;

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("me")]
        [Authorize]
        public async Task<ActionResult<dynamic>> CurrentUser()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            string userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

            var user = await _context.Users.FindAsync(Guid.Parse(userId));

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        [Route("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/",
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
            };

            Response.Cookies.Delete("freezebee_session", cookieOptions);

            return Ok();
        }
    }
}
