namespace Probe.Tests
{
    using Moq;
    using Probe.Service;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class ProbeServiceTests
    {
        private readonly ProbeService service;
        private readonly Mock<IServiceProvider> providerMock;

        public ProbeServiceTests()
        {
            providerMock = new Mock<IServiceProvider>();
            var type = typeof(ISimpleDependency);
            providerMock
                .Setup(x => x.GetService(It.Is<Type>(t => t == type)))
                .Returns(new SimpleDependency())
                .Verifiable();
            service = new ProbeService(providerMock.Object);
        }

        [Fact]
        public void Can_add_probes_to_builder()
        {
            service
                .AddProbe<ProbeServiceTestProbe>()
                .AddProbe<ProbeServiceTestProbeWithDependencies>();

            service.RegisteredProbes.ShouldContain(typeof(ProbeServiceTestProbe));
            service.RegisteredProbes.ShouldContain(typeof(ProbeServiceTestProbeWithDependencies));
            service.ProbeInstances.ShouldBeEmpty();
        }

        [Fact]
        public void Can_build_probes_without_dependencies()
        {
            service.AddProbe<ProbeServiceTestProbe>();
            var probeTestServiceKey = "ProbeServiceTestProbe".ToLower();

            service.BuildProbes();

            service.RegisteredProbes.ShouldBeEmpty();
            service.ProbeInstances.Count.ShouldBe(1);
            service.ProbeInstances.ContainsKey(probeTestServiceKey).ShouldBe(true);
            service.ProbeInstances[probeTestServiceKey].ShouldBeOfType(typeof(ProbeServiceTestProbe));
        }

        [Fact]
        public void Can_build_probes_with_working_dependencies()
        {
            service.AddProbe<ProbeServiceTestProbeWithDependencies>();
            string dependencyTest = new SimpleDependency().DependencyTest();
            var probeTestServiceDependenciesId = "ProbeServiceTestProbeWithDependencies";
            var probeTestServiceDependenciesKey = probeTestServiceDependenciesId.ToLower();

            service.BuildProbes();

            providerMock.Verify(x => x.GetService(typeof(ISimpleDependency)), Times.Exactly(1));
            service.RegisteredProbes.ShouldBeEmpty();
            service.ProbeInstances.Count.ShouldBe(1);
            service.ProbeInstances.ContainsKey(probeTestServiceDependenciesKey).ShouldBe(true);
            service.ProbeInstances[probeTestServiceDependenciesKey].ShouldBeOfType(typeof(ProbeServiceTestProbeWithDependencies));
            dynamic result = service.ProbeInstances[probeTestServiceDependenciesKey]
                .OnHandle(new ProbeRunArgs())
                .GetAwaiter()
                .GetResult();
            string dependencyText = result.DependencyText;
            dependencyText.ShouldBe(dependencyTest);
        }

        [Fact]
        public void Can_query_all_probes()
        {
            service
                .AddProbe<ProbeServiceTestProbe>()
                .AddProbe<ProbeServiceTestProbeWithDependencies>();
            service.BuildProbes();
            var probeTestServiceId = "ProbeServiceTestProbe";
            var probeTestServiceDependenciesId = "ProbeServiceTestProbeWithDependencies";

            var allProbes = service.GetAllProbes().ToList();

            allProbes.Count.ShouldBe(2);
            allProbes.Where(x => x.Id == probeTestServiceDependenciesId).ShouldNotBeNull();
            allProbes.Where(x => x.Id == probeTestServiceId).ShouldNotBeNull();
        }

        [Fact]
        public void Can_query_single_probe()
        {
            service
                .AddProbe<ProbeServiceTestProbe>()
                .AddProbe<ProbeServiceTestProbeWithDependencies>();
            service.BuildProbes();
            var probeTestServiceDependenciesId = "ProbeServiceTestProbeWithDependencies";
            var probeTestServiceDependenciesKey = probeTestServiceDependenciesId.ToLower();

            var dependencyProbe = service.GetProbe(probeTestServiceDependenciesKey);

            dependencyProbe.ShouldNotBeNull();
            dependencyProbe.Id.ShouldBe(probeTestServiceDependenciesId);
        }
    }

    public class ProbeServiceTestProbe : IProbe
    {
        public string Id => "ProbeServiceTestProbe";

        public string Description => "ProbeServiceTestProbe";

        public ISet<ProbeArg> Args => new HashSet<ProbeArg>();

        public Task<dynamic> OnHandle(ProbeRunArgs args)
        {
            object result = new { Id, Description };
            return Task.FromResult(result);
        }
    }

    public interface ISimpleDependency
    {
        string DependencyTest();
    }

    public class SimpleDependency : ISimpleDependency
    {
        public string DependencyTest()
        {
            return "DependencyWorks";
        }
    }

    public class ProbeServiceTestProbeWithDependencies : IProbe
    {
        private readonly ISimpleDependency dependency;

        public ProbeServiceTestProbeWithDependencies(ISimpleDependency dependency)
        {
            this.dependency = dependency;
        }

        public string Id => "ProbeServiceTestProbeWithDependencies";

        public string Description => "ProbeServiceTestProbeWithDependencies";

        public ISet<ProbeArg> Args => new HashSet<ProbeArg>();

        public Task<dynamic> OnHandle(ProbeRunArgs args)
        {
            object result = new { Id, Description, DependencyText = dependency.DependencyTest() };
            return Task.FromResult(result);
        }
    }
}
