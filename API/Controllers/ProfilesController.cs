using System;
using System.Runtime.CompilerServices;
using Application.Core;
using Application.Interfaces;
using Application.Profiles.Commands;
using Application.Profiles.Queries;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProfilesController() : BaseApicontoller
{
    [HttpPost("add-photo")]
    public async Task<ActionResult> AddPhoto(IFormFile imageFile)
    {
        return HandleResult(await Mediator.Send(new AddPhoto.Command { ImageFile = imageFile }));
    }

    [HttpDelete]
    public async Task<ActionResult> DeletePhoto()
    {
        return HandleResult(await Mediator.Send(new DeletePhoto.Command { }));
    }

    [HttpGet("get-avatar/{id}")]
    public async Task<ActionResult> GetAvatar(string id)
    {
        var result = await Mediator.Send(new GetUserAvatar.Query { Id = id });

        if (!result.Issuccess || result.Value == null)
        {
            return NotFound();
        }

        var photo = result.Value;

        return File(photo.Content!, photo.ContentType);
    }
}
