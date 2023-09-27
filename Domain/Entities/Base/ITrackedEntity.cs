namespace Domain.Entities.Base;

public interface ITrackedEntity
{
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
}