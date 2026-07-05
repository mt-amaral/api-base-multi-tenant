namespace Api.Configurations.Setup;

public static class SwaggerSetup
{
    public static IServiceCollection AddSwaggerSetup(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
            c.SwaggerDoc("v1", new()
            {
                Title = "Api Doc",
                Description = ""
            });
        });

        return services;
    }
}
