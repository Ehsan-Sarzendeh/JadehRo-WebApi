using System.ComponentModel.DataAnnotations;

namespace JadehRo.Service.UserService.Dto;

public class RegisterDto
{

    [Required]
    public int VerifyCode { get; set; }

    [Required, ValidIranianMobileNumber]
    public string PhoneNumber { get; set; }

    [Required, ValidIranianNationalCode]
    public string NationalCode { get; set; }

    public UserType UserType { get; set; }
}