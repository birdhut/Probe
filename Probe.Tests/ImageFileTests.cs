namespace Probe.Tests
{
    using Probe.Service;
    using Shouldly;
    using Xunit;

    public class ImageFileTests
    {
        private const string fileName = "file";
        private const string extension = "png";

        [Fact]
        public void Extracts_filename_and_extension_from_path()
        {
            string path = $"/some/path/to/a/{fileName}.{extension}";

            var imageFile = new ImageFile(path);

            imageFile.FileName.ShouldBe(fileName);
            imageFile.Extension.ShouldBe(extension);
        }

        [Fact]
        public void Infers_mimetype_from_path()
        {
            string path = $"/some/path/to/a/{fileName}.{extension}";

            var imageFile = new ImageFile(path);

            imageFile.MimeType.ShouldBe(ImageFile.MimeTypes[extension]);
        }

        [Fact]
        public void Infers_generic_mimetype_from_unsupported_filetype()
        {
            var unsupportedFileExtension = "bmp";
            string path = $"/some/path/to/a/{fileName}.{unsupportedFileExtension}";

            var imageFile = new ImageFile(path);

            imageFile.MimeType.ShouldBe(ImageFile.GenericImageMimeType);
        }

        [Fact]
        public void ToString_Outputs_filename_and_extension()
        {
            string path = $"/some/path/to/a/{fileName}.{extension}";

            var imageFile = new ImageFile(path);

            imageFile.ToString().ShouldBe($"{fileName}.{extension}");
        }
    }
}
