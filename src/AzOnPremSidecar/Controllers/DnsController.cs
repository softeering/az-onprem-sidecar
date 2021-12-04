using AzOnPremSidecar.Services.Dns;
using Microsoft.AspNetCore.Mvc;

namespace AzOnPremSidecar.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DnsController : ControllerBase
	{
		private readonly IPublicIpProvider _ipProvider;

		public DnsController(IPublicIpProvider ipProvider)
		{
			this._ipProvider = ipProvider;
		}

		[HttpGet("publicip")]
		public async Task<IActionResult> GetPublicIP()
		{
			var ip = await this._ipProvider.GetPublicIPAsync();
			return new ObjectResult(ip);
		}
	}
}
