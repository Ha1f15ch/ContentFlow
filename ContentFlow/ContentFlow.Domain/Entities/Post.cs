using ContentFlow.Domain.Enums;

namespace ContentFlow.Domain.Entities;

public class Post
{
    #region Class Post Properties
    
    public int Id { get; private set; }
    public string Title { get; private set; } = String.Empty;
    public string Slug { get; private set; } = String.Empty;
    public string Excerpt { get; private set; } = String.Empty;
    public string Content { get; private set; } = String.Empty;
    public PostStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime PublishedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public bool IsDeleted { get; private set; } = false;
    
    public int AuthorId { get; private set; }
    public int? CategoryId { get; private set; }
    
    private readonly List<PostTag>  _postTags = new();
    public IReadOnlyCollection<PostTag> PostTags => _postTags.AsReadOnly();

    #endregion
    
    #region Constructor

    public Post(
        string title,
        string content,
        int authorId,
        string? excerpt = null,
        int? categoryId = null,
        bool isDeleted = false)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null or empty. Its required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content cannot be null or empty. Its required.", nameof(content));
        }
        
        Title = title;
        Content = content;
        Excerpt = excerpt ?? Truncate(content, 200);
        AuthorId = authorId;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
        Status = PostStatus.Draft;
        IsDeleted = isDeleted;
        
        SetSlug(title);
    }

    #endregion

    #region Public Methods
    
    public void Publish()
    {
        if(Status != PostStatus.Draft && Status != PostStatus.Rejected)
            throw new InvalidOperationException("Only draft or rejected posts can be published");

        Status = PostStatus.Published;
        PublishedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        if(Status != PostStatus.PendingModeration)
            throw new InvalidOperationException("Only pending posts can be rejected");

        Status = PostStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContent(string newContent)
    {
        if(string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Content cannot be empty.", nameof(newContent));
        
        Content = newContent;
        Excerpt = Truncate(newContent, 200);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCategory(int? categoryId)
    {
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveCategory()
    {
        if (CategoryId <= 0)
            return;
        
        CategoryId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddTag(Tag tag)
    {
        if (tag == null) throw new ArgumentNullException(nameof(tag));
        if (!_postTags.Any(pt => pt.TagId == tag.Id))
        {
            _postTags.Add(new PostTag { PostId = Id, TagId = tag.Id, Tag = tag });
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveTag(Tag tag)
    {
        var postTag = _postTags.FirstOrDefault(pt => pt.TagId == tag.Id);
        if (postTag != null)
        {
            _postTags.Remove(postTag);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void SetSlug(string title)
    {
        Slug = GenerateSlug(title);
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
        DeletedAt = DateTime.UtcNow;
    }
    
    #endregion

    #region Private Methods

    private static string GenerateSlug(string title)
    {
        var slug = title
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("--", "-")
            .Replace("?", "")
            .Replace("!", "")
            .Trim('-');
        
        return string.IsNullOrEmpty(slug) ? "post" : slug;
    }

    private static string Truncate(string value, int maxLength)
    {
        if(string.IsNullOrEmpty(value)) return "";
        return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
    }

    #endregion
}