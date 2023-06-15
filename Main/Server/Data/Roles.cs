namespace Server.Data;

public static class Roles
{
    public static string Admin => nameof(Admin);

    public static string Moderator => nameof(Moderator);

    public static string Author => nameof(Author);

    public static string Subscriber => nameof(Subscriber);

    public static string Member => nameof(Member);

    public static string DisplayName(string role)
    {
        if (string.IsNullOrEmpty(role))
        {
            throw new ArgumentNullException(nameof(role));
        }

        return role switch
        {
            nameof(Admin) => "Админ",
            nameof(Moderator) => "Модеротор",
            nameof(Author) => "Автор",
            nameof(Subscriber) => "Подписчик",
            nameof(Member) => "Пользователь",
            _ => throw new NotImplementedException("Неизвестная роль!")
        };
    }
}