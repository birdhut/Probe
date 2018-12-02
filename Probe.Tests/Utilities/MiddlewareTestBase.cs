namespace Probe.Tests
{
    using Microsoft.AspNetCore.Http;
    using Moq;
    using System.IO;

    public abstract class MiddlewareTestBase
    {
        protected static Mock<HttpContext> CreateHttpContext(string pathBase = "/", string requestPath = null, string method = "GET")
        {
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.Scheme).Returns("http");
            requestMock.Setup(x => x.Host).Returns(new HostString("localhost"));
            requestMock.Setup(x => x.Path).Returns(new PathString($"/{requestPath}"));
            requestMock.Setup(x => x.PathBase).Returns(new PathString(pathBase));
            requestMock.Setup(x => x.Method).Returns(method);
            requestMock.Setup(x => x.Body).Returns(new MemoryStream());
            requestMock.Setup(x => x.QueryString).Returns(new QueryString("?param1=2"));

            var responseMock = new Mock<HttpResponse>();
            responseMock.Setup(x => x.Body).Returns(new MemoryStream());

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            contextMock.Setup(x => x.Response).Returns(responseMock.Object);

            return contextMock;
        }
    }
}
