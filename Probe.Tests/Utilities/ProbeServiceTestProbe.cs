namespace Probe.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

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
}
