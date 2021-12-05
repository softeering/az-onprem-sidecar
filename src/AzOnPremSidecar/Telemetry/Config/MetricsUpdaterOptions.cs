namespace AzOnPremSidecar.Telemetry.Config;

public class MetricsUpdaterOptions
{
	public bool Enabled { get; set; }
	public string Provider { get; set; } = "Azure";
	public TimeSpan? Interval { get; set; } = TimeSpan.FromMinutes(1);
}
