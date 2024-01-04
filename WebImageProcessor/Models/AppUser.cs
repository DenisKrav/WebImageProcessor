using System;
using System.Collections.Generic;

namespace WebImageProcessor.Models;

public partial class AppUser
{
    public string Nickname { get; set; } = null!;

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual UserRole Role { get; set; } = null!;

    public virtual ICollection<UserRequest> UserRequests { get; set; } = new List<UserRequest>();
}
