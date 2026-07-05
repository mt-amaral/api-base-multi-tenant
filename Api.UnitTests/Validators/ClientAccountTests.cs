namespace Api.UnitTests.Validators;

using System.ComponentModel.DataAnnotations;
using ClientLoginRequestDto = Api.Client.Dto.Account.LoginRequestDto;

public class ClientAccountTests
{
    private static bool TryValidate(object obj, out List<ValidationResult> results)
    {
        var context = new ValidationContext(obj, serviceProvider: null, items: null);
        results = new List<ValidationResult>();
        return Validator.TryValidateObject(obj, context, results, validateAllProperties: true);
    }

    [Fact]
    public void LoginRequestDto_ComDadosValidos_DeveSerValido()
    {
        var dto = new ClientLoginRequestDto
        {
            Email = "cliente@email.com",
            Password = "123456",
            RememberMe = true
        };

        var isValid = TryValidate(dto, out var results);

        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void LoginRequestDto_EmailObrigatorio_DeveFalhar()
    {
        var dto = new ClientLoginRequestDto
        {
            Email = null!,
            Password = "123456"
        };

        var isValid = TryValidate(dto, out var results);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Email e obrigatorio.");
    }

    [Fact]
    public void LoginRequestDto_EmailInvalido_DeveFalhar()
    {
        var dto = new ClientLoginRequestDto
        {
            Email = "email_invalido",
            Password = "123456"
        };

        var isValid = TryValidate(dto, out var results);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Email invalido.");
    }

    [Fact]
    public void LoginRequestDto_SenhaObrigatoria_DeveFalhar()
    {
        var dto = new ClientLoginRequestDto
        {
            Email = "cliente@email.com",
            Password = null!
        };

        var isValid = TryValidate(dto, out var results);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Senha e obrigatoria.");
    }

    [Fact]
    public void LoginRequestDto_SenhaMinimo6Caracteres_DeveFalhar()
    {
        var dto = new ClientLoginRequestDto
        {
            Email = "cliente@email.com",
            Password = "123"
        };

        var isValid = TryValidate(dto, out var results);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Senha deve ter no minimo 6 caracteres.");
    }
}
