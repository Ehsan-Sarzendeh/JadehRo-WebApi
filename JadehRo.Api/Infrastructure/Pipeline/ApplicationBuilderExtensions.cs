using JadehRo.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace JadehRo.Api.Infrastructure.Pipeline;

public static class ApplicationBuilderExtensions
{
    public static void InitializeDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
        dbContext!.Database.Migrate();
    }
}