using GameHub.Web.Components;
using GameHub.Web.Options;
using Microsoft.Extensions.Options;
using GameHub.Web.Services.Abstractions;
using GameHub.Web.Services.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<ApiOptions>()
    .Bind(builder.Configuration.GetSection(ApiOptions.SectionName))
    .Validate(
        options => Uri.TryCreate(
            options.BaseUrl,
            UriKind.Absolute,
            out _),
        "A valid absolute API BaseUrl must be configured.");
//.ValidateOnStart();

builder.Services.AddHttpClient<IApiHealthService, ApiHealthService>(
    (serviceProvider, httpClient) =>
    {
        var apiOptions = serviceProvider
            .GetRequiredService<IOptions<ApiOptions>>()
            .Value;

        httpClient.BaseAddress = new Uri(apiOptions.BaseUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(30);
    });

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
