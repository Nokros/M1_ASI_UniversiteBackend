namespace UniversiteDomain.Exceptions.CsvExceptions;

[Serializable]
public class CsvNullException : Exception
{
    public CsvNullException() : base() { }
    public CsvNullException(string message) : base(message) { }
    public CsvNullException(string message, Exception inner) : base(message, inner) { }
}