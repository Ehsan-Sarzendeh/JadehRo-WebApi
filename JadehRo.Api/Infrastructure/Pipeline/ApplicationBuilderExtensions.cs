namespace JadehRo.Api.Infrastructure.Pipeline;

public static class ApplicationBuilderExtensions
{
    public static void InitializeDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        dbContext!.Database.Migrate();
    }
}