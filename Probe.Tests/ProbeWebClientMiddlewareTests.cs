namespace Probe.Tests
{
    using Moq;
    using Probe.Middleware;
    using Probe.Service;
    using Shouldly;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class ProbeWebClientMiddlewareTests : MiddlewareTestBase
    {
        private readonly Mock<IWebClientService> clientService;

        private const string IndexHtml = "<html><body><h1>Test Body</h1></body></html>";
        private const string SupportFile = "supportFile";
        private static readonly byte[] SupportImage = System.Text.Encoding.UTF8.GetBytes("supportImage");

        public ProbeWebClientMiddlewareTests()
        {
            clientService = new Mock<IWebClientService>();
            clientService
                .SetupGet(x => x.IndexHtml)
                .Returns(IndexHtml)
                .Verifiable();
            clientService
                .SetupGet(x => x[It.IsAny<string>()])
                .Returns(SupportFile)
                .Verifiable();
            clientService
                .SetupGet(x => x[It.IsAny<ImageFile>()])
                .Returns(SupportImage)
                .Verifiable();
            clientService
                .Setup(x => x.IsImageFile(It.Is<string>(y => ImageFile.MimeTypes.Keys.Contains(y.Substring(y.LastIndexOf(".") + 1)))))
                .Returns(true)
                .Verifiable();
        }

        [Fact]
        public async Task Should_request_index_html_at_requested_url()
        {
            var paths = new string[]
            {
                "/test",
                "/",
                "/randomUrl",
                "/another-url"
            };

            for(int i = 0; i < paths.Length; i++)
            {
                var path = paths[i];
                var options = new ProbeOptions();
                options.UseWebClient = true;
                options.WebClientPath = path;
                var contextMock = CreateHttpContext(path);
                var clientMiddleware = new ProbeWebClientMiddleware(next: (innerHttpContext) => Task.FromResult(0), options: options, service: clientService.Object);

                await clientMiddleware.InvokeAsync(contextMock.Object);

                clientService.VerifyGet(x => x.IndexHtml, Times.Exactly(i + 1), "Index Html was not called at least once");
                using (StreamReader sr = new StreamReader(contextMock.Object.Response.Body))
                {
                    contextMock.Object.Response.Body.Position = 0;
                    var content = sr.ReadToEnd();
                    content.ShouldBe(IndexHtml);
                }

                contextMock = null;
            }
        }

        [Fact]
        public async Task Should_request_support_files_for_non_image_file_urls()
        {
            var paths = new string[]
            {
                "/test/test.css",
                "/test/style.json",
                "/test/test.js",
                "/test/test.txt"
            };

            for (int i = 0; i < paths.Length; i++)
            {
                var path = paths[i];
                var options = new ProbeOptions();
                options.UseWebClient = true;
                options.WebClientPath = "/test";
                var contextMock = CreateHttpContext(null, path);
                var clientMiddleware = new ProbeWebClientMiddleware(next: (innerHttpContext) => Task.FromResult(0), options: options, service: clientService.Object);

                await clientMiddleware.InvokeAsync(contextMock.Object);

                var filePath = path.Substring(1);
                clientService.Verify(x => x[filePath], Times.Exactly(1), "Support Files was not called at least once");
                using (StreamReader sr = new StreamReader(contextMock.Object.Response.Body))
                {
                    contextMock.Object.Response.Body.Position = 0;
                    var content = sr.ReadToEnd();
                    content.ShouldBe(SupportFile);
                }

                contextMock = null;
            }
        }

        [Fact]
        public async Task Should_request_image_files_for_supported_image_type_urls()
        {
            var paths = ImageFile.MimeTypes.Keys.Select(x => $"/test/testImage.{x}").ToArray();

            for (int i = 0; i < paths.Length; i++)
            {
                var path = paths[i];
                var options = new ProbeOptions();
                options.UseWebClient = true;
                options.WebClientPath = "/test";
                var contextMock = CreateHttpContext(null, path);
                var clientMiddleware = new ProbeWebClientMiddleware(next: (innerHttpContext) => Task.FromResult(0), options: options, service: clientService.Object);

                await clientMiddleware.InvokeAsync(contextMock.Object);

                var imageFile = new ImageFile(path.Substring(1));

                clientService.Verify(x => x[imageFile], Times.Exactly(1), "Image File was not called at least once");
                contextMock.Object.Response.Body.Position = 0;
                byte[] content = new byte[contextMock.Object.Response.Body.Length];
                contextMock.Object.Response.Body.Read(content, 0, (int)contextMock.Object.Response.Body.Length);
                content.ShouldBe(SupportImage);

                contextMock = null;
            }
        }
    }
}
