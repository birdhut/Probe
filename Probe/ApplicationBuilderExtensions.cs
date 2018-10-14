namespace Probe
{
    using Microsoft.AspNetCore.Builder;
    using Probe.Middleware;
    using Probe.Service;
    using System;

    /// <summary>
    /// Public Api for installing Probe into an aspnetcore Application
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Installs the middleware required to use the Probe Services in an aspnetcore pipeline
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the probe service has not been configured 
        /// during the call to ConfigureServices in the aspnetcore startup</exception>
        /// <param name="app">The <see cref="IApplicationBuilder"/> used to build the aspnetcore middleware pipeline</param>
        /// <returns>The <see cref="IApplicationBuilder"/> configured with the required Probe Middleware</returns>
        public static IApplicationBuilder UseProbeDiagnostics(this IApplicationBuilder app)
        {
            var service = (ProbeService)app.ApplicationServices.GetService(typeof(ProbeService));
            if (service == null)
            {
                throw new InvalidOperationException("Probe has not been configured");
            }

            var options = (ProbeOptions)app.ApplicationServices.GetService(typeof(ProbeOptions));
            if (options == null)
            {
                throw new InvalidOperationException("Probe has not been configured");
            }

            UseProbeDiagnostics(app, options, service);

            return app;
        }

        /// <summary>
        /// Adds the Probe Api Middleware and optionally the Probe Web Client Middleware to the application builder
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> used to build the aspnetcore middleware pipeline</param>
        /// <param name="options">The <see cref="ProbeOptions"/> configured</param>
        /// <param name="service"></param>
        private static void UseProbeDiagnostics(IApplicationBuilder app, ProbeOptions options, ProbeService service)
        {
            // Generate the Probe Types
            service.BuildProbes(); 

            app.UseMiddleware<ProbeWebServerMiddleware>(service, options);

            if (options.UseWebClient)
            {                
                app.UseMiddleware<ProbeWebClientMiddleware>(options);
            }
        }
    }
}
