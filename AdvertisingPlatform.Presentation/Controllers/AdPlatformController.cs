using AdvertisingPlatform.Application;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingPlatform.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdPlatformController(ILocationAdService locationAdService, ILogger<AdPlatformController> logger) : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> UploadAdPlatforms(IFormFile file)
        {
            Console.WriteLine("controlller");
            try
            {
                if (file == null)
                {
                    return BadRequest(new { error = "Файл не предоставлен" });
                }

                if (file.Length == 0)
                {
                    return BadRequest(new { error = "Файл пустой" });
                }

                var allowedExtensions = new[] { ".txt", ".csv" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!string.IsNullOrEmpty(fileExtension) && !allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { error = "Неподдерживаемый формат файла. Разрешены только .txt и .csv файлы" });
                }

                await locationAdService.LoadAdPlatformsFromFileAsync(file);

                logger.LogInformation("Рекламные площадки успешно загружены из файла {FileName}", file.FileName);

                return Ok(new { message = "Рекламные площадки успешно загружены" });
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Ошибка валидации при загрузке файла");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при загрузке рекламных площадок из файла");
                return StatusCode(500, new { error = "Внутренняя ошибка сервера при загрузке файла" });
            }
        }

      
        [HttpGet("search")]
        public IActionResult GetAdPlatformsForLocation([FromQuery] string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest(new { error = "Локация не может быть пустой" });
                }

                var platforms = locationAdService.GetAdPlatformsForLocation(location);

                logger.LogInformation("Найдено {Count} рекламных площадок для локации {Location}", platforms.Count, location);

                return Ok(new 
                { 
                    location = location,
                    platforms = platforms,
                    count = platforms.Count
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при поиске рекламных площадок для локации {Location}", location);
                return StatusCode(500, new { error = "Внутренняя ошибка сервера при поиске рекламных площадок" });
            }
        }
    }
}

