using DnsBman.Models.IdentityModels;
using System;
using System.Collections.Generic;

namespace DnsBman.Models;

public partial class UsersApiKey
{
    public int Id { get; set; }

    public string IdUser { get; set; } = null!;

    public string Value { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public bool Validity { get; set; } = true;

    public virtual ApplicationUser IdUserNavigation { get; set; } = null!;
}
