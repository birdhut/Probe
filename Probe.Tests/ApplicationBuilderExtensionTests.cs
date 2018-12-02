namespace Probe.Tests
{
    using Microsoft.AspNetCore.Builder.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Shouldly;
    using System;
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
