namespace Probe.Example.Probes
{
    using Microsoft.Extensions.Options;
    using Probe;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ConfigurationOptions
    {
        public bool BooleanOption { get; set; }
        public string StringOption { get; set; }
        public DateTime DateTimeOption { get; set; }
        public int NumberOption { get; set; }
    }

    public class ConfigurationOptionsProbe : IProbe
    {
        private readonly HashSet<ProbeArg> args = new HashSet<ProbeArg>();
        private readonly ConfigurationOptions configuration;

        public ConfigurationOptionsProbe(IOptions<ConfigurationOptions> configuration)
        {
            this.configuration = configuration.Value;
        }

        public string Id => "Options";

        public string Description => "Probe to return the specific ConfigurationOptions configuration configured in startup";

        public ISet<ProbeArg> Args => args;

        public Task<dynamic> OnHandle(ProbeRunArgs args)
        {
            object result = configuration;
            return Task.FromResult(result);
        }
    }
}
