using System.ComponentModel.DataAnnotations;
using Api.Dto.User;
using Xunit;

namespace Api.UnitTests.Validators;

public class UserTests
{
    private static bool TryValidate(object obj, out List<ValidationResult> results)
    {
        var context = new ValidationContext(obj, serviceProvider: null, items: null);
        results = new List<ValidationResult>();
        return Validator.TryValidateObject(obj, context, results, validateAllProperties: true);
    }

    #region CreateRequestDto Tests

    [Fact]
    public void CreateRequestDto_ComDadosValidos_DeveSerValido()
    {
        // Arrange
        var dto = new CreateRequestDto
        {
            UserName = "joaosilva",
            Email = "joao@email.com",
            NewPassword = "123456",
            ConfirmPassword = "123456",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void CreateRequestDto_UserNameObrigatorio_DeveFalhar()
    {
        // Arrange
        var dto = new CreateRequestDto
        {
            UserName = null!,
            Email = "joao@email.com",
            NewPassword = "123456",
            ConfirmPassword = "123456",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "UserName é obrigatório.");
    }

    [Fact]
    public void CreateRequestDto_UserNameCurto_DeveFalhar()
    {
        // Arrange
        var dto = new CreateRequestDto
        {
            UserName = "ab",
            Email = "joao@email.com",
            NewPassword = "123456",
            ConfirmPassword = "123456",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "UserName deve ter entre 3 e 100 caracteres.");
    }

    [Fact]
    public void CreateRequestDto_UserNameLongo_DeveFalhar()
    {
        // Arrange
        var dto = new CreateRequestDto
        {
            UserName = new string('A', 101),
            Email = "joao@email.com",
            NewPassword = "123456",
            ConfirmPassword = "123456",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "UserName deve ter entre 3 e 100 caracteres.");
    }

    [Fact]
    public void CreateRequestDto_EmailObrigatorio_DeveFalhar()
    {
        // Arrange
        var dto = new CreateRequestDto
        {
            UserName = "joaosilva",
            Email = null!,
            NewPassword = "123456",
            ConfirmPassword = "123456",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Email é obrigatório.");
    }

    [Fact]
    public void CreateRequestDto_EmailInvalido_DeveFalhar()
    {
        // Arrange
        var dto = new CreateRequestDto
        {
            UserName = "joaosilva",
            Email = "invalido",
            NewPassword = "123456",
            ConfirmPassword = "123456",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Email inválido.");
    }

    [Fact]
    public void CreateRequestDto_NewPasswordCurta_DeveFalhar()
    {
        // Arrange
        var dto = new CreateRequestDto
        {
            UserName = "joaosilva",
            Email = "joao@email.com",
            NewPassword = "12345",
            ConfirmPassword = "12345",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "NewPassword deve ter entre 6 e 100 caracteres.");
    }

    [Fact]
    public void CreateRequestDto_NewPasswordLonga_DeveFalhar()
    {
        // Arrange
        var dto = new CreateRequestDto
        {
            UserName = "joaosilva",
            Email = "joao@email.com",
            NewPassword = new string('A', 101),
            ConfirmPassword = new string('A', 101),
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "NewPassword deve ter entre 6 e 100 caracteres.");
    }

    [Fact]
    public void CreateRequestDto_ConfirmPasswordDiferente_DeveFalhar()
    {
        // Arrange
        var dto = new CreateRequestDto
        {
            UserName = "joaosilva",
            Email = "joao@email.com",
            NewPassword = "123456",
            ConfirmPassword = "654321",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "ConfirmPassword deve ser igual a NewPassword.");
    }

    [Fact]
    public void CreateRequestDto_NewPasswordNula_DeveSerValidoSeConfirmPasswordNula()
    {
        // Arrange
        var dto = new CreateRequestDto
        {
            UserName = "joaosilva",
            Email = "joao@email.com",
            NewPassword = null,
            ConfirmPassword = null,
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void CreateRequestDto_NewPasswordNulaComConfirmPasswordNaoNula_DeveFalhar()
    {
        // Arrange
        var dto = new CreateRequestDto
        {
            UserName = "joaosilva",
            Email = "joao@email.com",
            NewPassword = null,
            ConfirmPassword = "123456",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "ConfirmPassword deve ser igual a NewPassword.");
    }

    [Fact]
    public void CreateRequestDto_RoleIdInvalido_MenorQueUm_DeveFalhar()
    {
        // Arrange
        var dto = new CreateRequestDto
        {
            UserName = "joaosilva",
            Email = "joao@email.com",
            NewPassword = "123456",
            ConfirmPassword = "123456",
            RoleId = 0
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "RoleId deve ser maior que 0.");
    }

    #endregion

    #region FilterUsersRequestDto Tests

    [Fact]
    public void FilterUsersRequestDto_ComDadosValidos_DeveSerValido()
    {
        // Arrange
        var dto = new FilterUsersRequestDto
        {
            SearchString = "joao",
            RoleId = 1,
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
    public void FilterUsersRequestDto_SearchStringNula_DeveSerValido()
    {
        // Arrange
        var dto = new FilterUsersRequestDto
        {
            SearchString = null,
            RoleId = null,
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
    public void FilterUsersRequestDto_SearchStringExcedeTamanhoMaximo_DeveFalhar()
    {
        // Arrange
        var dto = new FilterUsersRequestDto
        {
            SearchString = new string('A', 101),
            RoleId = null,
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
    public void FilterUsersRequestDto_RoleIdInvalido_MenorQueUm_DeveFalhar()
    {
        // Arrange
        var dto = new FilterUsersRequestDto
        {
            SearchString = "joao",
            RoleId = 0,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "RoleId inválido.");
    }

    [Fact]
    public void FilterUsersRequestDto_RoleIdNulo_DeveSerValido()
    {
        // Arrange
        var dto = new FilterUsersRequestDto
        {
            SearchString = "joao",
            RoleId = null,
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
    public void FilterUsersRequestDto_PageNumberMenorQueUm_DeveFalhar()
    {
        // Arrange
        var dto = new FilterUsersRequestDto
        {
            SearchString = "joao",
            RoleId = null,
            PageNumber = 0,
            PageSize = 10
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        // Mensagem dependerá da validação em PaginationRequestDto
        Assert.Contains(results, r => r.ErrorMessage == "PageNumber deve ser maior que zero." || r.ErrorMessage.Contains("PageNumber"));
    }

    [Fact]
    public void FilterUsersRequestDto_PageSizeMenorQueUm_DeveFalhar()
    {
        // Arrange
        var dto = new FilterUsersRequestDto
        {
            SearchString = "joao",
            RoleId = null,
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
    public void FilterUsersRequestDto_PageSizeMaiorQueLimite_DeveFalhar()
    {
        // Arrange
        var dto = new FilterUsersRequestDto
        {
            SearchString = "joao",
            RoleId = null,
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

    #region UpdateUserRequestDto Tests

    [Fact]
    public void UpdateUserRequestDto_ComDadosValidos_DeveSerValido()
    {
        // Arrange
        var dto = new UpdateUserRequestDto
        {
            UserName = "joaosilva",
            Email = "joao@email.com",
            NewPassword = "123456",
            ConfirmPassword = "123456",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void UpdateUserRequestDto_UserNameObrigatorio_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateUserRequestDto
        {
            UserName = null!,
            Email = "joao@email.com",
            NewPassword = "123456",
            ConfirmPassword = "123456",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "UserName é obrigatório.");
    }

    [Fact]
    public void UpdateUserRequestDto_UserNameCurto_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateUserRequestDto
        {
            UserName = "ab",
            Email = "joao@email.com",
            NewPassword = "123456",
            ConfirmPassword = "123456",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "UserName deve ter no mínimo 3 caracteres.");
    }

    [Fact]
    public void UpdateUserRequestDto_UserNameLongo_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateUserRequestDto
        {
            UserName = new string('A', 101),
            Email = "joao@email.com",
            NewPassword = "123456",
            ConfirmPassword = "123456",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "UserName deve ter no máximo 100 caracteres.");
    }

    [Fact]
    public void UpdateUserRequestDto_EmailObrigatorio_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateUserRequestDto
        {
            UserName = "joaosilva",
            Email = null!,
            NewPassword = "123456",
            ConfirmPassword = "123456",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Email é obrigatório.");
    }

    [Fact]
    public void UpdateUserRequestDto_EmailInvalido_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateUserRequestDto
        {
            UserName = "joaosilva",
            Email = "invalido",
            NewPassword = "123456",
            ConfirmPassword = "123456",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Email inválido.");
    }

    [Fact]
    public void UpdateUserRequestDto_NewPasswordCurta_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateUserRequestDto
        {
            UserName = "joaosilva",
            Email = "joao@email.com",
            NewPassword = "12345",
            ConfirmPassword = "12345",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "NewPassword deve ter no mínimo 6 caracteres.");
    }

    [Fact]
    public void UpdateUserRequestDto_NewPasswordLonga_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateUserRequestDto
        {
            UserName = "joaosilva",
            Email = "joao@email.com",
            NewPassword = new string('A', 101),
            ConfirmPassword = new string('A', 101),
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "NewPassword deve ter no máximo 100 caracteres.");
    }

    [Fact]
    public void UpdateUserRequestDto_ConfirmPasswordDiferente_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateUserRequestDto
        {
            UserName = "joaosilva",
            Email = "joao@email.com",
            NewPassword = "123456",
            ConfirmPassword = "654321",
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "ConfirmPassword deve ser igual a NewPassword.");
    }

    [Fact]
    public void UpdateUserRequestDto_NewPasswordNula_DeveSerValido()
    {
        // Arrange
        var dto = new UpdateUserRequestDto
        {
            UserName = "joaosilva",
            Email = "joao@email.com",
            NewPassword = null,
            ConfirmPassword = null,
            RoleId = 1
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void UpdateUserRequestDto_RoleIdInvalido_MenorQueUm_DeveFalhar()
    {
        // Arrange
        var dto = new UpdateUserRequestDto
        {
            UserName = "joaosilva",
            Email = "joao@email.com",
            NewPassword = "123456",
            ConfirmPassword = "123456",
            RoleId = 0
        };

        // Act
        var isValid = TryValidate(dto, out var results);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "RoleId inválido.");
    }

    #endregion
}