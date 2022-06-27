using Compendium.Database;
using Compendium.Parser;
using Compendium.Provider;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

var services = builder.Services;

services.AddSingleton<DataProvider>();
services.AddSingleton<IDatabaseConnection, JsonDatabaseConnection>();
services.AddScoped<ActionParser>();
services.AddScoped<MonsterParser>();
services.AddScoped<SpellcastingParser>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
