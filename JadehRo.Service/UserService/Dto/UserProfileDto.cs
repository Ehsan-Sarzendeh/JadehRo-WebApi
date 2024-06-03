using JadehRo.Database.Entities.Users;
using JadehRo.Service.Infrastructure;

namespace JadehRo.Service.UserService.Dto;

public class UserProfileDto : BaseDto<UserProfileDto, User>
{
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsSuspended { get; set; }
}