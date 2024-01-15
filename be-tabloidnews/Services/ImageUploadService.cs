using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using be_tabloidnews.DTOs;

namespace be_tabloidnews.Services
{
public class ImageUploadService : IImageUploadService
{
    private readonly Cloudinary _cloudinary;

    public ImageUploadService(IConfiguration configuration)
    {
      var cloudinarySection = configuration.GetSection("Cloudinary");

        
        var cloudinaryAccount = new Account(
            cloudinarySection["CloudName"],
            cloudinarySection["ApiKey"],
            cloudinarySection["ApiSecret"]
        );

        _cloudinary = new Cloudinary(cloudinaryAccount);
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "News"

                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
        }

        return uploadResult.SecureUrl.AbsoluteUri;
    }
    public async Task<string> DeleteImageAsync(ImageDTO imageUrl)
    {
        var url = imageUrl.imageUrl;
        var publicId = $"News/{Path.GetFileNameWithoutExtension(url)}";
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result.Result == "ok" ? "Delete success" : "Delete fail";
    }

}
}
