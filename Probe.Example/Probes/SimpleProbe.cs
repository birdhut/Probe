namespace Probe.Example.Probes
{
    using Probe;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class SimpleProbe : IProbe
    {
        private readonly HashSet<ProbeArg> args = new HashSet<ProbeArg>();

        public string Id => "Simple";

        public string Description => "Simple Test Probe";

        public ISet<ProbeArg> Args => args;

        public Task<object> OnHandle(ProbeRunArgs args)
        {
            object result = new { Task = this.Id, Value = "Success" };
            return Task.FromResult(result);
        }
    }
    }
