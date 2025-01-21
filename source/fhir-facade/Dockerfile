# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy the csproj and restore any dependencies (via NuGet)
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . ./

# Publish the application to the /app/out folder
RUN dotnet publish -c Release -o /app/out

# Use the official .NET runtime image to run the app (ASP.NET Core runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory for the runtime container
WORKDIR /app

# Copy the published application from the build image
COPY --from=build /app/out .

# The following setting allows for Swagger UI - Disable when in production
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080
EXPOSE 80


# Set the entry point for the application
ENTRYPOINT ["dotnet", "OneCDPFHIRFacade.dll"]
