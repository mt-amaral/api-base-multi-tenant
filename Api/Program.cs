using Api.Configurations;
using Api.Configurations.Identity;
using Api.Configurations.Setup;
using Api.Configurations.Seed;
using Api.Configurations.Seed.Abstraction;
using Api.Core.Extensions;
using Api.Core.Entities.Identity;
using Api.Dto;
using Api.Middleware;
using Api.Services;
using Api.Services.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsStaging())
{
    builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true);
}

builder.AddSerilogSetup();

builder.Services.AddHealthChecks();

builder.AddOpenTelemetrySetup();

// Services
builder.Services.AddScoped<IAccountServices, AccountServices>();
builder.Services.AddScoped<IUserLoggedService, UserLoggedService>();
builder.Services.AddScoped<IRoleService, RoleServices>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleClaimService, RoleClaimService>();
builder.Services.AddScoped<IClaimsTransformation, AppClaimsTransformation>();

if (builder.Environment.IsStaging())
{
    builder.Services.AddScoped<IAppSeed, CompanySeed>();
    builder.Services.AddScoped<IAppSeed, RoleSeed>();
    builder.Services.AddScoped<IAppSeed, AdminUserSeed>();
    builder.Services.AddScoped<IAppSeed, ClientUserSeed>();
    builder.Services.AddScoped<IAppSeed, RoleClaimSeed>();
}

if (!builder.Environment.IsDevelopment())
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });
}

builder.Services.AddRateLimiterSetup();

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

builder.Services.AddIdentitySetup();

builder.Services.AddAuthorizationSetup();

builder.Services.AddControllers(options =>
{
    var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim(UserClaimNames.UserType, UserType.Admin.ToString())
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
                    ? $"Campo inválido: {x.Key}"
                    : e.ErrorMessage))
            .ToList();

        var response = new Response<object?>(
            null,
            "Dados inválidos.",
            errors
        );

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddSwaggerSetup();

builder.Services.AddCoreDatabase(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Admin API V1");
        c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
    });
}

if (app.Environment.IsProduction())
{
}

if (app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Admin API V1");
        c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
    });

    await app.UseSeedSetupAsync();
}

app.UseRouting();
app.UseForwardedHeaders();
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

app.UseRateLimiter();
app.MapControllers().RequireRateLimiting("api");
app.UseMiddleware<ExceptionMiddleware>();
app.UseOpenTelemetryPrometheusScrapingEndpoint(); 
app.MapHealthChecks("/health");

app.Run();
