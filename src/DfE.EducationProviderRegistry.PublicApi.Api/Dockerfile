ARG DOTNET_VERSION=10.0

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG BUILD_CONFIGURATION=Release

COPY ["src/DfE.EducationProviderRegistry.IntegrationsApi.Api/DfE.EducationProviderRegistry.IntegrationsApi.Api.csproj", "src/DfE.EducationProviderRegistry.IntegrationsApi.Api/"]
RUN dotnet restore "./src/DfE.EducationProviderRegistry.IntegrationsApi.Api/DfE.EducationProviderRegistry.IntegrationsApi.Api.csproj"

COPY . .
WORKDIR "/src/DfE.EducationProviderRegistry.IntegrationsApi.Api"
RUN dotnet build "./DfE.EducationProviderRegistry.IntegrationsApi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DfE.EducationProviderRegistry.IntegrationsApi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-noble-chiseled@sha256:a5532f95f87501535df78fd1b0f39a6356d3a5beaaa5d8637cb62931cd2747ab AS final
USER $APP_UID
EXPOSE 8080
EXPOSE 8081

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DfE.EducationProviderRegistry.IntegrationsApi.Api.dll"]
