namespace Probe.Middleware
{
    using Microsoft.AspNetCore.Http;
    using Probe.Service;
    using System.Threading.Tasks;

    /// <summary>
    /// Middleware for handling requests to the Probe Web Client
    /// </summary>
    public class ProbeWebClientMiddleware
    {
        /// <summary>
        /// The next delegate in the Middleware pipeline
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// 
        /// </summary>
        private readonly ProbeOptions options;
        private readonly IWebClientService service;

        public ProbeWebClientMiddleware(RequestDelegate next, ProbeOptions options, IWebClientService service)
        {
            this.next = next;
            this.options = options;
            this.service = service;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.TryMatchProbeClient(options, out string relativePath))
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
