using System.ComponentModel.DataAnnotations;

public class UpdateUserRequestDto
{

    [Required(ErrorMessage = "UserName é obrigatório.")]
    [MinLength(3, ErrorMessage = "UserName deve ter no mínimo 3 caracteres.")]
    [MaxLength(100, ErrorMessage = "UserName deve ter no máximo 100 caracteres.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; set; } = string.Empty;

    [MinLength(6, ErrorMessage = "NewPassword deve ter no mínimo 6 caracteres.")]
    [MaxLength(100, ErrorMessage = "NewPassword deve ter no máximo 100 caracteres.")]
    public string? NewPassword { get; set; }

    [Compare("NewPassword", ErrorMessage = "ConfirmPassword deve ser igual a NewPassword.")]
    public string? ConfirmPassword { get; set; }

    [Range(1, long.MaxValue, ErrorMessage = "RoleId inválido.")]
    public long RoleId { get; set; }
}
