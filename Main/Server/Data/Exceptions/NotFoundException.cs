namespace Server.Data.Exceptions;

public class NotFoundException : Exception
{
    public string Title => "Ничего по данному запросу не найдено";
}