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

ENV MemoryUsageAnomalyThresholdPercentage=0.4
ENV CpuUsageAnomalyThresholdPercentage=0.5
ENV MemoryUsageThresholdPercentage=0.8
ENV CpuUsageThresholdPercentage=0.9
ENV MongoDBConfigConnectionString=mongodb://admin:password@mongodb:27017
ENV MongoDBConfigDatabaseName=ServerMonitoring
ENV MongoDBConfigServerStatisticsCollection=ServerStatistics
ENV RabbitMessageQueueConnection=rabbitmq
ENV RabbitMessageQueueQueueName=StatMessages

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "ConsumerService.dll"]
