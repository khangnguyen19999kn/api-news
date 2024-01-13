using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using be_tabloidnews.Services;
using be_tabloidnews.DTOs;
namespace be_tabloidnews.Controllers
{
[Route("api/image")]
[ApiController]
public class ImageUploadController : ControllerBase
{
    private readonly IImageUploadService _imageUploadService;

    public ImageUploadController(IImageUploadService imageUploadService)
    {
        _imageUploadService = imageUploadService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        try
        {
        
            var imageUrl = await _imageUploadService.UploadImageAsync(file);
            return Ok(new { ImageUrl = imageUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteImage([FromBody]ImageDTO imageUrl)
    {
        try
        {
            var result = await _imageUploadService.DeleteImageAsync(imageUrl);
            return Ok(new { Result = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

}
}