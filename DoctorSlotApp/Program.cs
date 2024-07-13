using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DoctorSlotApp;
using System.Net.Http;
using DoctorSlotApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.Configure<DoctorSlotServiceSettings>(builder.Configuration.GetSection("DoctorSlotServiceSettings"));

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<DoctorSlotService>();


// https://localhost:7016/api/Slots/availability/20240708
// https://localhost:7016/api/Slots/book
await builder.Build().RunAsync();
