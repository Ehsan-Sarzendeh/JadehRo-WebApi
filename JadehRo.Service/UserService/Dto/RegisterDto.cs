using System.ComponentModel.DataAnnotations;
using JadehRo.Common.Validators;
using JadehRo.Database.Entities.Users;

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