namespace Probe.Service
{
    /// <summary>
    /// Defines the behaviour of the service used to deal with requests for the Probe Web Client.
    /// </summary>
    public interface IWebClientService
    {
        /// <summary>
        /// Gets a <see cref="byte[]"/> representing the <see cref="ImageFile"/> for the Web Client.
        /// </summary>
        /// <param name="file">The file which should be returned</param>
        /// <returns><see cref="byte[]"/></returns>
        byte[] this[ImageFile file] { get; }

        /// <summary>
        /// Gets the file content of the specified file for the Web Client.
        /// </summary>
        /// <param name="filename">The name of the file whose content should be returned</param>
        /// <returns><see cref="string"/></returns>
        string this[string filename] { get; }

        /// <summary>
        /// Gets the file content of the Index, or root, html file for the Web Client
        /// </summary>
        string IndexHtml { get; }

        /// <summary>
        /// Gets a value indicating whether the file at the given file path is an Image file.
        /// </summary>
        /// <param name="filePath">The path to the file to be checked</param>
        /// <returns><see cref="boolean"/>: True if the file is an image, false otherwise</returns>
        bool IsImageFile(string filePath);
    }
}