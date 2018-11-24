namespace Probe.Middleware
{
    using Microsoft.AspNetCore.Http;

    public static class HttpRequestExtensions
    {
        private static readonly PathString RootPath = new PathString("/");

        public static bool TryMatchProbeClient(this HttpRequest request, ProbeOptions options, 
            out string probeRelativePath)
        {
            probeRelativePath = null;

            // Map is used to configure the pipeline, so we should check PathBase to see if the map is populated
            if (!request.PathBase.HasValue || !request.PathBase.Value.StartsWith(options.ClientBase))
            {
                return false;
            }
            
            if (request.Path.HasValue && request.Path != RootPath)
            {
                probeRelativePath = request.Path.Value.Trim('/');
            }
            
            return true;
        }

        public static bool TryMatchProbeApi(this HttpRequest request, ProbeOptions options, out string probeId)
        {
            probeId = null;

            // Map is used to configure the pipeline, and there is only one path,
            // so we should check PathBase to see if the path is exact
            if (request.PathBase != options.ApiBase)
            {
                return false;
            }

            if (request.Path.HasValue && request.Path != RootPath)
            {
                probeId = request.Path.Value.Trim('/');
            }

            return true;
        }
    }
}
