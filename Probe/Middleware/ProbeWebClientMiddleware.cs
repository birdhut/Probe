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

            if ((context.Request.PathBase.HasValue && context.Request.PathBase.Value == options.WebClientPath)
                || context.Request.Path.HasValue && context.Request.Path.Value == options.WebClientPath)
            {
                await context.Response.WriteAsync(service.IndexHtml);
                return;
            }

            if (context.Request.Path.HasValue)
            {
                var filePath = context.Request.Path.Value.Trim('/');
                // image handling
                if (service.IsImageFile(filePath))
                {
                    var imageFile = new ImageFile(filePath);
                    var image = service[imageFile];
                    if (image != null)
                    {
                        await context.Response.Body.WriteAsync(image, 0, image.Length);
                        return;
                    }
                }
                else
                {
                    await context.Response.WriteAsync(service[context.Request.Path.Value.Trim('/')]);
                    return;
                }
                
                
            }

            // Call the next delegate/middleware in the pipeline
            await next(context);
        }
    }
}
