using System;
using System.Collections.Generic;

namespace Auth.Models;

public partial class AspNetUser
{
    public string Id { get; set; } = null!;

    /// <summary>
    /// Name used to show in change log. This is not indexed.
    /// </summary>
    public string UserName { get; set; } = null!;

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    /// <summary>
    /// Used for Login check
    /// </summary>
    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    /// <summary>
    /// This column is not part of Idenity. We have added this column. Used to link change logs from all tables
    /// </summary>
    public int MemberNo { get; set; }

    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; } = new List<AspNetUserClaim>();

    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; } = new List<AspNetUserLogin>();

    public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; } = new List<AspNetUserToken>();

    public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
}
