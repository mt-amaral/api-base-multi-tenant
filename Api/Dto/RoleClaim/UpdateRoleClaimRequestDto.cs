using System.ComponentModel.DataAnnotations;

namespace Api.Dto.RoleClaim;

public class UpdateRoleClaimRequestDto
{
    [Required(ErrorMessage = "A lista de claims é obrigatória.")]
    [MinLength(1, ErrorMessage = "A lista de claims deve conter pelo menos um item.")]
    public List<string> Claims { get; set; } = [];
}
