namespace AzOnPremSidecar.Services.Telemetry;

public class MetricsUpdaterOptions
{
	public bool Enabled { get; set; }
	public TimeSpan Interval { get; set; }
}
