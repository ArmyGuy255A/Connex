namespace Domain.Events;

public interface IApplicationResult
{
    bool Success { get; set; }
    string Message { get; set; }
}

public class ApplicationResult : IApplicationResult
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public ApplicationResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }
}