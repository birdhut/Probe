namespace Probe.Example
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Probe;
    using Probe.Example.Probes;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            /* Probe dependencies could be registered to the service collection here if the 
             * service collection is passed in as a dependency 
             *
             * We will configure the custom configuration options from the dependency passed in the constructor
             * for the ConfigurationOptionsProbe.  This requires nuget package Microsoft.Extensions.Options.ConfigurationExtensions.
             * Provided they are configured in the service collection,
             * the Probe builder will be able to instantiate the probe with the dependency.
             * The options can be changed in the appsettings.json file
             */
            var config = Configuration.GetSection("ConfigurationOptions");
            services.Configure<ConfigurationOptions>(config);

            services.AddProbeDiagnostics(options =>
                {
                    options.UseWebClient = true;
                },
                s =>
                {
                    /* Probe dependencies could also be registered here if the 
                     * service collection was not passed to the ConfigureServices method
                     */
                })
                .AddProbe<SimpleProbe>()
                .AddProbe<ManualConfigurationProbe>()
                .AddProbe<SlowProbe>()
                .AddProbe<ParametersProbe>()
                .AddProbe<ConfigurationOptionsProbe>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#pragma warning disable CA1822 // Mark members as static
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
#pragma warning restore CA1822 // Mark members as static
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseProbeDiagnostics();
        }
    }
}
