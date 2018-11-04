namespace Probe.Tests
{
    using Probe.Service;
    using Shouldly;
    using Xunit;

    public class WebClientResourceServiceTests
    {
        private readonly WebClientResourcesService webClient;
        private const string mainScriptName = "probeweb.20682a.js";

        public WebClientResourceServiceTests()
        {
            webClient = new WebClientResourcesService();
        }

        [Fact]
        public void Can_Serve_Web_Client_Entry_File()
        {
            var index = webClient.IndexHtml;

            index.ShouldNotBeNullOrWhiteSpace();
            index.ShouldContain("<html");
            index.ShouldContain($"<script type=\"text/javascript\" src=\"/{mainScriptName}\"></script>");
        }

        [Fact]
        public void Can_Serve_Supporting_Text_Files()
        {
            var script = webClient[mainScriptName];

            script.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void Can_Serve_Supporting_Image_Files()
        {
            const string faviconPath = "/favicon.ico";
            var faviconImageFile = new ImageFile(faviconPath);

            var favicon = webClient[faviconImageFile];

            favicon.ShouldNotBeNull();
            favicon.Length.ShouldBeGreaterThan(0);
        }
    }
}
