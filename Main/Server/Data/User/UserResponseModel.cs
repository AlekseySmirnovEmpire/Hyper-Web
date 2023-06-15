namespace Server.Data.User;

public class UserResponseModel
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public bool IsActive { get; set; }

    public string Name { get; set; }

    public string LastName { get; set; }

    public int Rating { get; set; }

    public string Role { get; set; }

    public DateTime CreateAt { get; set; }

    public UserResponseModel(UserModel userModel)
    {
        Id = userModel.Id;
        UserName = userModel.UserName;
        Email = userModel.Email;
        IsActive = userModel.IsActive;
        Name = userModel.Name;
        LastName = userModel.LastName;
        Rating = userModel.Rating;
        Role = userModel.Role.RoleName;
        CreateAt = userModel.CreateAt;
    }
}