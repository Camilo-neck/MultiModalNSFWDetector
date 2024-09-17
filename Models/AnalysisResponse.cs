namespace MultiModalNSFWDetector.Models;

public class AnalysisResponse
{
	public string Text { get; set; }
	public double HateSeverity { get; set; }
	public double SelfHarmSeverity { get; set; }
	public double SexualSeverity { get; set; }
	public double ViolenceSeverity { get; set; }
}
