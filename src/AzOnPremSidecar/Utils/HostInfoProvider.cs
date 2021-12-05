using Hardware.Info;

namespace AzOnPremSidecar.Utils;

public interface IHostInfoProvider
{
	Task<string> GetPublicIPAsync();
	string GetHostName();
	double GetCpuUsagePercentage();
	double GetAvailableMemoryPercentage();
}

public class WindowsHostInfoProvider : IHostInfoProvider
{
	private readonly HttpClient _client;
	private readonly IHardwareInfo _hardwareInfo = new HardwareInfo();

	public WindowsHostInfoProvider(HttpClient client)
	{
		this._client = client;
	}

	public Task<string> GetPublicIPAsync()
	{
		return this._client.GetStringAsync("https://ifconfig.me");
	}

	public string GetHostName()
	{
		return Environment.MachineName;
	}

	public double GetCpuUsagePercentage()
	{
		this._hardwareInfo.RefreshCPUList();
		var cpuCount = this._hardwareInfo.CpuList.Count;

		return this._hardwareInfo.CpuList.Sum(i => (double)i.PercentProcessorTime) / (double)cpuCount / (double)100;
	}

	public double GetAvailableMemoryPercentage()
	{
		this._hardwareInfo.RefreshMemoryStatus();
		var result = (double)this._hardwareInfo.MemoryStatus.AvailablePhysical / (double)this._hardwareInfo.MemoryStatus.TotalPhysical;

		return Math.Round(result, 2);
	}
}
