using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WASM_Reatime_D3.Contracts;
using WASM_Reatime_D3.Services;
using WASM_Reatime_D3;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using ApexCharts;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri("http://localhost:8080/api/Tbllog/") });
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddPWAUpdater();
builder.Services.AddApexCharts();
await builder.Build().RunAsync();