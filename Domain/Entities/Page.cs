using Domain.Entities.Base;

namespace Domain.Entities;

public class Page : TrackedEntity
{
    public Page() : base()
    {
    }

    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Editors { get; set; }
    public Guid? ParentId { get; set; }

    public string? Tags { get; set; }
    public string? ContentDirectory { get; set; }
}