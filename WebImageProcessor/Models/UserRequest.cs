using System;
using System.Collections.Generic;

namespace WebImageProcessor.Models;

public partial class UserRequest
{
    public int UserRequestId { get; set; }

    public string? ColorsInPhoto { get; set; }

    public string? ObjectsInPhoto { get; set; }

    public string Nickname { get; set; } = null!;

    public virtual AppUser NicknameNavigation { get; set; } = null!;
}
