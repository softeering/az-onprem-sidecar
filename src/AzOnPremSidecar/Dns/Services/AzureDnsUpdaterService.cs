using AzOnPremSidecar.Dns.Config;
using AzOnPremSidecar.Utils;
using Microsoft.Azure.Management.Dns;
using Microsoft.Azure.Management.Dns.Models;
using Microsoft.Extensions.Options;
using Microsoft.Rest.Azure.Authentication;

namespace AzOnPremSideca.Dns.Services;

public class AzureDnsUpdaterService : IDnsUpdater
{
	private readonly DnsUpdaterOptions _options;
	private readonly IHostInfoProvider _ipProvider;

	public AzureDnsUpdaterService(IOptions<DnsUpdaterOptions> options, IHostInfoProvider ipProvider)
	{
		this._options = options.Value;
		this._ipProvider = ipProvider;
	}

	public async Task UpdateDnsEntry(string zone, string record, CancellationToken cancellationToken = default)
	{
		var ip = await this._ipProvider.GetPublicIPAsync();

		var (tenantId, clientId, clientSecret) = GetCredentialInfo(this._options);
		var creds = await ApplicationTokenProvider.LoginSilentAsync(tenantId, clientId, clientSecret);
		var dnsClient = new DnsManagementClient(creds);
		dnsClient.SubscriptionId = this._options.SubscriptionId;

		var recordSet = new RecordSet()
		{
			TTL = (long)(this._options.TTL?.TotalSeconds).GetValueOrDefault(300),
			ARecords = new List<ARecord>() { new ARecord(ip) },
			Metadata = new Dictionary<string, string>()
			{
				["createdBy"] = "Azure-DynDns (.NET)",
				["updated"] = DateTime.Now.ToString()
			}
		};

		await dnsClient.RecordSets.CreateOrUpdateAsync(this._options.ResourceGroup, zone, record, RecordType.A, recordSet);
	}

	private (string, string, string) GetCredentialInfo(DnsUpdaterOptions options)
	{
		var tenantId = options.TenantId.Standardize() ?? Environment.GetEnvironmentVariable("AZURE_TENANT_ID") ?? throw new Exception("tenandId is mandatory");
		var clientId = options.ClientId.Standardize() ?? Environment.GetEnvironmentVariable("AZURE_CLIENT_ID") ?? throw new Exception("clientId is mandatory");
		var clientSecret = options.ClientSecret.Standardize() ?? Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET") ?? throw new Exception("clientSecret is mandatory");
		return (tenantId, clientId, clientSecret);
	}
}
