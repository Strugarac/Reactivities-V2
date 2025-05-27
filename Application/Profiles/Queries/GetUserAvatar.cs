using System;
using Application.Core;
using Application.Interfaces;
using Application.Profiles.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace Application.Profiles.Queries;

public class GetUserAvatar
{
    public class Query : IRequest<Result<UserImageDto>>
    {
        public required string Id { get; set; }
    }

    public class Handler(IPhotoService photoService) : IRequestHandler<Query, Result<UserImageDto>>
    {
        public async Task<Result<UserImageDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var actionResult = await photoService.GetPhoto(request.Id);

            if (actionResult == null) return Result<UserImageDto>.Failure("Fail getting photo", 400);

            return Result<UserImageDto>.Success(actionResult);
        }   
    }
}
