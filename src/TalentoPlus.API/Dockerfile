FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Development 

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TalentoPlus.API/TalentoPlus.API.csproj", "src/TalentoPlus.API/"]
COPY ["src/TalentoPlus.Application/TalentoPlus.Application.csproj", "src/TalentoPlus.Application/"]
COPY ["src/TalentoPlus.Domain/TalentoPlus.Domain.csproj", "src/TalentoPlus.Domain/"]
COPY ["src/TalentoPlus.Infrastructure/TalentoPlus.Infrastructure.csproj", "src/TalentoPlus.Infrastructure/"]
RUN dotnet restore "src/TalentoPlus.API/TalentoPlus.API.csproj"
COPY . .
WORKDIR "/src/src/TalentoPlus.API"
RUN dotnet build "./TalentoPlus.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TalentoPlus.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TalentoPlus.API.dll"]
