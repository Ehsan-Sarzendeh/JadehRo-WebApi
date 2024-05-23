using System.ComponentModel.DataAnnotations;
using JadehRo.Database.Entities.Base;

namespace JadehRo.Database.Entities.Users;

public  class VerifyCode : BaseEntity
{
    [StringLength(11)]
    public required string PhoneNumber { get; set; }
    public int Code { get; set; }
    public DateTime SendDate { get; set; }
}