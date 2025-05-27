using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Profiles.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace Infrastructure.Photos;

public class PhotoService(IWebHostEnvironment environment, IUserAccessor userAccessor, AppDbContext context) : IPhotoService
{
    public async Task<string> AddPhoto(IFormFile imageFile, List<string> allowedExtensions)
    {
        if (imageFile == null)
        {
            throw new ArgumentNullException(nameof(imageFile));
        }

        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, "Uploads");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var ext = Path.GetExtension(imageFile.FileName);
        if (!allowedExtensions.Contains(ext))
        {
            throw new ArgumentException($"Only {string.Join(",", allowedExtensions)} are allowed");
        }

        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);
        var user = await userAccessor.GetUserAsync();
        if (user.ImageUrl != null)
        {
            if (File.Exists(Path.Combine(path, user.ImageUrl)))
            {
                File.Delete(Path.Combine(path, user.ImageUrl));
            }
        }
        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await imageFile.CopyToAsync(stream);
        return fileName;
    }

    public async void DeletePhoto()
    {
        var user = await userAccessor.GetUserAsync() ?? throw new Exception("No user is logger");

        if (user.ImageUrl == null) throw new Exception("User does not have profile photo");

        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, "Uploads");

        if (File.Exists(Path.Combine(path, user.ImageUrl)))
        {
            File.Delete(Path.Combine(path, user.ImageUrl));
        }

        user.ImageUrl = null;
    }

    public async Task<UserImageDto> GetPhoto(string id)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("User is not found");

        if (user.ImageUrl == null) throw new Exception("User does not have avatar");

        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, "Uploads");

        string filePath = Path.Combine(path, user.ImageUrl);

        var imageBytes = System.IO.File.ReadAllBytes(filePath);

        return new UserImageDto
            {
                Content = imageBytes,
                ContentType = "images/jpg"
            };
    }
}
