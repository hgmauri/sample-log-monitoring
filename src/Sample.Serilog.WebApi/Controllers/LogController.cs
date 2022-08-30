using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sample.Serilog.WebApi.Controllers;

[Route("api/[controller]")]
public class LogController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LogController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
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

    [HttpGet("cidades")]
    public async Task<IActionResult> GetCitiesAsync()
    {
        var result = string.Empty;
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://servicodados.ibge.gov.br/api/v1/localidades/estados/ES/municipios");

        if (response.IsSuccessStatusCode)
        {
            result = await response.Content.ReadAsStringAsync();
        }

        return Ok(result);
    }
}