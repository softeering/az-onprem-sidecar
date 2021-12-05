using AzOnPremSideca.Dns.Services;
using AzOnPremSidecar.Dns.Config;
using AzOnPremSidecar.Telemetry.Config;
using AzOnPremSidecar.Telemetry.Services;
using AzOnPremSidecar.Utils;
using Microsoft.Extensions.Hosting.WindowsServices;

var options = new WebApplicationOptions
{
	Args = args,
	ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);
builder.Host.UseWindowsService();

builder.Services.Configure<DnsUpdaterOptions>(builder.Configuration.GetSection("DnsUpdater"));
var dnsUpdaterOptions = builder.Configuration.GetSection("DnsUpdater").Get<DnsUpdaterOptions>();

builder.Services.AddOptions();
builder.Services.AddHttpClient<IHostInfoProvider, WindowsHostInfoProvider>(); // .AddTransientHttpErrorPolicy(policy => policy.RetryAsync(3));

if (dnsUpdaterOptions?.Enabled ?? false)
{
	_ = dnsUpdaterOptions.Provider.ToLower() switch
	{
		"azure" => builder.Services.AddSingleton<IDnsUpdater, AzureDnsUpdaterService>(),
		"aws" => builder.Services.AddSingleton<IDnsUpdater, AwsDnsUpdaterService>(),
		_ => throw new NotImplementedException($"Unknown Dns updater provider {dnsUpdaterOptions.Provider}")
	};

	builder.Services.AddHostedService<DnsUpdaterJob>();
}

builder.Services.Configure<MetricsUpdaterOptions>(builder.Configuration.GetSection("MetricsUpdater"));
var metricsUpdaterOptions = builder.Configuration.GetSection("MetricsUpdater").Get<MetricsUpdaterOptions>();

if (metricsUpdaterOptions?.Enabled ?? false)
{
	_ = metricsUpdaterOptions.Provider.ToLower() switch
	{
		"azure" => builder.Services.AddSingleton<IMetricsPublisher, AzureMonitorMetricsPublisher>(),
		"aws" => builder.Services.AddSingleton<IMetricsPublisher, AwsCloudwatchMetricsPublisher>(),
		_ => throw new NotImplementedException($"Unknown Metrics publisher {metricsUpdaterOptions.Provider}")
	};

	builder.Services.AddHostedService<MetricsUpdaterJob>();
}

builder.Services.AddControllers();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
