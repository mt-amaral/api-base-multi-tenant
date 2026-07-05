using Api.Dto;
using Api.Dto.User;

namespace Api.Services.Abstractions;

public interface IUserService
{
    Task<(PagedResponse<List<UserResponseDto>?>, short)> GetUsersAsync(FilterUsersRequestDto request, CancellationToken ct);
    Task<(Response<UserResponseDto?>, short)> CreateAsync(CreateRequestDto request, CancellationToken ct);
    Task<(Response<UserResponseDto?>, short)> UpdateAsnc(long userId, UpdateUserRequestDto userRequest, CancellationToken ct);
    Task<(Response<UserResponseDto?>, short)> UpdateLoggedAsnc(UpdateUserRequestDto userRequest, CancellationToken ct);
    Task<(Response<bool>, short)> DeleteAsync(long userId, CancellationToken ct);
}