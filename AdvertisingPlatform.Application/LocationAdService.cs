using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

namespace AdvertisingPlatform.Application
{
    public class LocationAdService : ILocationAdService
    {
        private ConcurrentDictionary<string, HashSet<string>> locationToPlatforms = new();

        public async Task LoadAdPlatformsFromFileAsync(IFormFile file)
        {
            locationToPlatforms.Clear();

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
                if (locationToPlatforms.Count > 1) HandleNesting();
            }
             
        }

        private void AddPlatformToLocationHierarchy(string platformName, string location)
        {
            locationToPlatforms.AddOrUpdate(
                location,
                new HashSet<string> { platformName },
                (key, existing) =>
                {
                    existing.Add(platformName);
                    return existing;
                });
        }

        private void HandleNesting()
        {
            foreach (var item in locationToPlatforms)
            {
                var currentLocation = item.Key;
                while (currentLocation.Contains('/'))
                {
                    var lastSlashIndex = currentLocation.LastIndexOf('/');
                    if (lastSlashIndex <= 0)
                        break;

                    currentLocation = currentLocation.Substring(0, lastSlashIndex);

                    if (locationToPlatforms.TryGetValue(currentLocation, out var parentPlatforms))
                    {
                        foreach (var platform in parentPlatforms)
                        {
                            locationToPlatforms[item.Key].Add(platform);
                        }
                    }
                }
            }
        }

        public List<string> GetAdPlatformsForLocation(string location)
        {
            if (locationToPlatforms.TryGetValue(location, out var exactPlatforms))
            {
                return exactPlatforms.OrderBy(p => p).ToList(); 
            }
            else return new List<string>();
        } 
    }
}

