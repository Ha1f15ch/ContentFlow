namespace ContentFlow.Domain.Entities;

public class Category
{
    #region Class Category Properties

    public int Id { get; private set; }
    public string Name { get; private set; } = String.Empty;
    public string Slug { get; private set; } = String.Empty;
    public string? Description { get; private set; }

    #endregion
    
    #region Constructor

    public Category(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        
        Name = name;
        Description = description;
        Slug = GenerateSlug(name);
    }
    
    #endregion
    
    #region Public Methods

    public void Update(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        
        Name = name;
        Description = description;
        Slug = GenerateSlug(name);
    }
    
    #endregion
    
    #region Private Methods
    
    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant().Replace(" ", "-").Trim('-');
    }
    
    #endregion
}