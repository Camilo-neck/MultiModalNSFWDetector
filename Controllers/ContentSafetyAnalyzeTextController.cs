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
    private readonly SpeechToTextService _speechToTextService;

    public ContentSafetyAnalyzeTextController(ILogger<ContentSafetyAnalyzeTextController> logger, 
        ContentSafetySampleAnalyzeTextService contentSafetySampleAnalyzeTextService
        , SpeechToTextService speechToTextService)
    {
        _logger = logger;
        _contentSafetySampleAnalyzeTextService = contentSafetySampleAnalyzeTextService;
        _speechToTextService = speechToTextService;
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
    
    [HttpPost("speech")]
    public async Task<AnalysisResponse> AnalyzeAudio(SpeechRequest request)
    {
        var audioPath = request.AudioPath;
        return await _contentSafetySampleAnalyzeTextService.AnalyzeAudio(audioPath);   
    }
}
