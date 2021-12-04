namespace AzOnPremSidecar.Services.Telemetry;

using Microsoft.Extensions.Options;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Threading.Tasks;

public class MetricsUpdaterJob : BackgroundService
{
	private readonly MetricsUpdaterOptions _options;

	public MetricsUpdaterJob(IOptions<MetricsUpdaterOptions> options)
	{
		this._options = options.Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{

			}
			finally
			{
				await Task.Delay(5000, stoppingToken);
			}
		}
	}
}
