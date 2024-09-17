using Microsoft.AspNetCore.Mvc;
using MultiModalNSFWDetector.Services;
using MultiModalNSFWDetector.Models;

namespace MultiModalNSFWDetector.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentSafetyAnalyzeTextController : ControllerBase
{

    private readonly ILogger<ContentSafetyAnalyzeTextController> _logger;
    private readonly ContentSafetySampleAnalyzeTextService _contentSafetySampleAnalyzeTextService;

    public ContentSafetyAnalyzeTextController(ILogger<ContentSafetyAnalyzeTextController> logger)
    {
        _logger = logger;
        _contentSafetySampleAnalyzeTextService = new ContentSafetySampleAnalyzeTextService(logger);
    }

    [HttpGet]
    public string Get()
    {
        return "Content Safety Analyze Text Controller";
    }

    [HttpPost]
    public AnalysisResponse AnalyzeText(AnalyzeTextRequest request)
    {
        var response = _contentSafetySampleAnalyzeTextService.AnalyzeText(request.Text);
        return response;
    }
}
