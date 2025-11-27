# Use the official .NET image as a parent image
# This image includes the runtime and is used for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This image includes the SDK and is used for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project files and restore dependencies
# We copy only the csproj files first to leverage Docker cache
COPY ["Wallet.RestAPI/Wallet.RestAPI.csproj", "Wallet.RestAPI/"]
COPY ["Wallet.Funcionalidad/Wallet.Funcionalidad.csproj", "Wallet.Funcionalidad/"]
COPY ["Wallet.DOM/Wallet.DOM.csproj", "Wallet.DOM/"]
COPY ["Wallet.UnitTest/Wallet.UnitTest.csproj", "Wallet.UnitTest/"]

RUN dotnet restore "Wallet.RestAPI/Wallet.RestAPI.csproj"

# Copy the remaining files and build the application
COPY . .
WORKDIR "/src/Wallet.RestAPI"
RUN dotnet build "Wallet.RestAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Wallet.RestAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wallet.RestAPI.dll"]
