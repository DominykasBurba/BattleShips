using BattleShips.Components;

var builder = WebApplication.CreateBuilder(args);

// Razor Components + interactive (server) mode
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR();

// your DI
builder.Services.AddSingleton<BattleShips.Services.PlacementService>();
builder.Services.AddTransient<BattleShips.Services.IFleetPlacer, BattleShips.Services.RandomFleetPlacerAdapter>();
builder.Services.AddScoped<BattleShips.Services.GameService>();
builder.Services.AddSingleton<BattleShips.Services.ChatService>();
builder.Services.AddSingleton<BattleShips.Services.GameLobbyService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();

// 👇 This must be the ONLY UI host mapping
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<BattleShips.Hubs.GameHub>("/gamehub");

app.Run();