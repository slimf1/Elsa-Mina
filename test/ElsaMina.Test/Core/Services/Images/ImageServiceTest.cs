using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Images;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ElsaMina.Test.Core.Services.Images;

public class ImageServiceTest
{
    private IHttpService _httpService;
    private ImageService _imageService;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _imageService = new ImageService(_httpService);
    }

    [Test]
    public async Task Test_GetRemoteImageDimensions_ShouldReturnCorrectDimensions_WhenImageLoadsSuccessfully()
    {
        // Arrange
        var image = new Image<Rgba32>(100, 200);
        var memoryStream = new MemoryStream();
        await image.SaveAsPngAsync(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        _httpService.GetStream(Arg.Any<string>()).Returns(memoryStream);

        // Act
        var (width, height) = await _imageService.GetRemoteImageDimensions("http://example.com/image.png");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(width, Is.EqualTo(100));
            Assert.That(height, Is.EqualTo(200));
        });
    }

    [Test]
    public async Task Test_GetRemoteImageDimensions_ShouldReturnMinusOneDimensions_WhenImageFailsToLoad()
    {
        // Arrange
        _httpService.GetStream(Arg.Any<string>()).Throws(new Exception("Image load failed"));

        // Act
        var (width, height) = await _imageService.GetRemoteImageDimensions("http://example.com/image.png");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(width, Is.EqualTo(-1));
            Assert.That(height, Is.EqualTo(-1));
        });
    }

    [Test]
    public void Test_ResizeWithSameAspectRatio_ShouldReturnOriginalDimensions_WhenImageFitsWithinMaxBounds()
    {
        // Arrange
        var width = 400;
        var height = 300;
        var maxWidth = 500;
        var maxHeight = 500;

        // Act
        var (newWidth, newHeight) = _imageService.ResizeWithSameAspectRatio(width, height, maxWidth, maxHeight);

        // Assert
        Assert.That(newWidth, Is.EqualTo(400));
        Assert.That(newHeight, Is.EqualTo(300));
    }

    [Test]
    public void Test_ResizeWithSameAspectRatio_ShouldScaleWidth_WhenWidthExceedsMaxBounds()
    {
        // Arrange
        var width = 1000;
        var height = 500;
        var maxWidth = 800;
        var maxHeight = 800;

        // Act
        var (newWidth, newHeight) = _imageService.ResizeWithSameAspectRatio(width, height, maxWidth, maxHeight);
        
        // Assert
        Assert.Multiple(() =>
        {

            Assert.That(newWidth, Is.EqualTo(800));
            Assert.That(newHeight, Is.EqualTo(400));
        });
    }

    [Test]
    public void Test_ResizeWithSameAspectRatio_ShouldScaleHeight_WhenHeightExceedsMaxBounds()
    {
        // Arrange
        var width = 500;
        var height = 1000;
        var maxWidth = 800;
        var maxHeight = 800;

        // Act
        var (newWidth, newHeight) = _imageService.ResizeWithSameAspectRatio(width, height, maxWidth, maxHeight);

        // Assert
        Assert.Multiple(() =>
        {

            Assert.That(newWidth, Is.EqualTo(400));
            Assert.That(newHeight, Is.EqualTo(800));
        });
    }

    [Test]
    public void Test_ResizeWithSameAspectRatio_ShouldScaleProportionally_WhenBothDimensionsExceedMaxBounds()
    {
        // Arrange
        var width = 1600;
        var height = 1200;
        var maxWidth = 800;
        var maxHeight = 600;

        // Act
        var (newWidth, newHeight) = _imageService.ResizeWithSameAspectRatio(width, height, maxWidth, maxHeight);

        // Assert
        Assert.Multiple(() =>
        {

            Assert.That(newWidth, Is.EqualTo(800));
            Assert.That(newHeight, Is.EqualTo(600));
        });
    }
}