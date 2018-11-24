namespace Probe.Tests
{
    using Microsoft.AspNetCore.Builder.Extensions;
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
        [Fact]
        public void Should_error_when_services_have_not_been_configured()
        {
            var services = new ServiceCollection();
            var appBuilder = new ApplicationBuilder(services.BuildServiceProvider());
            Should.Throw<InvalidOperationException>(() => appBuilder.UseProbeDiagnostics());
        }
    }
}
