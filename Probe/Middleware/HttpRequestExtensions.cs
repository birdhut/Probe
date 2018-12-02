namespace Probe.Middleware
{
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Provides Probe route matching extensions for HttpRequest
    /// </summary>
    public static class HttpRequestExtensions
    {
        private static readonly PathString RootPath = new PathString("/");

        /// <summary>
        /// Attempts to match a probe web client path based on the Request and the <see cref="ProbeOptions"/>.
        /// </summary>
        /// <remarks>
        /// If the path is matched, the relative web client path is populated.  
        /// If it is the root, it is null, otherwise it will be the relative path.
        /// </remarks>
        /// <param name="request">The <see cref="HttpRequest"/> to match</param>
        /// <param name="options">The current <see cref="ProbeOptions"/></param>
        /// <param name="probeRelativePath">Populated with relative path if successful</param>
        /// <returns>True if the path is matched, False otherwise</returns>
        public static bool TryMatchProbeWebClient(this HttpRequest request, ProbeOptions options, 
            out string probeRelativePath)
        {
            probeRelativePath = null;

            // Map is used to configure the pipeline, so we should check PathBase to see if the map is populated
            if (!request.PathBase.HasValue && !request.PathBase.Value.StartsWith(options.ClientBase))
            {
                return false;
            }
            
            if (request.Path.HasValue && request.Path != RootPath)
            {
                probeRelativePath = request.Path.Value.Trim('/');
            }
            
            return true;
        }

        /// <summary>
        /// Attempts to match a probe api path based on the Request and the <see cref="ProbeOptions"/>.
        /// </summary>
        /// <remarks>
        /// If the path is matched, the probe id populated.  
        /// If it is the root, it will be null, otherwise it will be the relative path.
        /// </remarks>
        /// <param name="request">The <see cref="HttpRequest"/> to match</param>
        /// <param name="options">The current <see cref="ProbeOptions"/></param>
        /// <param name="probeRelativePath">Populated with ProbeId if successful</param>
        /// <returns>True if the path is matched, False otherwise</returns>
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
