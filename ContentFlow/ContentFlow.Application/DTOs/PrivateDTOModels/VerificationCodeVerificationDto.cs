namespace ContentFlow.Application.DTOs.PrivateDTOModels;

/// <summary>
/// Только для внутреннего использования в ConfirmEmailCommandHandler.
/// Не отправляется клиенту.
/// </summary>
public record VerificationCodeVerificationDto(
    int Id,
    string? CodeHash,
    string? CodeSalt)
{
    private VerificationCodeVerificationDto() : this(default, default, default){}
}