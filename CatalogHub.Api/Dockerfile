
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CatalogHub.Api/CatalogHub.Api.csproj", "CatalogHub.Api/"]
COPY ["CatalogHub.Application/CatalogHub.Application.csproj", "CatalogHub.Application/"]
COPY ["CatalogHub.Domain/CatalogHub.Domain.csproj", "CatalogHub.Domain/"]
COPY ["CatalogHub.Infrastructure/CatalogHub.Infrastructure.csproj", "CatalogHub.Infrastructure/"]
RUN dotnet restore "CatalogHub.Api/CatalogHub.Api.csproj"

COPY . .
WORKDIR "/src/CatalogHub.Api"
RUN dotnet publish "CatalogHub.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CatalogHub.Api.dll"]