namespace Server.Data.Exceptions;

public class WrongFiltersValueException : Exception
{
    private string FilterName { get; }
    
    public override string Message => $"Указано неверное значение фильтра {FilterName}";

    public WrongFiltersValueException(string filterName)
    {
        FilterName = filterName;
    }
}