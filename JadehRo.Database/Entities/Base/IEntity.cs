using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JadehRo.Database.Entities.Users;

namespace JadehRo.Database.Entities.Base;

public interface IEntity
{

};

public interface ISoftDelete
{
    bool IsActive { get; set; }
    DateTime? DeleteAt { get; set; }
}

public abstract class BaseEntity<TKey> : IEntity
{
    public virtual TKey Id { get; set; }
}

public abstract class BaseEntity : BaseEntity<long>
{

};

public abstract class BaseAuditableEntity<TKey, TUser, TUserKey> : BaseEntity<TKey>
{
    public virtual DateTime CreatedDateTime { get; set; }
    public virtual TUser CreatedUser { get; set; }
    public virtual TUserKey CreatedUserId { get; set; }
    public virtual DateTime ModifiedDateTime { get; set; }
    public virtual TUser ModifiedUser { get; set; }
    public virtual TUserKey ModifiedUserId { get; set; }

    [JsonIgnore]
    public virtual string Audit { get; set; }
}

public abstract class BaseAuditableEntity : BaseAuditableEntity<long, User, long>
{
    

};

public abstract class BaseAuditableEntity<TKey> : BaseAuditableEntity<TKey, User, long>
{
};

public abstract class BaseOptionEntity : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Key { get; set; }

    [Required]
    public string Value { get; set; }
}