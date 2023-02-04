using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Survey.API.Interfaces;

namespace Survey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveysController : ControllerBase
    {
        private readonly ISurvey _survey;

        public SurveysController(ISurvey survey)
        {
            _survey = survey;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var response = _survey.FindAll();
            if (response.status != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _survey.FindById(id);
            if (response.status != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("/api/surveys/link/{link}")]
        public IActionResult GetByLink(string link)
        {
            var response = _survey.FindByLink(link);
            if (response.status != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody] Models.Survey model)
        {
            var response = _survey.Create(model);
            if (response.status != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPut]
        public IActionResult Put([FromBody] Models.Survey model)
        {
            var response = _survey.Update(model);
            if (response.status != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = _survey.Delete(id);
            if (response.status != 200)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
