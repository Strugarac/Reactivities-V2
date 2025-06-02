using System;
using Microsoft.AspNetCore.Identity;

namespace Domain;

public class User : IdentityUser
{
    public string? Displayname { get; set; }

    public string? Bio { get; set; }

    public string? ImageUrl { get; set; }

    //nav properties

    public ICollection<ActivityAttendee> Activities { get; set; } = [];

    public ICollection<UserFollowing> Followings { get; set; } = [];

    public ICollection<UserFollowing> Followers { get; set; } = [];
}
