using System.ComponentModel.DataAnnotations;
using JadehRo.Database.Entities.Users;
using JadehRo.Service.Infrastructure;

namespace JadehRo.Service.UserService.Dto;

public class AddUserDto : BaseDto<AddUserDto, User>
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; }

    [Required, RegularExpression("^[a-zA-Z0-9._]*$")]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    [Compare(nameof(Password), ErrorMessage = "تکرار رمز عبور با رمز عبور مغایرت دارد")]
    public string ConfirmPass { get; set; }

    public string PhoneNumber { get; set; }

    public List<long> Roles { get; set; }
}

public class EditUserDto : BaseDto<EditUserDto, User>
{
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public List<long> Roles { get; set; }
}

public class GetUserDto
{
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsSuspended { get; set; }
    public List<AllUserRoleDto> UserRoles { get; set; }
}

public class UserListDto : BaseDto<UserListDto, User>
{
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsSuspended { get; set; }
}