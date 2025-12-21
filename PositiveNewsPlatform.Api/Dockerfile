FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PositiveNewsPlatform.Api/PositiveNewsPlatform.Api.csproj", "PositiveNewsPlatform.Api/"]
COPY ["PositiveNewsPlatform.Application/PositiveNewsPlatform.Application.csproj", "PositiveNewsPlatform.Application/"]
COPY ["PositiveNewsPlatform.Domain/PositiveNewsPlatform.Domain.csproj", "PositiveNewsPlatform.Domain/"]
COPY ["PositiveNewsPlatform.Infrastructure/PositiveNewsPlatform.Infrastructure.csproj", "PositiveNewsPlatform.Infrastructure/"]
RUN dotnet restore "PositiveNewsPlatform.Api/PositiveNewsPlatform.Api.csproj"
COPY . .
WORKDIR "/src/PositiveNewsPlatform.Api"
RUN dotnet build "./PositiveNewsPlatform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PositiveNewsPlatform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PositiveNewsPlatform.Api.dll"]
