using Server.Data;
using Server.Data.Dto;
using Server.Data.Email;

namespace Server.Services;

public class PasswordService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEmailService _emailService;

    public PasswordService(ApplicationDbContext dbContext, IEmailService emailService)
    {
        _dbContext = dbContext;
        _emailService = emailService;
    }

    public static string GenerateHashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public static bool Validate(string password, string hashPassword) => 
        BCrypt.Net.BCrypt.Verify(password, hashPassword);

    public void RefreshPasswordRequest(LoginModelDto? loginModel)
    {
        if (loginModel == null || string.IsNullOrEmpty(loginModel.Email))
        {
            throw new Exception("Введите пароль!");
        }

        var user = _dbContext.Users.FirstOrDefault(u => u.Email == loginModel.Email);
        if (user is not { IsActive: true })
        {
            throw new Exception("Пользователя с введёным email не существует или не активирован!");
        }

        _emailService.PutNewMessageInQueue(
            EmailSendingType.RefreshPassword,
            EmailPriority.High,
            "Восстановление пароля",
            user.Email,
            user);
    }

    public void RefreshPassword(Guid userId, string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new Exception("Пароль не заполнен!");
        }

        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            throw new Exception("Пользователь не найден!");
        }

        user.Password = GenerateHashPassword(password);

        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();
    }
}