namespace Probe.Middleware
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Probe.Service;
    using System.IO;
    using System.Threading.Tasks;

    public class ProbeWebServerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IProbeService probeService;
        private readonly ProbeOptions options;

        public ProbeWebServerMiddleware(RequestDelegate next, IProbeService probeService, ProbeOptions options)
        {
            this.next = next;
            this.probeService = probeService;
            this.options = options;
            JsonConvert.DefaultSettings = () =>
            {
                return new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.TryMatchProbeApi(options, out string probeId))
            {
                // Call the next delegate/middleware in the pipeline
                await next(context);
            }

            
            var method = context.Request.Method.ToUpper();
            if (method == "GET")
            {
                await HandleGet(context);
                return;
            }
            else if (method == "POST")
            {
                await HandlePost(context, probeId);
                return;
            }
  
        }

        private async Task HandleGet(HttpContext context)
        {
            var probes = probeService.GetAllProbes();
            var output = JsonConvert.SerializeObject(probes);
            await context.Response.WriteAsync(output);
        }

        private async Task HandlePost(HttpContext context, string probeId)
        {
            var probe = probeService.GetProbe(probeId);

            // Get args
            string content = null;
            using (var reader = new StreamReader(context.Request.Body))
            {
                content = await reader.ReadToEndAsync();
            }

            ProbeRunArgs args = null;
            if (!string.IsNullOrWhiteSpace(content))
            {
                args = JsonConvert.DeserializeObject<ProbeRunArgs>(content);
            }

            // Call handler
            dynamic result = await probe.OnHandle(args);
            string output = JsonConvert.SerializeObject(result);
            await context.Response.WriteAsync(output);
        }
    }
}
