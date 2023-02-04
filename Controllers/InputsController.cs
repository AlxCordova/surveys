using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Survey.API.Interfaces;
using Survey.API.Models;

namespace Survey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InputsController : ControllerBase
    {
        private readonly IInput _inputs;

        public InputsController(IInput inputs)
        {
            _inputs = inputs;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var response = _inputs.FindAll();
            if (response.status != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _inputs.FindById(id);
            if (response.status != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody] Input model)
        {
            var response = _inputs.Insert(model);
            if (response.status != 200)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
