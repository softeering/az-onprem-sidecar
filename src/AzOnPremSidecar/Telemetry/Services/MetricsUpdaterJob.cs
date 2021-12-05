namespace AzOnPremSidecar.Telemetry.Services;

using AzOnPremSidecar.Telemetry.Config;
using AzOnPremSidecar.Utils;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

public class MetricsUpdaterJob : BackgroundService
{
	private readonly MetricsUpdaterOptions _options;
	private readonly IHostInfoProvider _hostInfoProvider;
	private readonly IMetricsPublisher _metricsPublisher;

	public MetricsUpdaterJob(IOptions<MetricsUpdaterOptions> options, IHostInfoProvider hostInfoProvider, IMetricsPublisher metricsPublisher)
	{
		this._options = options.Value;
		this._hostInfoProvider = hostInfoProvider;
		this._metricsPublisher = metricsPublisher;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				var hostName = this._hostInfoProvider.GetHostName();
				var cpuUsage = this._hostInfoProvider.GetCpuUsagePercentage();
				var availableMemory = this._hostInfoProvider.GetAvailableMemoryPercentage();

				// TODO send metrics to external system (Azure insights, AWS cloudwatch...)


			}
			finally
			{
				await Task.Delay(this._options.Interval.GetValueOrDefault(TimeSpan.FromMinutes(1)), stoppingToken);
			}
		}
	}
}
