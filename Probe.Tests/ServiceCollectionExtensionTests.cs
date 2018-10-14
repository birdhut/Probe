namespace Probe.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Probe;
    using Probe.Service;
    using Shouldly;
    using System;
    using Xunit;

    public class ServiceCollectionExtensionTests
    {
        private readonly IServiceCollection collection;
        private readonly ProbeOptions defaultOptions;

        public ServiceCollectionExtensionTests()
        {
            // not testing the capabilities of aspnetcore dependencies - 
            // we can use the concrete class for verification rather than a mock
            collection = new ServiceCollection();

             defaultOptions = new ProbeOptions();
        }

        [Fact]
        public void Should_throw_on_invalid_options()
        {
            var nullApiPath = new ProbeOptions { ProbeApiPath = null };
            var nullClientPath = new ProbeOptions { UseWebClient = true, WebClientPath = null };
            // Path strings must begin with "/"
            var notPathStringApi = new ProbeOptions { ProbeApiPath = "api/probe" };
            var notPathStringClient = new ProbeOptions { UseWebClient = true, WebClientPath = "client" };

            Should.Throw<ArgumentException>(() => collection.AddProbeDiagnostics(SetProbeOptions(nullApiPath)));
            Should.Throw<ArgumentException>(() => collection.AddProbeDiagnostics(SetProbeOptions(nullClientPath)));
            Should.Throw<ArgumentException>(() => collection.AddProbeDiagnostics(SetProbeOptions(notPathStringApi)));
            Should.Throw<ArgumentException>(() => collection.AddProbeDiagnostics(SetProbeOptions(notPathStringClient)));
        }

        [Fact]
        public void Does_not_add_web_client_when_specified()
        {
            var noClient = new ProbeOptions { UseWebClient = false, WebClientPath = null };
            collection.AddProbeDiagnostics(SetProbeOptions(noClient));

            var (options, service, client, provider) = BuildExpectedServices();

            options.UseWebClient.ShouldBe(false);
            collection.Count.ShouldBe(2); // probe options, probe service, no other dependencies
            options.ProbeApiPath.ShouldBe(defaultOptions.ProbeApiPath);
        }

        [Fact]
        public void Adds_web_client_when_specified()
        {
            var withClient = new ProbeOptions { UseWebClient = true, WebClientPath = "/" };
            collection.AddProbeDiagnostics(SetProbeOptions(withClient));

            var (options, service, client, provider) = BuildExpectedServices();
            options.UseWebClient.ShouldBe(withClient.UseWebClient);
            options.WebClientPath.ShouldBe(withClient.WebClientPath);
            collection.Count.ShouldBe(3); // probe options, web client service, probe service, no other dependencies
            options.ProbeApiPath.ShouldBe(defaultOptions.ProbeApiPath);
        }

        [Fact]
        public void Should_add_services_with_default_options_and_no_dependencies()
        {
            collection.AddProbeDiagnostics();
            var (options, service, client, provider) = BuildExpectedServices();

            collection.Count.ShouldBe(3); // probe options, web client service, probe service, no other dependencies
            options.ProbeApiPath.ShouldBe(defaultOptions.ProbeApiPath);
            options.WebClientPath.ShouldBe(defaultOptions.WebClientPath);
            options.UseWebClient.ShouldBe(defaultOptions.UseWebClient);
        }

        [Fact]
        public void Should_add_services_with_configured_options_and_no_dependencies()
        {
            var noClient = new ProbeOptions { UseWebClient = false, WebClientPath = null };
            collection.AddProbeDiagnostics(SetProbeOptions(noClient));
            var (options, service, client, provider) = BuildExpectedServices();

            collection.Count.ShouldBe(2); // probe options, probe service, no other dependencies
            options.ProbeApiPath.ShouldBe(noClient.ProbeApiPath);
            options.WebClientPath.ShouldBeNull();
            options.UseWebClient.ShouldBe(noClient.UseWebClient);
            provider.GetService(typeof(ISimpleDependency)).ShouldBeNull();
        }

        [Fact]
        public void Should_support_pre_configured_dependencies()
        {
            collection.AddSingleton<ISimpleDependency>(new SimpleDependency());
            collection.AddProbeDiagnostics();
            var (options, service, client, provider) = BuildExpectedServices();

            collection.Count.ShouldBe(4); // probe options, web client service, probe service, ISimpleDependency
            options.ProbeApiPath.ShouldBe(defaultOptions.ProbeApiPath);
            options.WebClientPath.ShouldBe(defaultOptions.WebClientPath);
            options.UseWebClient.ShouldBe(defaultOptions.UseWebClient);
            provider.GetService(typeof(ISimpleDependency)).ShouldNotBeNull();
        }

        [Fact]
        public void Should_add_services_with_default_options_and_configured_dependencies()
        {
            collection.AddProbeDiagnostics(s => s.AddSingleton<ISimpleDependency>(new SimpleDependency()));
            var (options, service, client, provider) = BuildExpectedServices();

            collection.Count.ShouldBe(4); // probe options, web client service, probe service, ISimpleDependency
            options.ProbeApiPath.ShouldBe(defaultOptions.ProbeApiPath);
            options.WebClientPath.ShouldBe(defaultOptions.WebClientPath);
            options.UseWebClient.ShouldBe(defaultOptions.UseWebClient);
            provider.GetService(typeof(ISimpleDependency)).ShouldNotBeNull();
        }

        [Fact]
        public void Should_add_services_with_configured_options_and_configured_dependencies()
        {
            var noClient = new ProbeOptions { UseWebClient = false, WebClientPath = null };
            collection.AddProbeDiagnostics(SetProbeOptions(noClient), s => s.AddSingleton<ISimpleDependency>(new SimpleDependency()));
            var (options, service, client, provider) = BuildExpectedServices();

            collection.Count.ShouldBe(3); // probe options, probe service, ISimpleDependency
            options.ProbeApiPath.ShouldBe(noClient.ProbeApiPath);
            options.WebClientPath.ShouldBeNull();
            options.UseWebClient.ShouldBe(noClient.UseWebClient);
            provider.GetService(typeof(ISimpleDependency)).ShouldNotBeNull();
        }

        private Action<ProbeOptions> SetProbeOptions(ProbeOptions desiredOptions)
        {
            return 
                new Action<ProbeOptions>(
                    (passedOptions) => 
                    {
                        passedOptions.ProbeApiPath = desiredOptions.ProbeApiPath;
                        passedOptions.UseWebClient = desiredOptions.UseWebClient;
                        passedOptions.WebClientPath = desiredOptions.WebClientPath;
                    }
                );
        }

        private (ProbeOptions options, ProbeService service, 
            WebClientResourcesService client, IServiceProvider provider) BuildExpectedServices()
        {
            var provider = collection.BuildServiceProvider();
            var options = (ProbeOptions)provider.GetService(typeof(ProbeOptions));
            var service = (ProbeService)provider.GetService(typeof(IProbeService));
            var client = (WebClientResourcesService)provider.GetService(typeof(IWebClientService));

            return (options, service, client, provider);
        }

    }
}
