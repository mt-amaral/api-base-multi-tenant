using Api.Client.Configurations;
using Api.Client.Configurations.Identity;
using Api.Client.Configurations.Setup;
using Api.Client.Dto;
using Api.Client.Middleware;
using Api.Client.Services;
using Api.Client.Services.Abstractions;
using Api.Core.Entities.Identity;
using Api.Core.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsStaging())
{
    builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true);
}

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IClaimsTransformation, ClientClaimsTransformation>();

builder.Services.AddCoreDatabase(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddIdentitySetup();
builder.Services.AddAuthorizationSetup();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsDev", policy => policy
        .WithOrigins(ConfigApp.Cors.DevOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());

    options.AddPolicy("CorsProd", policy => policy
        .WithOrigins(ConfigApp.Cors.ProdOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddControllers(options =>
{
    var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim(UserClaimNames.UserType, UserType.Client.ToString())
        .Build();
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
})
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors.Select(e =>
                string.IsNullOrWhiteSpace(e.ErrorMessage)
                    ? $"Campo invalido: {x.Key}"
                    : e.ErrorMessage))
            .ToList();

        var response = new Response<object?>(null, "Dados invalidos.", errors);

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();
app.UseHttpsRedirection();

if (app.Environment.IsProduction())
{
    app.UseCors("CorsProd");
}
else
{
    app.UseCors("CorsDev");
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
