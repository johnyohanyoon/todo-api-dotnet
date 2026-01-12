using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAngular",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:4200",
                    "http://localhost:8080",
                    "https://todo-app-angular-n8j2.onrender.com"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders(
                    "X-Total-Count",
                    "X-Page-Number",
                    "X-Page-Size",
                    "X-Total-Pages"
                );
        }
    );
});

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions =>
            sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            )
    )
);

var app = builder.Build();

// Seed database with 100,000 rows if empty
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TodoContext>();

    // Check if database is empty
    if (!context.TodoItems.Any())
    {
        Console.WriteLine("Seeding database with 100,000 todos...");

        var random = new Random();
        var todos = new List<TodoItem>();

        for (int i = 1; i <= 100000; i++)
        {
            todos.Add(
                new TodoItem
                {
                    Name = $"Todo Item {i}",
                    IsComplete = random.Next(0, 2) == 1, // Random true/false
                }
            );

            // Add in batches of 1000 to avoid memory issues
            if (i % 1000 == 0)
            {
                context.TodoItems.AddRange(todos);
                await context.SaveChangesAsync();
                todos.Clear();
                Console.WriteLine($"Seeded {i} todos...");
            }
        }

        Console.WriteLine("Database seeding complete!");
    }
    else
    {
        Console.WriteLine(
            $"Database already contains {context.TodoItems.Count()} todos. Skipping seed."
        );
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

// API Key authentication middleware
app.Use(async (context, next) =>
{
    // Skip authentication for OPTIONS requests (CORS preflight)
    if (context.Request.Method == "OPTIONS")
    {
        await next();
        return;
    }

    // Skip authentication for OpenAPI/Swagger endpoints in development
    if (context.Request.Path.StartsWithSegments("/openapi") ||
        context.Request.Path.StartsWithSegments("/swagger"))
    {
        await next();
        return;
    }

    var apiKey = context.RequestServices
        .GetRequiredService<IConfiguration>()
        .GetValue<string>("ApiKey");

    if (string.IsNullOrEmpty(apiKey))
    {
        // If no API key is configured, skip authentication (allows gradual rollout)
        await next();
        return;
    }

    if (!context.Request.Headers.TryGetValue("X-API-Key", out var providedApiKey) ||
        providedApiKey != apiKey)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new { error = "Invalid or missing API key" });
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
