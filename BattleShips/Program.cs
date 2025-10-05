using BattleShips.Components;

var builder = WebApplication.CreateBuilder(args);

// Razor Components + interactive (server) mode
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// your DI
builder.Services.AddSingleton<BattleShips.Services.PlacementService>();
builder.Services.AddSingleton<BattleShips.Services.GameService>();
builder.Services.AddSingleton<BattleShips.Services.ChatService>();

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

app.Run();