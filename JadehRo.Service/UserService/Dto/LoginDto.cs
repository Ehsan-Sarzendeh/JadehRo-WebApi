using System.ComponentModel.DataAnnotations;

namespace JadehRo.Service.UserService.Dto;

public class LoginDto
{
    [Required]
    public string UserName { get; set; }

    public string Password { get; set; }

    public int VerifyCode { get; set; }

    public UserType? UserType { get; set; }
}