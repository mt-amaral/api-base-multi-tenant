namespace Api.Dto.Account;

public record LoginResponseDto(
    long Id,
    string Name,
    string Email,
    long RoleId,
    List<string>? Claims
);