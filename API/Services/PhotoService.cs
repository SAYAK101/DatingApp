using API.Contracts;
using API.Helpers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var account = new Account(cloud: config.Value.CloudName, apiKey: config.Value.ApiKey, apiSecret: config.Value.ApiSecret);
        _cloudinary = new Cloudinary(account);      
    }
    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResults = new ImageUploadResult();

        if(file.Length > 0){
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams{
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "datingApp-net8"
            };

            uploadResults = await _cloudinary.UploadAsync(uploadParams);            
        }

        return uploadResults;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);

        return await _cloudinary.DestroyAsync(deleteParams);
    }
}
