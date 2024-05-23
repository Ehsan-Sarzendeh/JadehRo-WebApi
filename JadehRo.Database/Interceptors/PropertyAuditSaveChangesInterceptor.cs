using System.Security.Claims;
using JadehRo.Database.Entities.Base;
using JadehRo.Database.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace JadehRo.Database.Interceptors;

public class PropertyAuditSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyAudits(eventData.Context.ChangeTracker);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        ApplyAudits(eventData.Context.ChangeTracker);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAudits(ChangeTracker changeTracker)
    {
        ApplyCreateAudits(changeTracker);
        ApplyUpdateAudits(changeTracker);
        ApplyDeleteAudits(changeTracker);
    }

    private void ApplyCreateAudits(ChangeTracker changeTracker)
    {
        var addedEntries = changeTracker.Entries()
            .Where(x => x.State == EntityState.Added);

        foreach (var addedEntry in addedEntries)
        {
            if (addedEntry.Entity is ISoftDelete entity)
            {
                entity.IsActive = true;
            }
            if (addedEntry.Entity is BaseAuditableEntity entity3)
            {
                var user = httpContextAccessor.HttpContext?.User;
                var currUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
                entity3.CreatedUserId = 1;
                entity3.ModifiedUserId = 1;
                if (currUserId != null)
                {
                    entity3.CreatedUserId = int.Parse(currUserId);
                    entity3.ModifiedUserId = int.Parse(currUserId);
                }

                entity3.CreatedDateTime = DateTime.Now;
                entity3.ModifiedDateTime = DateTime.Now;
            }
            if (addedEntry.Entity is BaseAuditableEntity<long> entity5)
            {
                var user = httpContextAccessor.HttpContext?.User;
                var currUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
                entity5.CreatedUserId = 1;
                entity5.ModifiedUserId = 1;
                if (currUserId != null)
                {
                    entity5.CreatedUserId = int.Parse(currUserId);
                    entity5.ModifiedUserId = int.Parse(currUserId);
                }

                entity5.CreatedDateTime = DateTime.Now;
                entity5.ModifiedDateTime = DateTime.Now;
            }
            if (addedEntry.Entity is BaseAuditableEntity<int> entity6)
            {
                var user = httpContextAccessor.HttpContext?.User;
                var currUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
                entity6.CreatedUserId = 1;
                entity6.ModifiedUserId = 1;
                if (currUserId != null)
                {
                    entity6.CreatedUserId = int.Parse(currUserId);
                    entity6.ModifiedUserId = int.Parse(currUserId);
                }

                entity6.CreatedDateTime = DateTime.Now;
                entity6.ModifiedDateTime = DateTime.Now;
            }
        }
    }

    private void ApplyUpdateAudits(ChangeTracker changeTracker)
    {
        var modifiedEntries = changeTracker.Entries<BaseAuditableEntity>()
            .Where(x => x.State == EntityState.Modified);

        foreach (var modifiedEntry in modifiedEntries)
        {
            if (modifiedEntry.Entity is { } entity)
            {
                var user = httpContextAccessor.HttpContext?.User;
                var currUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
                entity.ModifiedUserId = 1;
                if (currUserId != null)
                {
                    entity.ModifiedUserId = int.Parse(currUserId);
                }

                entity.ModifiedDateTime = DateTime.Now;

                modifiedEntry.Property(x => x.CreatedUserId).IsModified = false;
                modifiedEntry.Property(x => x.CreatedDateTime).IsModified = false;
            }
        }

        var modifiedEntriesGuid = changeTracker.Entries<BaseAuditableEntity<Guid>>()
            .Where(x => x.State == EntityState.Modified);

        foreach (var modifiedEntry in modifiedEntriesGuid)
        {
            if (modifiedEntry.Entity is { } entity)
            {
                entity.ModifiedDateTime = DateTime.Now;
            }
        }
    }

    private void ApplyDeleteAudits(ChangeTracker changeTracker)
    {
        var deletedEntries = changeTracker.Entries()
            .Where(x => x is { State: EntityState.Deleted, Entity: IEntity and not UserRole });

        foreach (var deletedEntry in deletedEntries)
        {
            if (deletedEntry.Entity is ISoftDelete entity)
            {
                deletedEntry.State = EntityState.Modified;
                entity.IsActive = false;
                entity.DeleteAt = DateTime.Now;
            }
        }
    }
}