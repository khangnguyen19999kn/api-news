using be_tabloidnews.DTOs;
namespace be_tabloidnews.Services
{
public interface IImageUploadService
{
    Task<string> UploadImageAsync(IFormFile file);
    Task<string> DeleteImageAsync(ImageDTO imageUrl);
}

}
