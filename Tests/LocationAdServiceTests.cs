using AdvertisingPlatform.Application;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Tests;

public class LocationAdServiceTests
{
    private readonly LocationAdService _service;

    public LocationAdServiceTests()
    {
        _service = new LocationAdService();
    }

    [Fact]
    public async Task LoadAdPlatformsFromFileAsync_ValidFile_LoadsPlatformsCorrectly()
    {
        // Arrange
        var content = "яндекс.ƒирект:/ru\n–евдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik\n√азета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl\n рута€ реклама:/ru/svrd";
        var file = CreateMockFile(content, "test.txt");
        var locations1 = new List<string> { "яндекс.ƒирект" };
        var locations2 = new List<string> { "яндекс.ƒирект", "–евдинский рабочий", " рута€ реклама"};
        var locations3 = new List<string> { "яндекс.ƒирект", "√азета уральских москвичей" };
        var locations4 = new List<string> { "яндекс.ƒирект", "√азета уральских москвичей" };

        // Act
        await _service.LoadAdPlatformsFromFileAsync(file);

        // Assert
        var result1 = _service.GetAdPlatformsForLocation("/ru");
        var result2 = _service.GetAdPlatformsForLocation("/ru/svrd/revda");
        var result3 = _service.GetAdPlatformsForLocation("/ru/msk");
        var result4 = _service.GetAdPlatformsForLocation("/ru/permobl");

        Assert.True(result1.All(e => locations1.Contains(e)));
        Assert.True(result2.All(e => locations2.Contains(e)));
        Assert.True(result3.All(e => locations3.Contains(e)));
        Assert.True(result4.All(e => locations4.Contains(e)));
    }

    [Fact]
    public async Task LoadAdPlatformsFromFileAsync_EmptyFile_DoesNotThrow()
    {
        // Arrange
        var file = CreateMockFile("", "empty.txt");

        // Act & Assert
        await _service.LoadAdPlatformsFromFileAsync(file);
        
        var platforms = _service.GetAdPlatformsForLocation("AnyLocation");
        Assert.Empty(platforms);
    }

    [Fact]
    public async Task LoadAdPlatformsFromFileAsync_InvalidLines_IgnoresInvalidLines()
    {
        // Arrange
        var content = "ValidPlatform: ValidLocation\nInvalidLine\nAnotherValid: AnotherLocation";
        var file = CreateMockFile(content, "test.txt");

        // Act
        await _service.LoadAdPlatformsFromFileAsync(file);

        // Assert
        var validPlatforms = _service.GetAdPlatformsForLocation("ValidLocation");
        var anotherPlatforms = _service.GetAdPlatformsForLocation("AnotherLocation");

        Assert.Contains("ValidPlatform", validPlatforms);
        Assert.Contains("AnotherValid", anotherPlatforms);
    }

    [Fact]
    public async Task GetAdPlatformsForLocation_NonExistentLocation_ReturnsEmptyList()
    {
        // Arange
        var file = CreateMockFile("яндекс.ƒирект:/ru", "empty.txt");
        await _service.LoadAdPlatformsFromFileAsync(file);

        // Act
        var platforms = _service.GetAdPlatformsForLocation("NonExistentLocation");

        // Assert
        Assert.Empty(platforms);
    }

    [Fact]
    public async Task GetAdPlatformsForLocation_EmptyLocation_ReturnsEmptyList()
    {
        // Arange
        var file = CreateMockFile("яндекс.ƒирект:/ru", "empty.txt");
        await _service.LoadAdPlatformsFromFileAsync(file);

        // Act
        var platforms = _service.GetAdPlatformsForLocation("");

        // Assert
        Assert.Empty(platforms);
    }

    private static IFormFile CreateMockFile(string content, string fileName)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        
        return new FormFile(stream, 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };
    }
}
