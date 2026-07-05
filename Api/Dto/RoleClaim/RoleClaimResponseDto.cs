namespace Api.Dto.RoleClaim;


public record RoleClaimResponseDto(
    long RoleId,
    string RoleName,
    List<RoleClaimItemDto> Claims
);

public record RoleClaimItemDto(
    string ClaimValue,
    string ClaimType,
    string Description,
    bool Selected
);