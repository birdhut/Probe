namespace Probe.Middleware
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Probe.Service;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides aspnetcore Pipeline RequestDelegate for the Probe Api
    /// </summary>
    public class ProbeApiMiddleware
    {
        /// <summary>
        /// The next deletgate in the aspnet core pipeline
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// The service used to service requests
        /// </summary>
        private readonly IProbeService probeService;

        /// <summary>
        /// Options used to determine whether to service the request
        /// </summary>
        private readonly ProbeOptions options;

        /// <summary>
        /// Initialises the Object
        /// </summary>
        /// <param name="next">The next delegate in the pipeline</param>
        /// <param name="probeService">An <see cref="IProbeService"/> used to service the request</param>
        /// <param name="options"><see cref="ProbeOptions"/> used to determine whether to service the request</param>
        public ProbeApiMiddleware(RequestDelegate next, IProbeService probeService, ProbeOptions options)
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

        /// <summary>
        /// Delegate method called by pipeline to determine whether this middleware can satisfy the request
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/></param>
        /// <returns><see cref="Task"/></returns>
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

        /// <summary>
        /// Handles a GET request to the endpoint
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/></param>
        /// <returns><see cref="Task"/></returns>
        private async Task HandleGet(HttpContext context)
        {
            var probes = probeService.GetAllProbes();
            var output = JsonConvert.SerializeObject(probes);
            await context.Response.WriteAsync(output);
        }

        /// <summary>
        /// Handles a POST request to the endpoint for the given probe Id
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/></param>
        /// <param name="probeId">The Id of the probe to run</param>
        /// <returns><see cref="Task"/></returns>
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
