using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("exception")]
        public IActionResult GetByName()
        {
            //Sample middlerare exception log
            throw new Exception("Não foi possível fazer o get.");
        }

    }
}
