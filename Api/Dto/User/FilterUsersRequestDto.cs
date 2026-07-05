using System.ComponentModel.DataAnnotations;

namespace Api.Dto.User;

public class FilterUsersRequestDto : PaginationRequestDto
{
    [StringLength(100, ErrorMessage = "SearchString deve ter no máximo 100 caracteres.")]
    public string? SearchString { get; set; }

    [Range(1, long.MaxValue, ErrorMessage = "RoleId inválido.")]
    public long? RoleId { get; set; }
}
