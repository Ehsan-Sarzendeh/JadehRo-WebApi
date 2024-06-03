using AutoMapper;
using JadehRo.Common.Exceptions;
using JadehRo.Common.Utilities;
using JadehRo.Database.Entities.Users;
using JadehRo.Database.Repositories.RepositoryWrapper;
using JadehRo.Service.Infrastructure.JwtServices;
using JadehRo.Service.SmsService;
using JadehRo.Service.SmsService.Dtos;
using JadehRo.Service.UserService.Dto;
using JadehRo.Service.UserService.Paginates;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace JadehRo.Service.UserService;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly SiteSettings _siteSetting;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;
    private readonly IRepositoryWrapper _repository;
    private readonly ISmsService _smsService;

    public UserService(IMapper mapper, IJwtService jwtService, IOptionsSnapshot<SiteSettings> siteSetting, UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, IRepositoryWrapper repository, ISmsService smsService)
    {
        _mapper = mapper;
        _jwtService = jwtService;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _repository = repository;
        _smsService = smsService;
        _siteSetting = siteSetting.Value;
    }

    public async Task<bool> CheckUsername(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user != null;
    }

    public async Task<AccessToken> RegisterClient(RegisterDto dto)
    {
        if (dto.UserType != UserType.Passenger && dto.UserType != UserType.Driver)
            throw new BadRequestException("درخواست نامعتبر");

        VerifyCode(dto.VerifyCode, dto.PhoneNumber, 120);

        var user = await _userManager.FindByNameAsync(dto.PhoneNumber);

        if (user != null)
            throw new BadRequestException("شماره تماس در سامانه ثبت شده است");

        var newUser = new User
        {
            UserName = dto.PhoneNumber,
            PhoneNumber = dto.PhoneNumber,
            NationalCode = dto.NationalCode,
            PhoneNumberConfirmed = true,
            Type = dto.UserType,
            IsActive = true,
            LastLoginDate = DateTime.Now,
            RefreshToken = _jwtService.GenerateRefreshToken(),
            RefreshTokenExpiryTime = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.ExpirationMinutes + _siteSetting.JwtSettings.RefreshTokenExpiryMinutes)
    };
        var resultAddUser = await _userManager.CreateAsync(newUser);

        if (!resultAddUser.Succeeded)
            throw new LogicException("عملیات با خطا مواجه شد", null, resultAddUser.Errors);

        var jwtResult = await _jwtService.GenerateTokenAsync(newUser);
        jwtResult.refresh_token = newUser.RefreshToken;
        return jwtResult;
    }

    public async Task<AccessToken> Login(LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName);

        if (user == null)
            throw new BadRequestException("نام کاربری یافت نشد");

        if (!user.IsActive || user.IsSuspended)
            throw new BadRequestException("حساب کاربری شما غیرفعال شده است.");

        if (string.IsNullOrEmpty(dto.Password))
        {
            VerifyCode(dto.VerifyCode, dto.UserName, 200);
        }
        else
        {
            var checkPassResult = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (checkPassResult.IsLockedOut)
                throw new BadRequestException("بیش از حد مجاز تلاش کرده‌اید");

            if (!checkPassResult.Succeeded)
                throw new BadRequestException("اطلاعات کاربری نادرست است.");
        }

        if (user.Type is UserType.Passenger or UserType.Driver)
        {
            if (dto.UserType != UserType.Passenger && dto.UserType != UserType.Driver)
                throw new BadRequestException("درخواست نامعتبر");

            user.Type = dto.UserType.Value;
        }

        user.RefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.ExpirationMinutes + _siteSetting.JwtSettings.RefreshTokenExpiryMinutes);
        user.LastLoginDate = DateTime.Now;
        await _userManager.UpdateAsync(user);

        var jwtResult = await _jwtService.GenerateTokenAsync(user);
        jwtResult.refresh_token = user.RefreshToken;

        return jwtResult;
    }

    public async Task<AccessToken> Token(LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName);

        if (user == null)
            throw new BadRequestException("اطلاعات کاربری نادرست است");

        // var checkPassResult = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        // if (!checkPassResult.Succeeded)
        //     throw new BadRequestException("اطلاعات کاربری نادرست است.");

        var jwtResult = await _jwtService.GenerateTokenAsync(user);

        return jwtResult;
    }

    public async Task<AccessToken> RefreshToken(TokenModel dto)
    {
        if (dto.RefreshToken == null)
            throw new BadRequestException("لطفا مجدد وارد شوید");

        var token = dto.AccessToken;
        var refreshToken = dto.RefreshToken;

        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("لطفا مجدد وارد شوید");

        var user = _userManager.Users
            .FirstOrDefault(x => x.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            throw new UnauthorizedAccessException("لطفا مجدد وارد شوید");

        var newRefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.ExpirationMinutes +
                                                              _siteSetting.JwtSettings.RefreshTokenExpiryMinutes);

        await _userManager.UpdateAsync(user);

        var accessToken = await _jwtService.GenerateTokenAsync(user);
        accessToken.refresh_token = newRefreshToken;

        return accessToken;
    }

    public UserProfileDto GetProfile(long userId)
    {
        var user = _repository.User.TableNoTracking
            .Single(x => x.Id == userId);

        if (user == null)
            throw new BadRequestException("کاربر یافت شد");

        return UserProfileDto.FromEntity(_mapper, user);
    }

    public (IList<UserListDto>, int) GetUsers(UserPaginate paginate)
    {
        var model = _repository.User.TableNoTracking;
        (model, var count) = UserPaginate.GetPaginatedList(paginate, model);

        return (UserListDto.FromEntities(_mapper, model.ToList()), count);
    }

    public GetUserDto GetUserById(long userId)
    {
        var user = _repository.User.TableNoTracking
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .Single(x => x.Id == userId);

        var allRoles = GetRoles();
        var userRoles = allRoles
            .Select(role => new AllUserRoleDto()
            {
                RoleId = role.Id,
                RoleName = role.Name,
                RoleDescription = role.Description,
                IsSelected = user.UserRoles.Any(x => x.RoleId == role.Id)
            }).ToList();

        var dto = new GetUserDto
        {
            FullName = user.FullName,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            UserRoles = userRoles
        };

        return dto;
    }

    public async Task AddUser(AddUserDto dto)
    {
        var exitUser = await _userManager.FindByNameAsync(dto.UserName);
        if (exitUser != null)
            throw new BadRequestException("نام کاربری تکراری می باشد");

        var userModel = dto.ToEntity(_mapper);
        userModel.IsActive = true;
        userModel.PhoneNumberConfirmed = true;

        var resultAddUser = await _userManager.CreateAsync(userModel, dto.Password);
        if (!resultAddUser.Succeeded)
            throw new LogicException("عملیات با خطا مواجه شد", null, resultAddUser.Errors);

        if (dto.Roles != null && dto.Roles.Any())
        {
            foreach (var id in dto.Roles)
            {
                var roleSelected = await _roleManager.FindByIdAsync(id.ToString());
                var addRole = await _userManager.AddToRoleAsync(userModel, roleSelected.Name);
                if (!addRole.Succeeded)
                    throw new LogicException("عملیات با خطا مواجه شد", null, addRole.Errors);
            }
        }
    }

    public async Task EditUser(EditUserDto dto)
    {
        var currentUser = await _userManager.FindByIdAsync(dto.Id.ToString());

        var user = dto.ToEntity(_mapper, currentUser);

        await _userManager.UpdateAsync(user);

        await EditRole(user, dto.Roles);

        _repository.Save();
    }

    public void DeleteUser(long userId)
    {
        var user = _repository.User.Table
            .Single(x => x.Id == userId);

        _repository.User.Delete(user);
        _repository.Save();
    }

    public async Task<string> ForgotPassword(ForgotPasswordDto forgotPassword)
    {
        var user = await _userManager.FindByNameAsync(forgotPassword.Username);

        if (user is null)
            throw new NotFoundException("کاربری یافت نشد");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        _smsService.SendSmsInBackground(new SendingSmsRequest
        {
            To = new List<string>() { user.PhoneNumber },
            PatternId = "rsgvm39juf",
            PatternParams = new List<string> { "token" },
            PatternValues = new List<string>() { token }
        });

        return user.PhoneNumber;
    }

    public async Task ResetPassword(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString())
                   ?? throw new NotFoundException(" کاربر یافت نشد");

        if (dto.CurrentPassword == dto.NewPassword) throw new BadRequestException(" پسورد تکراری است ");

        var result = await _userManager.ChangePasswordAsync(user!, dto.CurrentPassword, dto.NewPassword);

        if (result.Succeeded) throw new LogicException("عملیات با خطا مواجه شد");
    }

    public void ChangePhoneNumber(long userId, ChangePhoneNumberDto dto)
    {
        VerifyCode(dto.VerifyCode, dto.NewPhoneNumber, 120);
        var user = _repository.User.Table.Single(x => x.Id == userId);
        user.PhoneNumber = dto.NewPhoneNumber;
        _repository.Save();
    }

    public IList<RoleDto> GetRoles()
    {
        var roles = _repository.Role.TableNoTracking.ToList();

        return RoleDto.FromEntities(_mapper, roles);
    }

    public async Task ChangeIsSuspended(int userId)
    {
        var user = _repository.User.Table.SingleOrDefault(x => x.Id == userId)
                   ?? throw new BadRequestException("کاربر یافت نشد");
        user.IsSuspended = !user.IsSuspended;
        await _userManager.UpdateSecurityStampAsync(user);
        await _userManager.UpdateAsync(user);
        _repository.Save();
    }

    private async Task EditRole(User user, List<long> newRoleIds)
    {
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        var newRoles = _repository.Role.TableNoTracking
            .Where(x => newRoleIds.Contains(x.Id))
            .Select(x => x.Name)
            .ToList();
        await _userManager.AddToRolesAsync(user, newRoles);
    }

    public void SendVerifyCode(string phoneNumber)
    {
        if (phoneNumber.Length != 11 || !phoneNumber.StartsWith("09"))
            throw new BadRequestException("شماره موبایل صحیح نمی باشد");

        var check = _repository.VerifyCode.TableNoTracking
            .Any(x =>
                x.PhoneNumber == phoneNumber &&
                EF.Functions.DateDiffSecond(x.SendDate, DateTime.Now) <= 120);

        if (check)
            throw new AppException("امکان ارسال دوباره پیامک وجود ندارد لطفا صبر کنید");

        var code = GenerateVerifyCode();

        _smsService.SendSmsInBackground(new SendingSmsRequest
        {
            To = new List<string>() { phoneNumber },
            PatternId = "rsgvm39juf",
            PatternParams = new List<string>() { "token" },
            PatternValues = new List<string>() { code.ToString() }
        });

        _repository.VerifyCode.Add(new VerifyCode()
        {
            Code = code,
            PhoneNumber = phoneNumber,
            SendDate = DateTime.Now
        }, true);
    }

    public void VerifyCode(int code, string phoneNumber, int validSec)
    {
        var check = _repository.VerifyCode.TableNoTracking
            .Any(x =>
                x.Code == code &&
                x.PhoneNumber == phoneNumber &&
                EF.Functions.DateDiffSecond(x.SendDate, DateTime.Now) <= validSec);

        if (!check)
            throw new AppException("کد وارد شده معتبر نمی باشد");
    }
    private int GenerateVerifyCode()
    {
        const int lengthOfCode = 6;
        var keys = "123456789".ToCharArray();
        return Convert.ToInt32(GenerateCode(keys, lengthOfCode));
    }
    private static string GenerateCode(char[] keys, int lengthOfCode)
    {
        var random = new Random();
        return Enumerable
            .Range(1, lengthOfCode) // for(i.. ) 
            .Select(k => keys[random.Next(0, keys.Length - 1)])  // generate a new random char 
            .Aggregate("", (e, c) => e + c); // join into a string
    }
}