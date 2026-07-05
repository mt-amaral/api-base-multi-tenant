using Microsoft.AspNetCore.Identity;

namespace Api.Core.Entities.Identity;

public class Role : IdentityRole<long>
{
    public Role() { }
    public string? Description { get; set; }
}