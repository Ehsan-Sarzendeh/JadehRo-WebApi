using System.ComponentModel.DataAnnotations;

namespace JadehRo.Service.UserService.Dto;

public class ResetPasswordDto
{
    [Required]
    public string CurrentPassword { get; set; }

    [Required]
    public string NewPassword { get; set; }

    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "تکرار رمز عبور با رمز عبور مغایرت دارد")]
    public string ConfirmPass { get; set; }

    [Required]
    public long UserId { get; set; }
}