using AzOnPremSidecar.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AzOnPremSidecar.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HostInfoController : ControllerBase
	{
		private readonly IHostInfoProvider _hostInfoProvider;

		public HostInfoController(IHostInfoProvider hostInfoProvider)
		{
			this._hostInfoProvider = hostInfoProvider;
		}

		[HttpGet]
		public async Task<IActionResult> GetHostInfo()
		{
			var hostName = this._hostInfoProvider.GetHostName();
			var publicIP = await this._hostInfoProvider.GetPublicIPAsync();
			var cpuUsage = this._hostInfoProvider.GetCpuUsagePercentage();
			var availableMemory = this._hostInfoProvider.GetAvailableMemoryPercentage();

			return new ObjectResult(new
			{
				HostName = hostName,
				PublicIP = publicIP,
				CpuUsagePercentage = cpuUsage,
				AvailableMemoryPercentage = availableMemory
			});
		}
	}
}
