namespace ContentFlow.Domain.Entities;

public class Tag
{
    #region Class Tag Properties
    
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    
    
    private readonly List<PostTag> _postTags = new();
    public IReadOnlyCollection<PostTag> PostTags => _postTags.AsReadOnly();
    
    #endregion
    
    #region Constructor
    
    public Tag(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        Name = name;
        Slug = GenerateSlug(name);
    }
    
    #endregion
    
    #region Public Methods
    
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name is required");

        Name = newName;
        Slug = GenerateSlug(newName);
    }
    
    #endregion
    
    #region Private Methods
    
    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant().Replace(" ", "-").Trim('-');
    }
    
    #endregion
}