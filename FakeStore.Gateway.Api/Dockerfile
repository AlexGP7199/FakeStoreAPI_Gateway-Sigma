# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar todos los archivos de proyecto primero (para aprovechar el caché de Docker)
COPY ["FakeStore.Gateway.Api/FakeStore.Gateway.Api.csproj", "FakeStore.Gateway.Api/"]
COPY ["FakeStore.Gateway.Application/FakeStore.Gateway.Application.csproj", "FakeStore.Gateway.Application/"]
COPY ["FakeStore.Gateway.Infrastructure/FakeStore.Gateway.Infrastructure.csproj", "FakeStore.Gateway.Infrastructure/"]
COPY ["FakeStore.Gateway.Domain/FakeStore.Gateway.Domain.csproj", "FakeStore.Gateway.Domain/"]

# Restore de todas las dependencias
RUN dotnet restore "FakeStore.Gateway.Api/FakeStore.Gateway.Api.csproj"

# Copiar el resto del código
COPY . .

# Build del proyecto
WORKDIR "/src/FakeStore.Gateway.Api"
RUN dotnet build "FakeStore.Gateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FakeStore.Gateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FakeStore.Gateway.Api.dll"]