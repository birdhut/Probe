namespace Probe
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Probe.Service;
    using System;

    /// <summary>
    /// Public Api for configuring Probe in an aspnetcore application
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the Probe service using the default options with no required dependencies, 
        /// or dependencies registered with the <see cref="IServiceCollection"/> independently prior to this call 
        /// and returns an instance of IProbeBuilder
        /// to allow registration of <see cref="IProbe"/>s
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> used in aspnetcore to register services</param>
        public static IProbeBuilder AddProbeDiagnostics(this IServiceCollection services)
        {
            return ConfigureProbeDiagnostics(services, null, null);
        }

        /// <summary>
        /// Configures the Probe service using the provided options with no required dependencies, 
        /// or dependencies registered with the <see cref="IServiceCollection"/> independently prior to this call 
        /// and returns an instance of IProbeBuilder
        /// to allow registration of <see cref="IProbe"/>s
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> used in aspnetcore to register services</param>
        /// <param name="configureOptions">An <see cref="Action{T1}"/> used to configure probe options</param>
        public static IProbeBuilder AddProbeDiagnostics(this IServiceCollection services, Action<ProbeOptions> configureOptions)
        {
            return ConfigureProbeDiagnostics(services, configureOptions, null);
        }

        /// <summary>
        /// Configures the Probe service using the default options and dependencies and returns an instance of IProbeBuilder
        /// to allow registration of <see cref="IProbe"/>s
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> used in aspnetcore to register services</param>
        /// <param name="registerProbeDependencies">An <see cref="Action{T1}"/> used to configure probe dependencies</param>
        public static IProbeBuilder AddProbeDiagnostics(this IServiceCollection services, Action<IServiceCollection> registerProbeDependencies)
        {
            return ConfigureProbeDiagnostics(services, null, registerProbeDependencies);
        }

        /// <summary>
        /// Configures the Probe service using the provided options and dependencies and returns an instance of IProbeBuilder
        /// to allow registration of <see cref="IProbe"/>s
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> used in aspnetcore to register services</param>
        /// <param name="configureOptions">An <see cref="Action{T1}"/> used to configure probe options</param>
        /// <param name="registerProbeDependencies">An <see cref="Action{T1}"/> used to configure probe dependencies</param>
        public static IProbeBuilder AddProbeDiagnostics(this IServiceCollection services, Action<ProbeOptions> configureOptions,
            Action<IServiceCollection> registerProbeDependencies)
        {
            return ConfigureProbeDiagnostics(services, configureOptions, registerProbeDependencies);
        }

        /// <summary>
        /// Validates that the provided <see cref="ProbeOptions"/> are correctly configured.
        /// </summary>
        /// <param name="options">The <see cref="ProbeOptions"/> to validate</param>
        private static void ValidateOptions(ProbeOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ProbeApiPath))
            {
                throw new ArgumentException(nameof(options));
            }

            PathString probeApi = new PathString(options.ProbeApiPath);
            options.ApiBase = probeApi;

            if (options.UseWebClient)
            {
                if (string.IsNullOrWhiteSpace(options.WebClientPath))
                {
                    throw new ArgumentException(nameof(options));
                }

                PathString probeWeb = new PathString(options.WebClientPath.TrimEnd('/'));
                options.ClientBase = probeWeb;
            }
        }

        /// <summary>
        /// Configures the Probe service using the provided options and dependencies and returns an instance of IProbeBuilder
        /// to allow registration of <see cref="IProbe"/>s
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> used in aspnetcore to register services</param>
        /// <param name="configureOptions">An <see cref="Action{T1}"/> used to configure probe options (optional)</param>
        /// <param name="registerProbeDependencies">An <see cref="Action{T1}"/> used to configure probe dependencies (optional)</param>
        /// <returns></returns>
        private static IProbeBuilder ConfigureProbeDiagnostics(IServiceCollection services, 
            Action<ProbeOptions> configureOptions = null,
            Action<IServiceCollection> registerProbeDependencies = null)
        {
            // Check and handle options
            var options = new ProbeOptions();
            configureOptions?.Invoke(options);
            ValidateOptions(options);
            services.AddSingleton(options);

            // Register any required dependencies (must be conducted before instantiating the service provider)
            registerProbeDependencies?.Invoke(services);

            // Build dependencies at current state for use in probe service
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            // Create probe service and register
            var probeService = new ProbeService(serviceProvider);
            services.AddSingleton<IProbeService>(probeService);

            // Create and register client service if configured for use
            if (options.UseWebClient)
            {
                var clientService = new WebClientResourcesService(options);
                services.AddSingleton<IWebClientService>(clientService);
            }

            return probeService;
        }
    }
}
