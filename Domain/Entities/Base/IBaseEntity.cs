namespace Domain.Entities.Base;

public interface IBaseEntity
{
    public Guid Id { get; internal set; }
    public DateTime CreatedDate { get; set; }
}