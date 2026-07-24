using GameHub.Web.Components;
using GameHub.Web.Options;
using GameHub.Web.Services.Abstractions;
using GameHub.Web.Services.Api;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using GameHub.Web.State;
using GameHub.Web.Services.Authentication;
using GameHub.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<UserSession>();
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy(
        AuthorizationPolicies.AdminOnly,
        policy => policy.RequireRole(
            GameHubRoles.Admin));
});

builder.Services.AddCascadingAuthenticationState();

builder.Services
    .AddScoped<GameHubAuthenticationStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider>(
    serviceProvider => serviceProvider
        .GetRequiredService<GameHubAuthenticationStateProvider>());

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<IUserSessionStorage, ProtectedUserSessionStorage>();

builder.Services
    .AddOptions<ApiOptions>()
    .Bind(builder.Configuration.GetSection(ApiOptions.SectionName))
    .Validate(
        options => Uri.TryCreate(
            options.BaseUrl,
            UriKind.Absolute,
            out _),
        "A valid absolute API BaseUrl must be configured.");

builder.Services.AddScoped<IApiHealthService, ApiHealthService>();
builder.Services.AddHttpClient<IApiClient, ApiClient>(
    ConfigureApiHttpClient);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AllowAnonymous();

app.Run();

static void ConfigureApiHttpClient(
    IServiceProvider serviceProvider,
    HttpClient httpClient)
{
    var apiOptions = serviceProvider
        .GetRequiredService<IOptions<ApiOptions>>()
        .Value;

    httpClient.BaseAddress = new Uri(apiOptions.BaseUrl);
    httpClient.Timeout = TimeSpan.FromSeconds(30);
}
