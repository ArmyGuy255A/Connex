namespace Domain.Events;

public class ApplicationOkResult : ApplicationResult, IApplicationResult
{
    public ApplicationOkResult() : base(true, "SUCCESS")
    {
    }
}