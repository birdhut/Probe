namespace Probe.Service
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Defines an Image file in component parts
    /// </summary>
    public struct ImageFile
    {
        public const string GenericImageMimeType = "image";
        /// <summary>
        /// A list of supported image formats to mime types
        /// </summary>
        public static readonly Dictionary<string, string> MimeTypes = new Dictionary<string, string>
        {
            { "ico", $"{GenericImageMimeType}/x-icon" },
            { "gif", $"{GenericImageMimeType}/gif" },
            { "png", $"{GenericImageMimeType}/png" },
            { "jpeg", $"{GenericImageMimeType}/jpg" },
            { "jpg", $"{GenericImageMimeType}/jpg" }
        };

        /// <summary>
        /// Gets the name of the file (without extension)
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets the extension of the file (without preceding ".")
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// Gets the mime type of the file
        /// </summary>
        public string MimeType { get; }

        /// <summary>
        /// Initialises the object from the given relative file url path
        /// </summary>
        /// <param name="filePath">The relative url path to the file</param>
        public ImageFile(string filePath)
        {
            FileName = Path.GetFileNameWithoutExtension(filePath);
            Extension = Path.GetExtension(filePath).Trim(' ', '.');
            if (!MimeTypes.TryGetValue(Extension, out string mime))
            {
                mime = GenericImageMimeType;
            }
            MimeType = mime;
        }

        /// <summary>
        /// Outputs the filename and extenstion without the path
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return $"{FileName}.{Extension}";
        }
    }
}
