namespace Server.Data.Exceptions;

public class WrongDataException : Exception
{
    public override string Message => "Переданные данные не соответствуют норме.";
}