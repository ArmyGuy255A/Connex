namespace Domain.Entities.Base;

public class TrackedEntity : BaseEntity, ITrackedEntity
{
    protected TrackedEntity() : base()
    {
        
    }
    
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
}