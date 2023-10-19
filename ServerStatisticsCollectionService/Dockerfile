# Use the .NET 6 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the application files and restore dependencies
COPY . ./
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o out

# Use a smaller runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Copy the published output from the build stage to the runtime image
COPY --from=build /app/out ./

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80
ENV SamplingIntervalSeconds=4
ENV ServerIdentifier=Server123
ENV RabbitMessageQueueConnection=rabbitmq
ENV RabbitMessageQueueQueueName=StatMessages

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "ServerStatisticsCollectionService.dll"]
