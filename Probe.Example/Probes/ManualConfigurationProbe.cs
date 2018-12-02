namespace Probe.Example.Probes
{
    using Microsoft.Extensions.Configuration;
    using Probe;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ManualConfigurationProbe : IProbe
    {
        private readonly HashSet<ProbeArg> args = new HashSet<ProbeArg>();
        private readonly IConfiguration configuration;

        public ManualConfigurationProbe(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string Id => "Configuration";

        public string Description => "Probe to return specific values in the current application configuration taking IConifguration object as a dependency";

        public ISet<ProbeArg> Args => args;

        public Task<dynamic> OnHandle(ProbeRunArgs args)
        {
            var conn = configuration.GetConnectionString("DefaultConnection");
            var log = configuration.GetSection("Logging").GetSection("LogLevel").GetValue<string>("Default");
            var hosts = configuration.GetValue<string>("AllowedHosts");
            object result = new { AllowedHosts = hosts, LogLevelDefault = log, DefaultConnection = conn };

            return Task.FromResult(result);
        }
    }
}
