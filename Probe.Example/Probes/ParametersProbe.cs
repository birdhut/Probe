namespace Probe.Example.Probes
{
    using Probe;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ParametersProbe : IProbe
    {
        private readonly HashSet<ProbeArg> args = new HashSet<ProbeArg>();

        public ParametersProbe()
        {
            args.Add(new ProbeArg("number", ProbeArgType.Number, true));
            args.Add(new ProbeArg("string", ProbeArgType.String, true));
            args.Add(new ProbeArg("date", ProbeArgType.Date, true));
            args.Add(new ProbeArg("datetime", ProbeArgType.DateTime, true));
        }

        public string Id => "Parameters";

        public string Description => "Probe to take parameters and display information back";

        public ISet<ProbeArg> Args => args;

        public async Task<dynamic> OnHandle(ProbeRunArgs args)
        {
            DateTime start = DateTime.UtcNow;

            var number = Math.Round(args.ParseDoubleNumberArg("number"), 0);
            var text = args.ParseStringArg("string");
            var date = args.ParseDateArg("date");
            var datetime = args.ParseDateTimeArg("datetime");
   
            DateTime end = DateTime.UtcNow;
            object result = new { Date = date, Number = number, Text = text, DateWithTime = datetime, TotalSecondsOnServer = (end - start).TotalSeconds };

            return await Task.FromResult(result);
        }
    }
    }
