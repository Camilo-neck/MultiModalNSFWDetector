using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using MultiModalNSFWDetector.Controllers;
using System.IO;

namespace MultiModalNSFWDetector.Services;

public class SpeechToTextService
{
    private static string _speechEndpoint;
    private static string _speechKey;
    private static string _speechRegion;
    private static ILogger<ContentSafetyAnalyzeTextController> _logger;

    public SpeechToTextService(ILogger<ContentSafetyAnalyzeTextController> logger)
    {
		DotNetEnv.Env.TraversePath().Load();
        _speechEndpoint = DotNetEnv.Env.GetString("SPEECH_ENDPOINT");
        _speechKey = DotNetEnv.Env.GetString("SPEECH_KEY");
        _speechRegion = DotNetEnv.Env.GetString("SPEECH_REGION");
        _logger = logger;
    }

    private static string OutputSpeechToText(SpeechRecognitionResult speechRecognitionResult)
    {
        switch (speechRecognitionResult.Reason)
        {
            case ResultReason.RecognizedSpeech:
                _logger.LogInformation($"RECOGNIZED: Text={speechRecognitionResult.Text}");
                return speechRecognitionResult.Text;
            case ResultReason.NoMatch:
                _logger.LogError($"NOMATCH: Speech could not be recognized.");
                break;
            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                _logger.LogError($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    _logger.LogDebug($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    _logger.LogDebug($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    _logger.LogDebug($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
        }

        return null;
    }

    public async Task<string> SpeechToText(string audioPath)
    {
        var speechConfig = SpeechConfig.FromSubscription(_speechKey, _speechRegion);
        speechConfig.SpeechRecognitionLanguage = "es-CO";

        using var audioConfig = AudioConfig.FromWavFileInput(audioPath);
        using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);
        _logger.LogInformation("Starting speech recognition.");

        var result = await recognizer.RecognizeOnceAsync();
        return OutputSpeechToText(result);
    }
}