using Microsoft.Azure.Management.Dns;
using Microsoft.Azure.Management.Dns.Models;
using Microsoft.Extensions.Options;
using Microsoft.Rest.Azure.Authentication;

namespace AzOnPremSidecar.Services.Dns;

public interface IPublicIpProvider
{
	Task<string> GetPublicIPAsync();
} 

public class PublicIpProvider : IPublicIpProvider
{
	private readonly HttpClient _client;

	public PublicIpProvider(HttpClient client)
	{
		this._client = client;
	}

	public Task<string> GetPublicIPAsync()
	{
		return this._client.GetStringAsync("https://ifconfig.me");
	}
}
