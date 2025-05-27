using System;

namespace Application.Profiles.DTOs;

public class UserImageDto
{
    public byte[]? Content { get; set; }

    public string ContentType { get; set; } = "image/jpg";
}
