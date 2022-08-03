using AspNetCoreRateLimit;
using Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MusicBrainzApi.Middleware;
using Polly;
using Polly.Extensions.Http;
using Services;
using System;

namespace MusicBrainzApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError() 
                                       .WaitAndRetryAsync(GetMusicBrainzMaxRetry(), retryAttempt => TimeSpan.FromSeconds(retryAttempt));

            services.AddHttpClient("MusicBrainz", client =>
            {
                client.BaseAddress = new Uri(GetMusicBrainzBaseUrl());
                client.DefaultRequestHeaders.Add("User-Agent", GetMusicBrainzUserAgent());
            }).AddPolicyHandler(retryPolicy);

            services.AddScoped<IArtistService, ArtistService>();

            services.AddControllers();

            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(GetRateLimitingConfig());
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            services.AddInMemoryRateLimiting();

            services.AddRedisCache("");

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Coles MusicBrainz Api", Version = "v1" });
            });
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MusicBrainz Api v1"));
            }

            app.UseMiddleware<HealthCheckMiddleware>();

            app.UseHttpsRedirection();

            app.UseIpRateLimiting();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private string GetMusicBrainzBaseUrl()
        {
            return Configuration.GetSection("MusicBrainz:BaseUrl").Get<string>();
        }

        private string GetMusicBrainzUserAgent()
        {
            return Configuration.GetSection("MusicBrainz:UserAgent").Get<string>();
        }
        private int GetMusicBrainzMaxRetry()
        {
            return Configuration.GetSection("MusicBrainz:MaxRetry").Get<int>();
        }
        private IConfigurationSection GetRateLimitingConfig()
        {
            return Configuration.GetSection("IpRateLimiting");
        }

    }
}
