using System.ComponentModel.DataAnnotations.Schema;
using JadehRo.Database.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace JadehRo.Database.Entities.Users;

public class UserRole : IdentityUserRole<long>, IEntity
{
    [ForeignKey("UserId")]
    public virtual required User User { get; set; }

    [ForeignKey("RoleId")]
    public virtual required Role Role { get; set; }
}