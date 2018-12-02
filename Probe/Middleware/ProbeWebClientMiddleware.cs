namespace Probe.Middleware
{
    using Microsoft.AspNetCore.Http;
    using Probe.Service;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides aspnetcore Pipeline RequestDelegate for the Probe Web Client
    /// </summary>
    public class ProbeWebClientMiddleware
    {
        /// <summary>
        /// The next delegate in the Middleware pipeline
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// Options used to determine whether to service the request
        /// </summary>
        private readonly ProbeOptions options;

        /// <summary>
        /// The service used to service requests
        /// </summary>
        private readonly IWebClientService service;

        /// <summary>
        /// Initialises the object
        /// </summary>
        /// <param name="next">The next delegate in the pipeline</param>
        /// <param name="options"><see cref="ProbeOptions"/> used to determine whether to service the request</param>
        /// <param name="service"><see cref="IWebClientService"/> used to service the request</param>
        public ProbeWebClientMiddleware(RequestDelegate next, ProbeOptions options, IWebClientService service)
        {
            this.next = next;
            this.options = options;
            this.service = service;
        }

        /// <summary>
        /// Delegate method called by pipeline to determine whether this middleware can satisfy the request
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/></param>
        /// <returns><see cref="Task"/></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.TryMatchProbeWebClient(options, out string relativePath))
            {
                // Call the next delegate/middleware in the pipeline
                await next(context);
            }

            if (relativePath != null) // content link
            {
                // image handling
                if (service.IsImageFile(relativePath))
                {
                    var imageFile = new ImageFile(relativePath);
                    var image = service[imageFile];
                    if (image != null)
                    {
                        await context.Response.Body.WriteAsync(image, 0, image.Length);
                        return;
                    }
                }
                else
                {
                    await context.Response.WriteAsync(service[relativePath]);
                    return;
                }
            }
            else // root index
            {
                await context.Response.WriteAsync(service.IndexHtml);
            }           
        }
    }
}
