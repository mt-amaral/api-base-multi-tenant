using System.ComponentModel.DataAnnotations;

public class CreateRequestDto
{
    [Required(ErrorMessage = "UserName é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "UserName deve ter entre 3 e 100 caracteres.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 6, ErrorMessage = "NewPassword deve ter entre 6 e 100 caracteres.")]
    public string? NewPassword { get; set; }

    [Compare("NewPassword", ErrorMessage = "ConfirmPassword deve ser igual a NewPassword.")]
    public string? ConfirmPassword { get; set; }

    [Range(1, long.MaxValue, ErrorMessage = "RoleId deve ser maior que 0.")]
    public long RoleId { get; set; }
}
