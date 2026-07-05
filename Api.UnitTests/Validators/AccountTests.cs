namespace Api.UnitTests.Validators;



using System.ComponentModel.DataAnnotations;
using Api.Dto.Account;
using Xunit;

public class AccountTests
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
        // Arrange
        var dto = new LoginRequestDto
        {
            Email = "teste@email.com",
            Password = "123456",
            RememberMe = true
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void LoginRequestDto_EmailObrigatorio_DeveFalhar()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Email = null!,
            Password = "123456"
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Email é obrigatório.");
    }

    [Fact]
    public void LoginRequestDto_EmailInvalido_DeveFalhar()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Email = "email_invalido",
            Password = "123456"
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Email inválido.");
    }

    [Fact]
    public void LoginRequestDto_SenhaObrigatoria_DeveFalhar()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Email = "teste@email.com",
            Password = null!
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Senha é obrigatória.");
    }

    [Fact]
    public void LoginRequestDto_SenhaMinimo6Caracteres_DeveFalhar()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Email = "teste@email.com",
            Password = "123"
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Senha deve ter no mínimo 6 caracteres.");
    }
}