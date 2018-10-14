namespace Probe.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

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
