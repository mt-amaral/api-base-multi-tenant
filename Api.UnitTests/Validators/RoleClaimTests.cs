using System.ComponentModel.DataAnnotations;
using Api.Dto.RoleClaim;
using Xunit;

namespace Api.UnitTests.Validators;

public class RoleClaimTests
{
    private static bool TryValidate(object obj, out List<ValidationResult> results)
    {
        var context = new ValidationContext(obj, serviceProvider: null, items: null);
        results = new List<ValidationResult>();
        return Validator.TryValidateObject(obj, context, results, validateAllProperties: true);
    }

    #region UpdateRoleClaimRequestDto Tests

    [Fact]
    public void UpdateRoleClaimRequestDto_ComDadosValidos_DeveSerValido()
    {
        // Arrange
        var dto = new UpdateRoleClaimRequestDto
        {
            Claims = new List<string> { "permission1", "permission2" }
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void UpdateRoleClaimRequestDto_ClaimsNulas_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateRoleClaimRequestDto
        {
            Claims = null!
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "A lista de claims é obrigatória.");
    }

    [Fact]
    public void UpdateRoleClaimRequestDto_ClaimsVazias_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateRoleClaimRequestDto
        {
            Claims = new List<string>()
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "A lista de claims deve conter pelo menos um item.");
    }

    [Fact]
    public void UpdateRoleClaimRequestDto_ClaimsInicializadasComListaVazia_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateRoleClaimRequestDto();

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "A lista de claims deve conter pelo menos um item.");
    }

    #endregion
}
