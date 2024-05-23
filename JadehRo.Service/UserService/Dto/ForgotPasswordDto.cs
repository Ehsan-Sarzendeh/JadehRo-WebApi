
using System.ComponentModel.DataAnnotations;

namespace JadehRo.Service.UserService.Dto;

public class ForgotPasswordDto
{
    [Required]
    public string Username { get; set; }    
}