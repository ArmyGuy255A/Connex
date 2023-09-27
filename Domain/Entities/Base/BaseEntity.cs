namespace Domain.Entities.Base;

public class BaseEntity : IBaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }

    public DateTime LastModifiedDate { get; set; }

    protected BaseEntity(Guid id) : this()
    {
        Id = id;
    }

    protected BaseEntity()
    {
        if (Id == Guid.Empty)
        {
            Id = new Guid();
        }

        CreatedDate = DateTime.Now;
        LastModifiedDate = CreatedDate;
    }
}