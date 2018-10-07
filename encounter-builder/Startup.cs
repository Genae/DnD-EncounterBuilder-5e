using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using encounter_builder.Database;
using encounter_builder.Parser;
using encounter_builder.Provider;
using LiteDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson.Converters;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace encounter_builder
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            }); 

            services.TryAddSingleton<DataProvider>();
            services.TryAddSingleton<IDatabaseConnection, LiteDbConnection>();
            services.TryAddScoped<ActionParser>();
            services.TryAddScoped<MonsterParser>();
            services.TryAddScoped<SpellcastingParser>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Use(async (context, next) => {
                await next();
                if (context.Response.StatusCode == 404 &&
                   !Path.HasExtension(context.Request.Path.Value) &&
                   !context.Request.Path.Value.StartsWith("/api/"))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });

            app.UseMvcWithDefaultRoute();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            JsonConvert.DefaultSettings = (() =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters = new List<JsonConverter>
                {
                    new ObjectIdConverter(),
                    new StringEnumConverter { CamelCaseText = true }
                };
                return settings;
            });

            #if DEBUG
            var p = new ProcessStartInfo()
            {
                Arguments = " /c npm run serve",
                CreateNoWindow = true,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                FileName = "cmd.exe"
            };
            new Process()
            {
                StartInfo = p
            }.Start();
            #endif
        }
    }

    public class ObjectIdConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new ObjectId((string) existingValue);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ObjectId).IsAssignableFrom(objectType);
        }
    }
}
