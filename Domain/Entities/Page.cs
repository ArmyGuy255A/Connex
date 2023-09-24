namespace Domain.Entities;

public class Page
{
    public Page(string title)
    {
        Title = title;
        Id = new Guid();
    }

    public Page(string title, Guid parentId) : this(title)
    {
        this.ParentId = parentId;
    }

    public string Title { get; set; }
    public string Author { get; set; }
    public string[] Editors { get; set; }
    public Guid? ParentId { get; set; }
    public Guid Id { get; set; }
    public string[]? Tags { get; set; }
    public string[]? Categories { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
}