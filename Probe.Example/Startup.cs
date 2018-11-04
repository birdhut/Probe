namespace Probe.Example
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Probe;

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            /* Probe dependencies could be registered to the service collection here if the 
             * service collection is passed in as a dependency */

            services.AddProbeDiagnostics(options =>
                {
                    options.UseWebClient = true;
                },
                s =>
                {
                    /* Probe dependencies could be registered here whether the service collection 
                     * was passed as a dependency or not */
                })
                .AddProbe<SimpleProbe>()
                .AddProbe<ConfigurationProbe>()
                .AddProbe<SlowProbe>()
                .AddProbe<ParametersProbe>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseProbeDiagnostics();
        }
    }
}
