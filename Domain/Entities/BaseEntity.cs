namespace Infrastructure.Persistence;

public interface IBaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
}