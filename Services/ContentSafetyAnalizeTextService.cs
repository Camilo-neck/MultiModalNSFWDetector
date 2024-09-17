using System;
using Azure;
using Azure.Core;
using Azure.AI.ContentSafety;
using MultiModalNSFWDetector.Models;
using MultiModalNSFWDetector.Controllers;

namespace MultiModalNSFWDetector.Services;

class ContentSafetySampleAnalyzeTextService
{
	private static ILogger<ContentSafetyAnalyzeTextController> _logger;
	public ContentSafetySampleAnalyzeTextService(ILogger<ContentSafetyAnalyzeTextController> logger)
	{
		_logger = logger;
	}
	public AnalysisResponse AnalyzeText(string? sendedText = null)
	{
		// retrieve the endpoint and key from the environment variables created earlier
		string endpoint = "https://cj-safety-service.cognitiveservices.azure.com/";
		string key = "7fabc0da798b44e5b163f3e7af0e397d";

		ContentSafetyClient client = new ContentSafetyClient(new Uri(endpoint), new AzureKeyCredential(key));

		string text = sendedText;
		var request = new AnalyzeTextOptions(text.Substring(0, Math.Min(text.Length, 999)));

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
