using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Data.Dto;
using Server.Data.Email;
using Server.Data.Exceptions;
using Server.Data.User;

namespace Server.Services;

public class UserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly RoleManager _roleManager;
    private readonly ILogger<UserService> _logger;
    private readonly IEmailService _emailService;

    public UserService(
        ApplicationDbContext dbContext, 
        RoleManager roleManager, 
        ILogger<UserService> logger,
        IEmailService emailService)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
        _logger = logger;
        _emailService = emailService;
    }

    public UserModel? GetUserByEmail(string email) =>
        _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefault(u => u.Email == email);

    public UserModel? GetUserById(Guid userId) =>
        _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefault(u => u.Id == userId);

    public void SetRefreshToken(Guid userId, string token, DateTime expiredAt)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return;
        }

        user.RefreshToken = token;
        user.TokenExpireAt = expiredAt;

        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();
    }

    public void DropRefreshToken(Guid userId)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return;
        }

        user.RefreshToken = null;
        user.TokenExpireAt = null;

        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();
    }

    public UserResponseModel CreateUser(CreateUserDto? userModel)
    {
        if (userModel == null ||
            string.IsNullOrEmpty(userModel.UserName) ||
            string.IsNullOrEmpty(userModel.Email) ||
            string.IsNullOrEmpty(userModel.Password))
        {
            throw new WrongDataException();
        }

        var knownUser = _dbContext.Users
            .FirstOrDefault(u => u.Email == userModel.Email || u.UserName == userModel.UserName);

        if (knownUser != null)
        {
            throw new InvalidDataException("Пользователь с данным email или именем уже существует");
        }

        try
        {
            var user = new UserModel
            {
                Role = _roleManager.GetMemberRole(),
                Id = new Guid(),
                UserName = userModel.UserName,
                Email = userModel.Email,
                Name = userModel.Name,
                LastName = userModel.LastName,
                Password = PasswordService.GenerateHashPassword(userModel.Password),
                Rating = 1600,
                CreateAt = DateTime.Now,
                IsActive = false
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            _logger.LogInformation($"Создан пользователь с ID: '{user.Id}'");
            
            _emailService.PutNewMessageInQueue(
                EmailSendingType.EmailConfirm,
                EmailPriority.High,
                "Подтверждение регистрации",
                user.Email,
                user);
            
            return new UserResponseModel(user);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при создании пользователя: {ex.Message}");
            throw new Exception("Не удалось создать пользователя!");
        }
    }

    public void ConfirmUser(Guid userId)
    {
        var user = GetUserById(userId);
        if (user == null || user.IsActive)
        {
            throw new Exception("Неверные данные!");
        }

        user.IsActive = true;
        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();
    }
}