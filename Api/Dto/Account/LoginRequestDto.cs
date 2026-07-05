using System.ComponentModel.DataAnnotations;

namespace Api.Dto.Account;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
