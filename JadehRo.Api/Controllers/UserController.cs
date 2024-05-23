using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadehRo.Api.Controllers;

[Authorize]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("CheckUsername"), AllowAnonymous]
    public async Task<bool> CheckUsername(string username)
    {
        var res = await _userService.CheckUsername(username);
        return res;
    }

    [HttpPost("RegisterClient"), AllowAnonymous]
    public async Task<ApiResult<AccessToken>> RegisterClient(RegisterDto dto)
    {
        var res = await _userService.RegisterClient(dto);
        return res;
    }

    [HttpPost("Login"), AllowAnonymous]
    public async Task<ApiResult<AccessToken>> Login(LoginDto dto)
    {
        if (!ModelState.IsValid)
            throw new BadRequestException("پارامتر های ارسالی معتبر نیستند");

        var response = await _userService.Login(dto);
        return response;
    }

    [HttpPost("Token"), AllowAnonymous]
    public async Task<JsonResult> Token([FromForm] LoginDto dto)
    {
        var response = await _userService.Token(dto);
        return new JsonResult(response);
    }

    [HttpPost("RefreshToken")]
    public async Task<ApiResult<AccessToken>> RefreshToken(TokenModel dto)
    {
        var res = await _userService.RefreshToken(dto);
        return res;
    }

    [HttpGet("Profile")]
    public ApiResult<UserProfileDto> Profile()
    {
        var res = _userService.GetProfile(UserId!.Value);
        return res;
    }

    [HttpGet]
    public ApiResult<IList<UserListDto>> GetUsers([FromQuery] UserPaginate paginate)
    {
        var (userDto, count) = _userService.GetUsers(paginate);
        return new ApiResult<IList<UserListDto>>()
        {
            TotalRecord = count,
            Data = userDto
        };
    }

    [HttpGet("{userId:long}")]
    public ApiResult<GetUserDto> GetUserById([FromRoute] long userId)
    {
        var response = _userService.GetUserById(userId);
        return Ok(response);
    }

    [HttpPost("Add")]
    public async Task<ApiResult> AddUser(AddUserDto dto)
    {
        await _userService.AddUser(dto);
        return Ok();
    }

    [HttpPut("Edit")]
    public async Task<ApiResult> EditUser(EditUserDto dto)
    {
        await _userService.EditUser(dto);
        return Ok();
    }

    [HttpDelete("Delete/{userId:long}")]
    public ApiResult DeleteUser(long userId)
    {
        _userService.DeleteUser(userId);
        return Ok();
    }

    [HttpPost("ForgotPassword"), AllowAnonymous]
    public async Task<ApiResult<string>> ForgotPassword(ForgotPasswordDto forgotPassword)
    {
        var response = await _userService.ForgotPassword(forgotPassword);
        return Ok(response);
    }

    [HttpPut("ResetPassword")]
    public async Task<ApiResult> ResetPassword(ResetPasswordDto dto)
    {
        await _userService.ResetPassword(dto);
        return Ok();
    }

    [HttpPut("ChangePhoneNumber")]
    public ApiResult ChangePhoneNumber(ChangePhoneNumberDto dto)
    {
        _userService.ChangePhoneNumber(UserId!.Value, dto);
        return Ok();
    }

    [HttpGet("Roles")]
    public ApiResult<IList<RoleDto>> GetRoles()
    {
        var res = _userService.GetRoles();
        return Ok(res);
    }

    [HttpPut("ChangeIsSuspended/{userId:int}")]
    public async Task<ApiResult> ChangeUserIsSuspended([FromRoute] int userId)
    {
        await _userService.ChangeIsSuspended(userId);
        return Ok();
    }

    [HttpPost("SendVerifyCode"), AllowAnonymous]
    public ApiResult SendVerifyCode(string phoneNumber)
    {
        _userService.SendVerifyCode(phoneNumber);
        return Ok();
    }

    [HttpGet("VerifyCode"), AllowAnonymous]
    public ApiResult VerifyCode([FromQuery] string phoneNumber, [FromQuery] int code)
    {
        _userService.VerifyCode(code, phoneNumber, 120);
        return Ok();
    }
}