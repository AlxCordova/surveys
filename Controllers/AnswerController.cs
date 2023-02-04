using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Survey.API.Interfaces;
using Survey.API.Models;

namespace Survey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswer _answer;

        public AnswerController(IAnswer answer)
        {
            _answer = answer;
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _answer.GetResponsesBySurveyId(id);
            if (response.status != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post([FromBody] List<SurveyAnswers> answers)
        {
            var response = _answer.SaveResponse(answers);
            if (response.status != 200)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
