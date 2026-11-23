using DfE.EducationProviderRegistry.Core.Query.Establishments;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.PublicApi.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure versioning
builder.Services
    .AddApiVersioning()
    .AddMvc();

// DbContext
builder.Services
    .AddEducationProviderRegistryDbContext(builder.Configuration);

// Establishment services
builder.Services
    .AddEstablishmentsInfrastructureDependencies()
    .AddEstablishmentsUseCaseDependencies();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
