namespace Probe.Tests
{
    using Microsoft.AspNetCore.Builder.Internal;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Probe.Middleware;
    using Probe.Service;
    using Shouldly;
    using System;
    using System.Reflection;
    using Xunit;

    public class ApplicationBuilderExtensionTests
    {
        public ApplicationBuilderExtensionTests()
        {
            
            

        }

        [Fact]
        public void Should_error_when_services_have_not_been_configured()
        {
            var services = new ServiceCollection();
            var appBuilder = new ApplicationBuilder(services.BuildServiceProvider());
            Should.Throw<InvalidOperationException>(() => appBuilder.UseProbeDiagnostics());
        }

        [Fact]
        public void Adds_probe_service_middleware_to_pipeline()
        {
            var services = new ServiceCollection();
            var options = new ProbeOptions { UseWebClient = false };
            services.AddSingleton<ProbeOptions>(options);
            services.AddSingleton<IProbeService>(new ProbeService(services.BuildServiceProvider()));
            var appBuilder = new ApplicationBuilder(services.BuildServiceProvider());

            appBuilder.UseProbeDiagnostics();
            var requestDelegate = appBuilder.Build();

            requestDelegate.Target.ShouldBeOfType(typeof(ProbeWebServerMiddleware));
        }

        [Fact]
        public void Adds_web_client_middleware_to_pipeline()
        {
            var services = new ServiceCollection();
            var options = new ProbeOptions();
            services.AddSingleton<ProbeOptions>(options);
            services.AddSingleton<IProbeService>(new ProbeService(services.BuildServiceProvider()));
            services.AddSingleton<IWebClientService>(new WebClientResourcesService());
            var appBuilder = new ApplicationBuilder(services.BuildServiceProvider());
            // Use a little reflection for discovery of the aspnetcore request pipeline
            var nextField = typeof(ProbeWebServerMiddleware).GetField("next", BindingFlags.Instance | BindingFlags.NonPublic);

            appBuilder.UseProbeDiagnostics();
            var requestDelegate = appBuilder.Build();

            requestDelegate.Target.ShouldBeOfType(typeof(ProbeWebServerMiddleware));
            var server = requestDelegate.Target as ProbeWebServerMiddleware;
            // Next prop passes the next part of the pipeline
            var nextRequestDelegate = (RequestDelegate)nextField.GetValue(server);
            nextRequestDelegate.Target.ShouldBeOfType(typeof(ProbeWebClientMiddleware));
        }
    }
}
