using Compendium;
using Compendium.Database;
using Compendium.Models.CoreData;
using Compendium.Provider;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    o.SerializerSettings.Converters = new List<JsonConverter>
                                {
                                    new ObjectIdConverter(),
                                    new StringEnumConverter()
                                };
    o.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
});

var services = builder.Services;

var dbConnection = new JsonDatabaseConnection();
services.AddSingleton<IDatabaseConnection>(dbConnection);

var dynamicEnumProvider = new DynamicEnumProvider(dbConnection);
services.AddSingleton(dynamicEnumProvider);

var monsterProvider = new ProjectDocumentProvider<Monster>(dbConnection);
services.AddSingleton<Provider<Monster>>(monsterProvider);

var spellProvider = new ProjectDocumentProvider<Spell>(dbConnection);
services.AddSingleton<Provider<Spell>>(spellProvider);

var projectProvider = new ProjectProvider(dbConnection, monsterProvider, spellProvider);
projectProvider.RegisterAll();
services.AddSingleton(projectProvider);

var weaponTypeProvider = new WeaponTypeProvider(dbConnection);
services.AddSingleton<Provider<WeaponType>>(weaponTypeProvider);

new DataLoader(spellProvider, monsterProvider, projectProvider, dynamicEnumProvider).LoadData();

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
