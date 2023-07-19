using System.Text.Json;
using Azure.Data.Tables;
using Magic365.Core.Interfaces;
using Magic365.Core.Models;
using Magic365.Core.Options;
using Magic365.Shared.DTOs;

namespace Magic365.Core.Services;

public class TableStoragePlansService : IPlansStorageService
{
    private readonly TableClient _plansTable;
    private readonly MicrosoftAuthenticationOptions _authOptions;
    private readonly IdentityOptions _identityOptions;
    public TableStoragePlansService(string connectionString, MicrosoftAuthenticationOptions authOptions, IdentityOptions identityOptions)
    {
        _plansTable = new TableClient(connectionString, "UserPlans");
        _authOptions = authOptions;
        _identityOptions = identityOptions;
    }

    public async Task SavePlanAsync(PlanMetadata session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        await _plansTable.CreateIfNotExistsAsync();
        await _plansTable.AddEntityAsync(session);

    }

    public async Task<IEnumerable<PlanMetadata>> ListPlansAsync(string userId)
    {
        var plans = new List<PlanMetadata>();

        await _plansTable.CreateIfNotExistsAsync();
        var query = TableClient.CreateQueryFilter<PlanMetadata>(p => p.UserId == userId);
        await foreach (var plan in _plansTable.QueryAsync<PlanMetadata>(query))
        {
            plans.Add(plan);
        }

        return plans;
    }

}
