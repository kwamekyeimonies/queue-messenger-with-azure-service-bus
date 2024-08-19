# Use the official .NET 8 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["ServiceBusProducerService.csproj", "./"]
RUN dotnet restore

# Copy the remaining files and build the application
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# Use the official .NET 8 runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the application will listen on
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "ServiceBusProducerService.dll"]
