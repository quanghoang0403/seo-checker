using Application.Features.Search.Queries;
using Caching;
using Caching.MemoryCache;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Infrastructure.Services.SearchServices;
using Sympli.SearchApp.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SearchRequestHandler).Assembly));

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<GoogleSearchService>();
builder.Services.AddHttpClient<BingSearchService>();

// Dependency Injection
builder.Services.AddSingleton<ISearchService, GoogleSearchService>();
builder.Services.AddSingleton<ISearchService, BingSearchService>();
builder.Services.AddSingleton<ISearchServiceFactory, SearchServiceFactory>();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseCors("AllowAllOrigins");

app.MapControllers();

app.Run();
