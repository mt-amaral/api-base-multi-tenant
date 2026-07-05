using System.ComponentModel.DataAnnotations;
using Api.Dto.Role;


namespace Api.UnitTests.Validators;

public class RoleTests
{
    private static bool TryValidate(object obj, out List<ValidationResult> results)
    {
        var context = new ValidationContext(obj, serviceProvider: null, items: null);
        results = new List<ValidationResult>();
        return Validator.TryValidateObject(obj, context, results, validateAllProperties: true);
    }

    #region CreateRoleRequestDto Tests

    [Fact]
    public void CreateRoleRequestDto_ComDadosValidos_DeveSerValido()
    {
        // Arrange
        var dto = new CreateRoleRequestDto
        {
            Name = "Administrador",
            Description = "Perfil com acesso total ao sistema."
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void CreateRoleRequestDto_NomeObrigatorio_DeveFalhar()
    {
        // Arrange
        var dto = new CreateRoleRequestDto
        {
            Name = null!,
            Description = "Descrição qualquer"
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "O nome do perfil é obrigatório.");
    }

    [Fact]
    public void CreateRoleRequestDto_NomeExcedeTamanhoMaximo_DeveFalhar()
    {
        // Arrange
        var dto = new CreateRoleRequestDto
        {
            Name = new string('A', 101),
            Description = "Descrição válida"
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "O nome do perfil deve ter no máximo 100 caracteres.");
    }

    [Fact]
    public void CreateRoleRequestDto_DescricaoObrigatoria_DeveFalhar()
    {
        // Arrange
        var dto = new CreateRoleRequestDto
        {
            Name = "Perfil Teste",
            Description = null!
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Descrição é obrigatória.");
    }

    [Fact]
    public void CreateRoleRequestDto_DescricaoExcedeTamanhoMaximo_DeveFalhar()
    {
        // Arrange
        var dto = new CreateRoleRequestDto
        {
            Name = "Perfil Teste",
            Description = new string('B', 256)
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "A descrição do perfil deve ter no máximo 255 caracteres.");
    }

    #endregion

    #region FilterRoleRequestDto Tests

    [Fact]
    public void FilterRoleRequestDto_ComDadosValidos_DeveSerValido()
    {
        // Arrange
        var dto = new FilterRoleRequestDto
        {
            SearchString = "admin",
            PageNumber = 2,
            PageSize = 20
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void FilterRoleRequestDto_SearchStringNula_DeveSerValido()
    {
        // Arrange
        var dto = new FilterRoleRequestDto
        {
            SearchString = null,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void FilterRoleRequestDto_SearchStringExcedeTamanhoMaximo_DeveFalhar()
    {
        // Arrange
        var dto = new FilterRoleRequestDto
        {
            SearchString = new string('C', 101),
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "SearchString deve ter no máximo 100 caracteres.");
    }

    [Fact]
    public void FilterRoleRequestDto_PageNumberMenorQueUm_DeveFalhar()
    {
        // Arrange
        var dto = new FilterRoleRequestDto
        {
            SearchString = "admin",
            PageNumber = 0,
            PageSize = 10
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        // Ajuste conforme a validação real do PaginationRequestDto
        Assert.Contains(results, r => r.ErrorMessage == "PageNumber deve ser maior que zero." || r.ErrorMessage.Contains("PageNumber"));
    }

    [Fact]
    public void FilterRoleRequestDto_PageSizeMenorQueUm_DeveFalhar()
    {
        // Arrange
        var dto = new FilterRoleRequestDto
        {
            SearchString = "admin",
            PageNumber = 1,
            PageSize = 0
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "PageSize deve estar entre 1 e 100." || r.ErrorMessage.Contains("PageSize"));
    }

    [Fact]
    public void FilterRoleRequestDto_PageSizeMaiorQueLimite_DeveFalhar()
    {
        // Arrange
        var dto = new FilterRoleRequestDto
        {
            SearchString = "admin",
            PageNumber = 1,
            PageSize = 101
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "PageSize deve estar entre 1 e 100." || r.ErrorMessage.Contains("PageSize"));
    }

    #endregion

    #region UpdateRoleRequestDto Tests

    [Fact]
    public void UpdateRoleRequestDto_ComDadosValidos_DeveSerValido()
    {
        // Arrange
        var dto = new UpdateRoleRequestDto
        {
            Name = "Administrador",
            Description = "Perfil com acesso total ao sistema."
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void UpdateRoleRequestDto_NomeObrigatorio_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateRoleRequestDto
        {
            Name = null!,
            Description = "Descrição válida"
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "O nome do perfil é obrigatório.");
    }

    [Fact]
    public void UpdateRoleRequestDto_NomeExcedeTamanhoMaximo_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateRoleRequestDto
        {
            Name = new string('A', 101),
            Description = "Descrição válida"
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "O nome do perfil deve ter no máximo 100 caracteres.");
    }

    [Fact]
    public void UpdateRoleRequestDto_DescricaoObrigatoria_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateRoleRequestDto
        {
            Name = "Perfil Teste",
            Description = null!
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Descrição é obrigatória.");
    }

    [Fact]
    public void UpdateRoleRequestDto_DescricaoExcedeTamanhoMaximo_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateRoleRequestDto
        {
            Name = "Perfil Teste",
            Description = new string('B', 256)
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "A descrição do perfil deve ter no máximo 255 caracteres.");
    }

    #endregion
}
