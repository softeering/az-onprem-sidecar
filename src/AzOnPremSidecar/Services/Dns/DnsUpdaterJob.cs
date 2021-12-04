using Microsoft.Extensions.Options;
using SoftEEring.Core.Helpers;

namespace AzOnPremSidecar.Services.Dns;

public class DnsUpdaterJob : BackgroundService
{
	private readonly ILogger<DnsUpdaterJob> _logger;
	private readonly DnsUpdaterOptions _options;
	private readonly IDnsUpdater _dnsUpdater;

	public DnsUpdaterJob(ILogger<DnsUpdaterJob> logger, IOptions<DnsUpdaterOptions> options, IDnsUpdater dnsUpdater)
	{
		this._logger = logger;
		this._options = options.Value;
		this._dnsUpdater = dnsUpdater;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (this._options.Records is null)
		{
			this._logger.LogWarning("No records set in the Dns updater config. Job won't update any record");
			return;
		}

		while (!stoppingToken.IsCancellationRequested)
		{
			var interval = this._options.Interval;

			try
			{
				foreach (var record in this._options.Records)
				{
					try
					{
						if (record.Zone is null || record.Record is null)
						{
							this._logger.LogError("Cannot update the Dns entry since Zone and / or Record are not set");
						}
						else
						{
							this._logger.LogInformation($"Updating Dns entry {record.Zone} / {record.Record}...");
							await RetryUtil.WithRetryAsync(() => this._dnsUpdater.UpdateDnsEntry(record.Zone, record.Record));
							this._logger.LogInformation($"Done updating Dns entry {record.Zone} / {record.Record}");
						}
					}
					catch (Exception error)
					{
						this._logger.LogError(error, $"An error occurred while updating Dns entry {record.Zone} / {record.Record}");
						// we divide the interval by 2 since the update failed
						interval /= 2;
					}
				}
			}
			finally
			{
				await Task.Delay(interval, stoppingToken);
			}
		}
	}
}
