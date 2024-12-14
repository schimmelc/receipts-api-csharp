using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ReceiptsApi.Application;
using ReceiptsApi.Core.Interfaces;
using ReceiptsApi.Web.CustomAttributes;
using ReceiptsApi.Web.Receipts;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

//set up a simple console logger
builder.Logging.AddConsole();
builder.Services.AddLogging();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
  options.AddDocumentTransformer((document, context, cancellationToken) =>
  {
    document.Info = new()
    {
      Title = "Receipt Processor",
      Version = "1.0.0",
      Description = "A simple receipt processor",
    };

    document.Servers.Clear();

    return Task.CompletedTask;
  });

  options.AddSchemaTransformer((schema, context, cancellationToken) =>
  {
    if (context.JsonTypeInfo.Type != null && schema.Properties != null)
    {
      foreach (var property in context.JsonTypeInfo.Type.GetProperties())
      {
        var exampleAttr = property.GetCustomAttribute<OpenApiExampleAttribute>();
        var jsonAttr = property.GetCustomAttribute<JsonPropertyNameAttribute>();
        if (exampleAttr != null && jsonAttr != null && schema.Properties.ContainsKey(jsonAttr.Name))
        {
          schema.Properties[jsonAttr.Name].Example = new OpenApiString(exampleAttr.Example);
        }
      }
    }

    return Task.CompletedTask;
  });
});

builder.Logging.AddOpenTelemetry(options =>
{
  options
      .SetResourceBuilder(
          ResourceBuilder.CreateDefault()
              .AddService("Receipts Api"))
      .AddConsoleExporter()
      .AddOtlpExporter();
});
builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource => resource.AddService("Receipts Api"))
      .WithTracing(tracing => tracing
          .AddAspNetCoreInstrumentation()
          .AddConsoleExporter()
          .AddOtlpExporter())
      .WithMetrics(metrics => metrics
          .AddAspNetCoreInstrumentation()
          .AddConsoleExporter()
          .AddOtlpExporter());

//add an in memory database using entity framework core
builder.Services.AddDbContext<ReceiptsApiDbContext>(options =>
{
  options.UseInMemoryDatabase("ReceiptsApi");
});

builder.Services.AddTransient<IReceiptsRepository, ReceiptsRepository>();
builder.Services.AddSingleton<IPointsCalculator, PointsCalculator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.MapScalarApiReference();
}

//For easier testing, we are not using HTTPS in this example
//app.UseHttpsRedirection();

app.MapPost("/receipts/process", (
  [FromServices] ILogger<Program> logger,
  [FromServices] IReceiptsRepository receiptsRepository,
  [FromServices] IPointsCalculator pointsCalculator,
  [FromBody] Receipt receipt) =>
{
  try
  {
    //validate the receipt
    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(receipt);

    bool isValid = Validator.TryValidateObject(receipt, validationContext, validationResults, true);

    if (!isValid)
    {
      return Results.BadRequest(validationResults);
    }

    // Map the DTO to the entity
    var receiptEntity = new ReceiptEntity
    {
      Retailer = receipt.Retailer,
      PurchaseDate = receipt.PurchaseDate,
      PurchaseTime = receipt.PurchaseTime,
      Items = receipt.Items.Select(item => new ItemEntity
      {
        ShortDescription = item.ShortDescription,
        Price = item.Price
      }).ToList(),
      Total = receipt.Total
    };

    receiptEntity.Points = pointsCalculator.CalculatePointsForReceipt(receiptEntity);

    // Add the receipt entity to the database
    receiptsRepository.AddReceipt(receiptEntity);

    return Results.Ok(new { id = receiptEntity.Id.ToString() });
  }
  catch (Exception e)
  {
    logger.LogError(e, "An error occurred while processing the receipt");
    return Results.Problem("An error occurred while processing the receipt", statusCode: 400);
  }
})
  .Produces<object>(StatusCodes.Status200OK, "application/json")
  .Produces<ProblemDetails>(StatusCodes.Status400BadRequest, "application/json")
  .WithDescription("Submits a receipt for processing")
  .WithSummary("Submits a receipt for processing")
  .Accepts<Receipt>("application/json");

app.MapGet("/receipts/{id}/points", (
  [FromServices] ILogger<Program> logger,
  [FromServices] IReceiptsRepository receiptsRepository,
  [FromRoute][RegularExpression("^\\S+$")] string id) =>
{
  try
  {
    var receipt = receiptsRepository.GetReceiptById(Guid.Parse(id));

    if (receipt == null)
    {
      return Results.NotFound("No receipt found for that id");
    }

    return Results.Ok(new { points = receipt.Points });
  }
  catch (Exception e)
  {
    logger.LogError(e, "An error occurred while retrieving the points for the receipt");
    return Results.Problem("An error occurred while retrieving the points for the receipt", statusCode: 400);
  }
})
  .Produces<object>(200, "application/json")
  .Produces(404)
  .WithDescription("Returns the points awarded for the receipt")
  .WithSummary("Returns the points awarded for the receipt");

await app.RunAsync();
