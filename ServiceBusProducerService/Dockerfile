# Use the official .NET runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the .csproj file and restore the dependencies
COPY ["ServiceBusProducerService/ServiceBusProducerService.csproj", "ServiceBusProducerService/"]
RUN dotnet restore "ServiceBusProducerService/ServiceBusProducerService.csproj"

# Copy the rest of the application code
COPY . .

WORKDIR "/src/ServiceBusProducerService"
RUN dotnet build "ServiceBusProducerService.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ServiceBusProducerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create the final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the entry point for the container
ENTRYPOINT ["dotnet", "ServiceBusProducerService.dll"]
