using CatalogHub.Api.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfiguration(builder.Configuration);

var app = builder.Build();

app.UseRequestLocalization();

app.UseApiConfiguration(app.Environment);

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogHub.Infrastructure.Data.CatalogHubDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
