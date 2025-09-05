using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

namespace AdvertisingPlatform.Application
{
    public class LocationAdService : ILocationAdService
    {
        private ConcurrentDictionary<string, HashSet<string>> _locationToPlatforms = new();

        public async Task LoadAdPlatformsFromFileAsync(IFormFile file)
        {
            _locationToPlatforms.Clear();

            using var reader = new StreamReader(file.OpenReadStream());
            string? line;
            
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                try
                {
                    var parts = line.Split(':', 2);
                    if (parts.Length != 2)
                        continue;

                    var platformName = parts[0].Trim();
                    var locationsString = parts[1].Trim();
                    
                    if (string.IsNullOrEmpty(platformName) || string.IsNullOrEmpty(locationsString))
                        continue;

                    var locations = locationsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(l => l.Trim())
                        .ToList();

                    foreach (var location in locations)
                    {
                        AddPlatformToLocationHierarchy(platformName, location);
                    }
                }
                catch
                {
                    continue;
                }
            }
            foreach (var item in _locationToPlatforms)
            {
                var currentLocation = item.Key;
                while (currentLocation.Contains('/'))
                {
                    var lastSlashIndex = currentLocation.LastIndexOf('/');
                    if (lastSlashIndex <= 0)
                        break;

                    currentLocation = currentLocation.Substring(0, lastSlashIndex);

                    if (_locationToPlatforms.TryGetValue(currentLocation, out var parentPlatforms))
                    {
                        foreach (var platform in parentPlatforms)
                        {
                            _locationToPlatforms[item.Key].Add(platform);
                        }
                    }
                }
            } 
        }

        private void AddPlatformToLocationHierarchy(string platformName, string location)
        {
            _locationToPlatforms.AddOrUpdate(
                location,
                new HashSet<string> { platformName },
                (key, existing) =>
                {
                    existing.Add(platformName);
                    return existing;
                });
        }

        public List<string> GetAdPlatformsForLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return new List<string>();

            var result = new HashSet<string>();

            if (_locationToPlatforms.TryGetValue(location, out var exactPlatforms))
            {
                foreach (var platform in exactPlatforms)
                {
                    result.Add(platform);
                }
            }

            return result.OrderBy(p => p).ToList();
        } 
    }
}

//foreach (var items in _locationToPlatforms)
//{
//    Console.WriteLine($"{items.Key}");
//    foreach (var s in items.Value)
//    {
//        Console.Write($" {s}");
//    }
//    Console.WriteLine(" ");
//}

