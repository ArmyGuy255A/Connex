namespace Domain.Events;

public class ApplicationOkResult : ApplicationResult
{
    public ApplicationOkResult() : base(true, "SUCCESS")
    {
    }
}