namespace Probe.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Default implementation of <see cref="IWebClientService"/> using embedded resources as 
    /// the source for the required web client files.  Once loaded, the file content is cached in memory for faster access.
    /// </summary>
    public partial class WebClientResourcesService : IWebClientService
    {
        /// <summary>
        /// Holds the namespace of this type, used for accessing resources embedded at namespace.clientfolder.filename
        /// </summary>
        private readonly string ns;

        /// <summary>
        /// The name of the client folder where files are stored
        /// </summary>
        private const string ClientFolder = "Client";

        /// <summary>
        /// Reference to this assembly used to read embedded resources
        /// </summary>
        private readonly Assembly assembly;

        /// <summary>
        /// Stores the index.html or entry point of the cweb client
        /// </summary>
        private static string indexResource;

        /// <summary>
        /// Stores the content of any supporting text based files
        /// </summary>
        private static readonly Dictionary<string, string> supportFiles = new Dictionary<string, string>();

        /// <summary>
        /// Stores the content of any image based files
        /// </summary>
        private static readonly Dictionary<string, byte[]> supportImages = new Dictionary<string, byte[]>();

        /// <summary>
        /// Stores the current probe options
        /// </summary>
        private readonly ProbeOptions options;

        /// <summary>
        /// Initialises the object
        /// </summary>
        public WebClientResourcesService(ProbeOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            ns = GetType().Namespace;
            assembly = Assembly.GetExecutingAssembly();
            var files = assembly.GetManifestResourceNames();
        }

        /// <summary>
        /// Gets the embedded resource path in format required for reading from resources
        /// </summary>
        private string RootResourcePath => $"{ns}.{ClientFolder}.";

        /// <inheritdoc />
        public string IndexHtml
        {
            get
            {
                if (string.IsNullOrWhiteSpace(indexResource))
                {
                    indexResource = ReadEmbeddedTextResource("index.html");
                    if (ProbeOptions.DefaultApiPath != options.ProbeApiPath)
                    {
                        indexResource = indexResource.Replace(ProbeOptions.DefaultApiPath, options.ProbeApiPath);
                    }
                }

                return indexResource;
            }
        }

        /// <inheritdoc />
        public string this[string filename]
        {
            get
            {
                if (!supportFiles.TryGetValue(filename, out string fileContent))
                {
                    fileContent = ReadEmbeddedTextResource(filename);
                    supportFiles.Add(filename, fileContent);
                }

                if (fileContent == null)
                {
                    fileContent = ReadEmbeddedTextResource(filename);
                    supportFiles[filename] = fileContent;
                }

                return fileContent;
            }
        }

#pragma warning disable CA1819 // Properties should not return arrays
        /// <inheritdoc />
        public byte[] this[ImageFile file]
#pragma warning restore CA1819 // Properties should not return arrays
        {
            get
            {
                string fileName = file.ToString();
                if (!supportImages.TryGetValue(fileName, out byte[] imgContent))
                {
                    imgContent = ReadEmbeddedImageResource(fileName);
                    supportImages.Add(fileName, imgContent);
                }

                if (imgContent == null)
                {
                    imgContent = ReadEmbeddedImageResource(fileName);
                    supportImages[fileName] = imgContent;
                }

                return imgContent;
            }
        }

        /// <inheritdoc />
        public bool IsImageFile(string filePath)
        {
            string ext = Path.GetExtension(filePath).Trim(' ', '.');
            return ImageFile.MimeTypes.Keys.Contains(ext);
        }

        /// <summary>
        /// Reads a text based resource from the assembly manifest
        /// </summary>
        /// <param name="fileName">The filename with extension</param>
        /// <returns>File content as <see cref="string"/></returns>
        private string ReadEmbeddedTextResource(string fileName)
        {
            using (var stream = assembly.GetManifestResourceStream($"{RootResourcePath}{fileName}"))
            {
                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Reads a binary/image based resource from the assembly manifest
        /// </summary>
        /// <param name="fileName">The filename with extension</param>
        /// <returns>File content as <see cref="byte[]"/></returns>
        private byte[] ReadEmbeddedImageResource(string fileName)
        {
            using (var stream = assembly.GetManifestResourceStream($"{RootResourcePath}{fileName}"))
            {
                stream.Position = 0;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}
