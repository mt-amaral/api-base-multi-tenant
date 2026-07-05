namespace Api.Client.Dto.Account;

public record LoginResponseDto(
    long Id,
    string Name,
    string Email,
    string UserType,
    Guid CompanyId,
    string CompanyName,
    string TenantId
);
