using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;

namespace Sample.Serilog.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class LogController : Controller
    {
        public LogController()
        {
        }

        [HttpPost("sample")]
        public IActionResult PostSampleData()
        {
            return Ok(new { Result = "Data successfully registered with Elasticsearch" });
        }

        [HttpGet("")]
        public IActionResult GetAll()
        {
            return Ok();
        }

        [HttpGet("name")]
        public IActionResult GetByName([FromQuery] string name)
        {
            return Ok();
        }

        [HttpGet("description")]
        public IActionResult GetByDescription([FromQuery] string description)
        {
            return Ok();
        }

        [HttpGet("condiction")]
        public IActionResult GetByCondictions([FromQuery] string name, [FromQuery] string description, [FromQuery] DateTime? birthdate)
        {
            return Ok();
        }

        [HttpGet("term")]
        public IActionResult GetByAllCondictions([FromQuery] string term)
        {
            return Ok();
        }

        [HttpGet("aggregation")]
        public IActionResult GetActorsAggregation()
        {
            return Ok();
        }
    }
}
