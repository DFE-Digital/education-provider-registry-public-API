ARG DOTNET_VERSION=10.0

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG BUILD_CONFIGURATION=Release

COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Directory.Packages.props", "src/"]
COPY ["src/DfE.EducationProviderRegistry.PublicApi.Api/DfE.EducationProviderRegistry.PublicApi.Api.csproj", "src/DfE.EducationProviderRegistry.PublicApi.Api/"]

RUN --mount=type=secret,id=nuget_config \
    dotnet restore "./src/DfE.EducationProviderRegistry.PublicApi.Api/DfE.EducationProviderRegistry.PublicApi.Api.csproj" \
    --configfile /run/secrets/nuget_config

COPY . .

WORKDIR "/src/DfE.EducationProviderRegistry.PublicApi.Api"

RUN dotnet build "./DfE.EducationProviderRegistry.PublicApi.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./DfE.EducationProviderRegistry.PublicApi.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore

# This stage is used in production
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-noble-chiseled@sha256:a5532f95f87501535df78fd1b0f39a6356d3a5beaaa5d8637cb62931cd2747ab AS final

USER $APP_UID

EXPOSE 8080
EXPOSE 8081

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "DfE.EducationProviderRegistry.PublicApi.Api.dll"]