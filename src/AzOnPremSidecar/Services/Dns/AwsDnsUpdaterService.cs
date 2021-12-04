using Amazon;
using Amazon.Route53;
using Amazon.Route53.Model;
using Microsoft.Extensions.Options;
using System.Net;

namespace AzOnPremSidecar.Services.Dns;

public class AwsDnsUpdaterService : IDnsUpdater
{
	private readonly DnsUpdaterOptions _options;
	private readonly IPublicIpProvider _ipProvider;

	public AwsDnsUpdaterService(IOptions<DnsUpdaterOptions> options, IPublicIpProvider ipProvider)
	{
		this._options = options.Value;
		this._ipProvider = ipProvider;
	}

	public async Task UpdateDnsEntry(string zone, string record, CancellationToken cancellationToken = default)
	{
		var ip = await this._ipProvider.GetPublicIPAsync();

		var client = new AmazonRoute53Client(RegionEndpoint.GetBySystemName(this._options.Region));
		var records = new List<ResourceRecord>() { new ResourceRecord(ip) };

		var recordSet = new ResourceRecordSet(record, RRType.A)
		{
			TTL = (long)this._options.TTL.TotalSeconds,
			ResourceRecords = records
		};

		Change change = new Change(ChangeAction.UPSERT, recordSet);

		ChangeResourceRecordSetsRequest request = new ChangeResourceRecordSetsRequest(zone, new ChangeBatch(new List<Change>() { change }));

		ChangeResourceRecordSetsResponse response = await client.ChangeResourceRecordSetsAsync(request);
		if (response.HttpStatusCode != HttpStatusCode.OK)
			throw new Exception("");
	}
}
