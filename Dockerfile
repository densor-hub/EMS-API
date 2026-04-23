# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy solution file and project file
COPY WebApplication1.sln ./
COPY WebApplication1/WebApplication1.csproj WebApplication1/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build and publish the application
WORKDIR "/src/WebApplication1"
RUN dotnet publish "WebApplication1.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy published files from build stage
COPY --from=publish /app/publish .

# Configure for Render's dynamic port
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "WebApplication1.dll"]