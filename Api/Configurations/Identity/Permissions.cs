namespace Api.Configurations.Identity;

public static class Permissions
{
    // Roles
    public const string Admin = "Admin";

    // ClaimTypes
    public const string Permission = "permission";

    // Users
    public const string UsersView = "users.view";
    public const string UsersRegister = "users.register";
    public const string UsersUpdate = "users.update";
    public const string UsersDelete = "users.delete";
    public const string RolesView = "users.roles.view";
    public const string RolesRegister = "users.roles.register";
    public const string RolesUpdate = "users.roles.update";
    public const string RolesDelete = "users.roles.delete";
    public const string ClaimsView = "users.claims.view";
    public const string ClaimsUpdate = "users.claims.update";


    public static IEnumerable<PermissionDefinition> GetPermissions()
    {
        return new List<PermissionDefinition>
        {
            new(UsersView, Permission, "Pode visualizar usuarios"),
            new(UsersRegister, Permission, "Pode criar novos usuários"),
            new(UsersUpdate, Permission, "Pode atualizar dados de usuários"),
            new(UsersDelete, Permission, "Pode deletar usuários"),

            new(RolesView, Permission, "Pode visualizar perfis"),
            new(RolesRegister, Permission, "Pode criar novos perfis"),
            new(RolesUpdate, Permission, "Pode atualizar dados de perfis"),
            new(RolesDelete, Permission, "Pode deletar perfis"),

            new(ClaimsView, Permission, "Pode visualizar permissões de perfil"),
            new(ClaimsUpdate, Permission, "Pode atualizar permissões de perfil"),
        };
    }
}

public record PermissionDefinition(string PermissionName, string ClaimType, string Description);