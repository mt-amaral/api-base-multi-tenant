using System.ComponentModel.DataAnnotations;

namespace Api.Client.Dto.Account;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email e obrigatorio.")]
    [EmailAddress(ErrorMessage = "Email invalido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha e obrigatoria.")]
    [MinLength(6, ErrorMessage = "Senha deve ter no minimo 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
