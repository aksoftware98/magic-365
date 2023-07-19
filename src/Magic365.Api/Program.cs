using Magic365.Api.Extensions;
using Magic365.Api.Middlewares;
using Magic365.Core;
using Magic365.Core.Interfaces;
using Magic365.Shared;
using Magic365.Shared.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddLanguageUnderstandingService(builder.Configuration["LanguageServiceApiKey"] ?? string.Empty);
builder.Services.AddCoreServices(builder.Configuration);

builder.Services.AddHttpContextAccessor();
// Add the Graph service client and an authorized HttpClient 
builder.Services.AddAuthorizedHttpClient();

builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseMiddleware<ErrorHandlingMiddleware>(); 
app.UseHttpsRedirection();

app.UseAuthorization();

#region API Endpoints
app.MapPost("/analyze-note", async ([FromBody]SubmitNoteRequest note, [FromServices] INoteAnalyzingService analyzingService) =>
{
	var result = await analyzingService.AnalyzePlanAsync(note.Query);
	return Results.Ok(result);
})
	.WithName("Analyze Note")
	.WithDescription("Submit a note to be analyzed by AI and return a plan object of the graph services to be done")
	.WithOpenApi();

app.MapPost("/submit-plan", async ([FromBody] PlanDetails plan, IPlanningService planningService) =>
{
	await planningService.SubmitPlanAsync(plan);
	return Results.Ok();
})
	.WithName("Submit Plan")
	.WithDescription("Submit a plan so the planned items inside it can be created via the Microsoft Graph")
	.WithOpenApi();

app.MapGet("/contacts/search", async ([FromQuery] string query, IPlanningService planningService) =>
{
    var result = await planningService.FetchContactsAsync(query);
    return Results.Ok(result);
})
    .WithName("Search contacts")
    .WithDescription("Search the contacts of the user using Microsoft Graph")
    .WithOpenApi();


app.MapGet("/tasks/list", async (IGraphDataService planningService) =>
{
    var result = await planningService.GetLastTasksAsync();
    return Results.Ok(result);
})
    .WithName("List pending tasks")
    .WithDescription("Retrieve 5 pending tasks from the 'Tasks' list in Microsoft To Do")
    .WithOpenApi();

app.MapGet("/events/list", async (IGraphDataService planningService) =>
{
    var result = await planningService.GetUpcomingEventsAsync();
    return Results.Ok(result);
})
    .WithName("List upcoming calendar events")
    .WithDescription("Retrieve the upcoming 5 calendar events")
    .WithOpenApi();

app.MapPost("/usages/start-session", async ([FromBody] SaveUserSessionDto session, IUsageTrackingService trackingService) =>
{
    var result = await trackingService.SaveUserSessionAsync(session);
    return Results.Ok(result);
})
    .WithName("Track User Session")
    .WithDescription("Track and start a user session")
    .WithOpenApi();

app.MapPost("/usages/track-event", async ([FromBody] TrackUserEventDto eventDto, IUsageTrackingService trackingService) =>
{
    await trackingService.TrackEventAsync(eventDto);
    return Results.Ok();
})
    .WithName("Track User Event")
    .WithDescription("Track and start a user session")
    .WithOpenApi();

app.MapGet("/plans/history", async ([FromQuery] string userId, IPlansStorageService plansStorageService) =>
{
    var result = await plansStorageService.ListPlansAsync(userId);
    return Results.Ok(result.OrderByDescending(o => o.PlanDate).Select(r => new PlanHistoryDto
    {
        Note = r.Note,
        Date = r.PlanDate,
        EventsCount = r.EventsCount,
        MeetingsCount = r.MeetingsCount,
        ToDoItemsCount = r.ToDoItemsCount
    }));
})
    .RequireAuthorization()
    .WithName("List History")
    .WithDescription("List the history of submitted notes")
    .WithOpenApi();
#endregion 

app.Run();
