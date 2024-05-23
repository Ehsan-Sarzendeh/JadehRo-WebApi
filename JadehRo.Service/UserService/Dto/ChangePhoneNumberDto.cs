namespace JadehRo.Service.UserService.Dto;

public class ChangePhoneNumberDto
{
    public int VerifyCode { get; set; }

    [ValidIranianMobileNumber]
    public string NewPhoneNumber { get; set; }
}