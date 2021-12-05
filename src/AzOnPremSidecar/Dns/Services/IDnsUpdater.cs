namespace AzOnPremSideca.Dns.Services;

public interface IDnsUpdater
{
	Task UpdateDnsEntry(string zone, string record, CancellationToken cancellationToken = default);
}
