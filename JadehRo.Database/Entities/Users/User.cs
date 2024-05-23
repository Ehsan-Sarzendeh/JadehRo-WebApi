using JadehRo.Database.Entities.Base;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JadehRo.Database.Entities.Users;

public class User : IdentityUser<long>, IEntity, ISoftDelete
{
    public UserType Type { get; set; }

    [Required, StringLength(100)]
    public required string FullName { get; set; }

    [StringLength(10)]
    public string? NationalCode { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public bool IsSuspended { get; set; }

    [StringLength(10)]
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }


    public bool IsActive { get; set; }
    public DateTime? DeleteAt { get; set; }

    public virtual ICollection<UserRole>? UserRoles { get; set; }
}

public enum UserType
{
    Admin = 1,
    Supporter = 2,
    Driver = 3,
    Passenger = 4
}