using Microsoft.AspNetCore.Http;

namespace AdvertisingPlatform.Application;

public interface ILocationAdService
{
    Task LoadAdPlatformsFromFileAsync(IFormFile file);
    List<string> GetAdPlatformsForLocation(string location);
}
