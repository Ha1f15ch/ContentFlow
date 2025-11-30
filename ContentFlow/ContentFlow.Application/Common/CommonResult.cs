namespace ContentFlow.Application.Common;

public class CommonResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }
    public int CodeResult { get; set; }
}