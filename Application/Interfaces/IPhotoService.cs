using System;
using Application.Core;
using Application.Profiles.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IPhotoService
{
    Task<string> AddPhoto(IFormFile imageFile, List<string> allowedExtensions);

    void DeletePhoto();

    Task<UserImageDto> GetPhoto(string id);
}
