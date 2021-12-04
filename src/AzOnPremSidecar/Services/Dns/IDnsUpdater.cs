namespace AzOnPremSidecar.Services.Dns;

public interface IDnsUpdater
{
	Task UpdateDnsEntry(string zone, string record, CancellationToken cancellationToken = default);
}
