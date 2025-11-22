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
    
    /// <summary>
    /// Генерирует URL-безопасный slug из имени.
    /// Может использоваться в бизнес-логике вне сущности.
    /// </summary>
    /// <param name="name">Исходное имя</param>
    /// <returns>Нормализованный slug</returns>
    public static string GenerateSlug(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "";
        
        return name.Trim()
            .ToLowerInvariant()
            .Replace("+", "-plus")
            .Replace(".", "-dot ")
            .Replace("&", "")
            .Replace("?", "")
            .Replace("!", "")
            .Replace("@", "")
            .Replace("#", "-sharp")
            .Replace("$", "")
            .Replace("%", "")
            .Replace("*", "")
            .Replace("/", "")
            .Replace("\\", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace("<", "")
            .Replace(">", "")
            .Replace("=", "")
            .Replace(",", "")
            .Replace(";", "")
            .Replace(":", "")
            .Replace("\"", "")
            .Replace("'", "")
            .Replace("`", "")
            .Replace("~", "")
            .Replace("^", "")
            .Replace("|", "")
            // После очистки — заменяем пробелы на дефисы
            .Replace(" ", "-")
            // Убираем множественные дефисы
            .Replace("--", "-")
            .Replace("--", "-") // Два раза — на случай тройных и более
            .Trim('-');
    }
    
    #endregion
    
    #region Private Methods
    
    
    
    #endregion
}