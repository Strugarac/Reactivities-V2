using System;
using Application.Configuration;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Persistance;

namespace Application.Profiles.Commands;

public class AddPhoto
{
    public class Command : IRequest<Result<string>>
    {
        public required IFormFile ImageFile { get; set; }
    }


    public class Handler(IPhotoService photoService, IUserAccessor userAccessor, AppDbContext context, IOptions<ImageSettings> options) : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            List<string> allowedExtensions = options.Value.AllowedExt;

            if (request.ImageFile.Length > 1 * 1024 * 1024)
            {
                return Result<string>.Failure("File size should not exceed 1 MB", 400);
            }

            var actionResult = await photoService.AddPhoto(request.ImageFile, allowedExtensions);

            if (actionResult == null) return Result<string>.Failure("Failed to upload photo", 400);

            var user = await userAccessor.GetUserAsync();

            user.ImageUrl = actionResult;

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            return result ? Result<string>.Success(actionResult) : Result<string>.Failure("Problem uploading photo", 400);
        }
    }
}
