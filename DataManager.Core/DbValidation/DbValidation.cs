namespace DataManager.Core.DbValidation;

public static class DbValidation
{
    public static bool CheckIsValidDate(DateOnly dateString, string format = "dd.MM.yyyy.")
    {
        try
        {
            DateOnly.ParseExact(dateString.ToString(format), format, null);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public static bool CheckIsValidDate(DateTime dateTime, string format = "dd.MM.yyyy. HH:mm:ss")
    {
        try
        {
            DateTime.ParseExact(dateTime.ToString(format), format, null);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public static bool CheckAreAllUnique<T>(IEnumerable<T> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);
        return collection.Distinct().Count() == collection.Count();
    }
}