namespace Probe.Example
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Probe;

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

    public class ConfigurationProbe : IProbe
    {
        private readonly HashSet<ProbeArg> args = new HashSet<ProbeArg>();
        private readonly IConfiguration configuration;

        public ConfigurationProbe(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Id => "Configuration";

        public string Description => "Probe to return the current application configuration";

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

    public class SlowProbe : IProbe
    {
        private readonly HashSet<ProbeArg> args = new HashSet<ProbeArg>();

        public SlowProbe()
        {
            args.Add(new ProbeArg("count", ProbeArgType.Number, true));
            args.Add(new ProbeArg("delay", ProbeArgType.Number, true));
        }
        public string Id => "Slow";

        public string Description => "Probe to iterate a specified number of counts using the specified delay";

        public ISet<ProbeArg> Args => args;

        public async Task<dynamic> OnHandle(ProbeRunArgs args)
        {
            DateTime start = DateTime.UtcNow;

            var count = Math.Round(args.ParseDoubleNumberArg("count"), 0);
            var delay = Math.Round(args.ParseDoubleNumberArg("delay"), 0);
            if (count < 1)
            {
                count = 1;
            }

            if (delay < 1)
            {
                delay = 1;
            }

            var span = TimeSpan.FromSeconds(delay);
            

            for (int i = 0; i < count; i++)
            {
                await Task.Delay(span);
            }

            DateTime end = DateTime.UtcNow;
            object result = new { ExecutionCount = count, DelaySeconds = delay, TotalSecondsOnServer = (end - start).TotalSeconds };

            return  await Task.FromResult(result);
        }
    }
}
