using System.ComponentModel.DataAnnotations;
using JadehRo.Database.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace JadehRo.Database.Entities.Users;

public class Role : IdentityRole<long>, IEntity
{
    [StringLength(100)]
    public string? Description { get; set; }
    public virtual required ICollection<UserRole> UserRoles { get; set; }
}