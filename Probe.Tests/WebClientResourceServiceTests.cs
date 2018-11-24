namespace Probe.Tests
{
    using Probe.Service;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Xunit;

    public class WebClientResourceServiceTests
    {
        private readonly WebClientResourcesService webClient;
        private readonly string mainScriptName = "probeweb.8095d0.js";

        public WebClientResourceServiceTests()
        {
            webClient = new WebClientResourcesService(new ProbeOptions());
            Type clientService = typeof(WebClientResourcesService);
            var assembly = Assembly.GetAssembly(clientService);
            var ns = clientService.Namespace;
            var resourcePath = $"{ns}.Client.";
            string[] resources = assembly.GetManifestResourceNames();
            List<string> resx = resources.Select(x => x.Replace(resourcePath, "")).ToList();
            mainScriptName = resx.First(x => x.EndsWith(".js"));
        }

        [Fact]
        public void Can_Serve_Web_Client_Entry_File()
        {
            var index = webClient.IndexHtml;

            index.ShouldNotBeNullOrWhiteSpace();
            index.ShouldContain("<html");
            index.ShouldContain($"<script type=\"text/javascript\" src=\"{mainScriptName}\"></script>");
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
