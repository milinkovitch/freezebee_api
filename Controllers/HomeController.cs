
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using freezebee_api.Services;
using freezebee_api.Models;

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
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User model)
        {
            var user = await _context.Users.FindAsync(model);

            if (user == null)
                return NotFound(new { message = "User or password invalid" });

            var token = TokenService.CreateToken(user);
            user.Password = "";
            return new
            {
                user = user,
                token = token
            };
        }
    }
}
