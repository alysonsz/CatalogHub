using Amazon.S3;
using CatalogHub.Application.Interfaces.Services;
using CatalogHub.Application.Services;
using CatalogHub.Domain.Interfaces.Repository;
using CatalogHub.Infrastructure.Data;
using CatalogHub.Infrastructure.Repository;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Globalization;

namespace CatalogHub.Api.Configuration;

public static class ApiConfiguration
{
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogHubDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();

        services.AddScoped<IAmazonS3>(sp =>
        {
            var s3Config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2
            };

            var accessKey = configuration["AWS:AccessKey"];
            var secretKey = configuration["AWS:SecretKey"];
            var region = configuration["AWS:Region"];

            return new AmazonS3Client(accessKey, secretKey, s3Config);
        });

        services.AddScoped<IStorageService, AWSStorageService>();

        services.AddControllers()
            .AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
            });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CatalogHub API",
                Version = "v1",
                Description = "API para gerenciamento de produtos e categorias."
            });
        });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });

        var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
        if (!Directory.Exists(imageDir))
            Directory.CreateDirectory(imageDir);

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var defaultCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
            CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

            options.DefaultRequestCulture = new RequestCulture(defaultCulture);
            options.SupportedCultures = new[] { defaultCulture };
            options.SupportedUICultures = new[] { defaultCulture };
        });

        return services;
    }

    public static IApplicationBuilder UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseCors("AllowAll");
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CatalogHub API v1");
            c.RoutePrefix = "swagger";
        });

        app.UseStaticFiles();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        return app;
    }
}
