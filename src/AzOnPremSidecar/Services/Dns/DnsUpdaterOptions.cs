namespace AzOnPremSidecar.Services.Dns;

public class DnsUpdaterOptions
{
	// COMMON
	public bool Enabled { get; set; }
	public string Provider { get; set; } = "Azure";
	public TimeSpan? Interval { get; set; } = TimeSpan.FromMinutes(10);
	public RecordItem[]? Records { get; set; }
	public TimeSpan? TTL { get; set; } = TimeSpan.FromMinutes(5);

	// AZURE
	public string? ResourceGroup { get; set; }
	public string? SubscriptionId { get; set; }
	public string? TenantId { get; set; }
	public string? ClientId { get; set; }
	public string? ClientSecret { get; set; }

	// AWS
	public string? Region { get; set; }
	public string? Profile { get; set; }
}

public class RecordItem
{
	public string? Zone { get; set; }
	public string? Record { get; set; }
}
