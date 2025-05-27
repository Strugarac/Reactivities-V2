using System;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Persistance;

namespace Application.Profiles.Commands;

public class DeletePhoto
{
    public class Command : IRequest<Result<Unit>>;

    public class Handler(IPhotoService photoService, AppDbContext context) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            photoService.DeletePhoto();

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem deleting photo", 400);
        }
    }
}
