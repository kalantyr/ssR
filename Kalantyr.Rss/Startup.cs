using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Kalantyr.Rss
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            if (appBuilder == null) throw new ArgumentNullException(nameof(appBuilder));

            appBuilder.UseRouting();
            appBuilder.UseEndpoints(bld => bld.MapControllers());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddOptions();

            services
                .AddControllers()
                .AddNewtonsoftJson(opt => opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);

            services.AddSingleton<IRssService>(new RssService());
        }
    }
}
