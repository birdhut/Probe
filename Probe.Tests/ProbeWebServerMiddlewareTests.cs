namespace Probe.Tests
{
    using Xunit;
    using Moq;
    using Shouldly;
    using Probe.Service;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using Probe.Middleware;
    using System.IO;
    using Newtonsoft.Json;
    using System.Dynamic;
    using Microsoft.AspNetCore.Http;

    public class ProbeWebServerMiddlewareTests : MiddlewareTestBase
    {
        private class TestProbe : IProbe
        {
            private readonly string id;
            private readonly string description;
            private readonly HashSet<ProbeArg> probeArgs;

            public TestProbe(string id, string description, params ProbeArg[] args)
            {
                this.id = id;
                this.description = description;
                this.probeArgs = new HashSet<ProbeArg>();
                if (args != null)
                {
                    foreach (var a in args)
                    {
                        probeArgs.Add(a);
                    }
                }
            }

            public string Id => this.id;

            public string Description => this.description;

            public ISet<ProbeArg> Args => new HashSet<ProbeArg>();

            public Task<dynamic> OnHandle(ProbeRunArgs args)
            {
                dynamic result = new ExpandoObject();
                result.RunArgs = args;
                result.Id = this.id;
                result.Description = this.description;
                result.Args = this.probeArgs;

                return Task.FromResult(result as object);
            }
        }

        private readonly Mock<IProbeService> service;
        private readonly List<IProbe> probes;

        public ProbeWebServerMiddlewareTests()
        {
            probes = new List<IProbe>()
            {
                new TestProbe("Test1", "Test 1")
            };

            service = new Mock<IProbeService>();
            service
                .Setup(x => x.GetAllProbes())
                .Returns(probes.Select(x => new ProbeInfo { Id = x.Id, Description = x.Description, Args = x.Args }));
            service
                .Setup(x => x.GetProbe(It.IsAny<string>()))
                .Returns<string>(y => probes.FirstOrDefault(z => z.Id == y));
        }

        [Fact]
        public async Task Should_serve_get_request_at_specified_url()
        {
            var paths = new string[]
            {
                "/api/test",
                "/api",
                "/randomUrl",
                "/another-url"
            };

            for (int i = 0; i < paths.Length; i++)
            {
                var path = paths[i];
                var options = new ProbeOptions();
                options.UseWebClient = false;
                options.ProbeApiPath = path;
                options.ApiBase = new PathString(path);
                var contextMock = CreateHttpContext(path, path);
                var serverMiddleware = new ProbeWebServerMiddleware(next: (innerHttpContext) => Task.FromResult(0), probeService: service.Object, options: options);

                await serverMiddleware.InvokeAsync(contextMock.Object);

                using (StreamReader sr = new StreamReader(contextMock.Object.Response.Body))
                {
                    contextMock.Object.Response.Body.Position = 0;
                    var content = sr.ReadToEnd();
                    var target = JsonConvert.SerializeObject(probes.Select(x => new ProbeInfo { Id = x.Id, Description = x.Description, Args = x.Args }));
                    content.ShouldBe(target);
                }

                contextMock = null;
            }
        }

        [Fact]
        public async Task Should_serve_post_request_at_specified_url()
        {
            var paths = new string[]
            {
                "/api/test",
                "/api",
                "/randomUrl",
                "/another-url"
            };

            for (int i = 0; i < paths.Length; i++)
            {
                var path = paths[i];
                var options = new ProbeOptions();
                options.UseWebClient = false;
                options.ProbeApiPath = path;
                options.ApiBase = new PathString(path);
                
                var serverMiddleware = new ProbeWebServerMiddleware(next: (innerHttpContext) => Task.FromResult(0), probeService: service.Object, options: options);

                foreach (var probe in probes)
                {
                    var contextMock = CreateHttpContext(options.ProbeApiPath, $"{probe.Id}", "POST");

                    await serverMiddleware.InvokeAsync(contextMock.Object);

                    using (StreamReader sr = new StreamReader(contextMock.Object.Response.Body))
                    {
                        contextMock.Object.Response.Body.Position = 0;
                        var content = sr.ReadToEnd();
                        dynamic expected = await probe.OnHandle(null);
                        var obj = (string)JsonConvert.SerializeObject(expected);
                        content.ShouldBe(obj);
                    }

                    contextMock = null;
                }
            }
        }
    }
}
