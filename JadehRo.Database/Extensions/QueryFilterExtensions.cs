using JadehRo.Database.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JadehRo.Database.Extensions;

public static class QueryFilterExtensions
{
    public static void ApplyIsActiveQueryFilters(this ModelBuilder modelBuilder)
    {
        var softDeleteEntities = typeof(ISoftDelete).Assembly.GetTypes()
            .Where(type => typeof(ISoftDelete).IsAssignableFrom(type) && type is { IsClass: true, IsAbstract: false });

        foreach (var softDeleteEntity in softDeleteEntities)
        {
            modelBuilder.Entity(softDeleteEntity).HasQueryFilter(GenerateIsActiveLambdaExpression(softDeleteEntity));
        }
    }

    private static LambdaExpression GenerateIsActiveLambdaExpression(Type type)
    {
        // e =>
        var parameter = Expression.Parameter(type, "e");

        // true
        var falseConstant = Expression.Constant(true);

        // e.IsActive
        var propertyAccess = Expression.PropertyOrField(parameter, nameof(ISoftDelete.IsActive));

        // e.IsActive == true
        var equalExpression = Expression.Equal(propertyAccess, falseConstant);

        // e => e.IsActive == true
        var lambda = Expression.Lambda(equalExpression, parameter);

        return lambda;
    }
}