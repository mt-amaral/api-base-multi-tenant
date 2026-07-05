using System.ComponentModel.DataAnnotations;

namespace Api.Dto.Role;

public class FilterRoleRequestDto : PaginationRequestDto
{
    [StringLength(100, ErrorMessage = "SearchString deve ter no máximo 100 caracteres.")]
    public string? SearchString { get; set; }
}
