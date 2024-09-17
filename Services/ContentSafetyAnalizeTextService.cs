using System;
using System.IO;
using Azure;
using Azure.Core;
using Azure.AI.ContentSafety;
using MultiModalNSFWDetector.Models;
using MultiModalNSFWDetector.Controllers;

// Services/ContentSafetySampleAnalyzeTextService.cs
namespace MultiModalNSFWDetector.Services
{
    public class ContentSafetySampleAnalyzeTextService
    {
        private static ILogger<ContentSafetySampleAnalyzeTextService> _logger;
        private static SpeechToTextService _speechToTextService;

        public ContentSafetySampleAnalyzeTextService(ILogger<ContentSafetySampleAnalyzeTextService> logger, SpeechToTextService speechToTextService)
        {
            DotNetEnv.Env.TraversePath().Load();
            _logger = logger;
            _speechToTextService = speechToTextService;
        }

        public async Task<AnalysisResponse> AnalyzeAudio(string audioPath)
        {
            if (String.IsNullOrEmpty(audioPath))
            {
                throw new ArgumentNullException(nameof(audioPath));
            }
            var text = await _speechToTextService.SpeechToText(audioPath);
            return AnalyzeText(text);
        }

        public AnalysisResponse AnalyzeText(string? sendedText = null)
        {
            if (String.IsNullOrEmpty(sendedText))
            {
                throw new ArgumentNullException(nameof(sendedText));
            }

            var client = CreateContentSafetyClient();
            var request = CreateAnalyzeTextOptions(sendedText);
            var response = GetAnalysisResponse(client, request);

            return MapToAnalysisResponse(sendedText, response);
        }

        private ContentSafetyClient CreateContentSafetyClient()
        {
            string endpoint = Environment.GetEnvironmentVariable("CONTENT_SAFETY_ENDPOINT")!;
            string key = Environment.GetEnvironmentVariable("CONTENT_SAFETY_KEY")!;
            return new ContentSafetyClient(new Uri(endpoint), new AzureKeyCredential(key));
        }

        private AnalyzeTextOptions CreateAnalyzeTextOptions(string text)
        {
            return new AnalyzeTextOptions(text.Substring(0, Math.Min(text.Length, 999)));
        }

        private Response<AnalyzeTextResult> GetAnalysisResponse(ContentSafetyClient client, AnalyzeTextOptions request)
        {
            try
            {
                return client.AnalyzeText(request);
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError("Analyze text failed.\nStatus code: {0}, Error code: {1}, Error message: {2}", ex.Status, ex.ErrorCode, ex.Message);
                throw;
            }
        }

        private AnalysisResponse MapToAnalysisResponse(string text, Response<AnalyzeTextResult> response)
        {
            return new AnalysisResponse
            {
                Text = text,
                HateSeverity = response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == TextCategory.Hate)?.Severity ?? 0,
                SelfHarmSeverity = response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == TextCategory.SelfHarm)?.Severity ?? 0,
                SexualSeverity = response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == TextCategory.Sexual)?.Severity ?? 0,
                ViolenceSeverity = response.Value.CategoriesAnalysis.FirstOrDefault(a => a.Category == TextCategory.Violence)?.Severity ?? 0
            };
        }
    }
}