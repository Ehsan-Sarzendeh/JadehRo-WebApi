using JadehRo.Service.Infrastructure.JwtServices;
using JadehRo.Service.UserService.Dto;
using JadehRo.Service.UserService.Paginates;

namespace JadehRo.Service.UserService;

public interface IUserService : IScopedDependency
{
    Task<bool> CheckUsername(string username);
    Task<AccessToken> RegisterClient(RegisterDto dto);
    Task<AccessToken> Login(LoginDto dto);
    Task<AccessToken> Token(LoginDto dto);
    Task<AccessToken> RefreshToken(TokenModel dto);

    UserProfileDto GetProfile(long userId);

    (IList<UserListDto>, int) GetUsers(UserPaginate paginate);
    GetUserDto GetUserById(long userId);
    Task AddUser(AddUserDto dto);
    Task EditUser(EditUserDto dto);
    void DeleteUser(long userId);

    Task<string> ForgotPassword(ForgotPasswordDto forgotPassword);
    Task ResetPassword(ResetPasswordDto dto);

    void ChangePhoneNumber(long userId, ChangePhoneNumberDto dto);

    IList<RoleDto> GetRoles();

    Task ChangeIsSuspended(int userId);

    void SendVerifyCode(string phoneNumber);
    void VerifyCode(int code, string phoneNumber, int validSec);
}