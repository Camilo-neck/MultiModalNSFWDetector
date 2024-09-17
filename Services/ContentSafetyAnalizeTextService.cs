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

        public ContentSafetySampleAnalyzeTextService(ILogger<ContentSafetySampleAnalyzeTextService> logger)
        {
            DotNetEnv.Env.TraversePath().Load();
            _logger = logger;
        }

        public AnalysisResponse AnalyzeText(string? sendedText = null)
        {
            string endpoint = Environment.GetEnvironmentVariable("CONTENT_SAFETY_ENDPOINT")!;
            string key = Environment.GetEnvironmentVariable("CONTENT_SAFETY_KEY")!;

            ContentSafetyClient client = new ContentSafetyClient(new Uri(endpoint), new AzureKeyCredential(key));
            
            if (String.IsNullOrEmpty(sendedText))
            {
                throw new ArgumentNullException(nameof(sendedText));
            }
            string text = sendedText;
            var request = new AnalyzeTextOptions(text?.Substring(0, Math.Min(text.Length, 999)));

            Response<AnalyzeTextResult> response;
            try
            {
                response = client.AnalyzeText(request);
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError("Analyze text failed.\nStatus code: {0}, Error code: {1}, Error message: {2}", ex.Status, ex.ErrorCode, ex.Message);
                throw;
            }

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