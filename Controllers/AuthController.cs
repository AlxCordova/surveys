using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Survey.API.Interfaces;
using Survey.API.Models;

namespace Survey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _auth;

        public AuthController(IAuth auth)
        {
            _auth = auth;
        }

        [AllowAnonymous]
        [HttpPost("/api/auth/create-user")]
        public IActionResult Post([FromBody] User model)
        {
            var response = _auth.CreateUser(model);
            if (response.status != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("/api/auth/login")]
        public IActionResult Login([FromBody] Login model)
        {
            var response = _auth.ValidateUser(model);
            if (response.status != 200)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
