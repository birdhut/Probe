namespace Probe.Example.Probes
{
    using Probe;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

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
