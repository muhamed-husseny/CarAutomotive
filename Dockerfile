# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["CarAutomotive.sln", "./"]
COPY ["CarAutomotive.Core/CarAutomotive.Core.csproj", "CarAutomotive.Core/"]
COPY ["CarAutomotive.Application/CarAutomotive.Application.csproj", "CarAutomotive.Application/"]
COPY ["CarAutomotive.Infrastructure/CarAutomotive.Infrastructure.csproj", "CarAutomotive.Infrastructure/"]
COPY ["CarAutomotive.API/CarAutomotive.API.csproj", "CarAutomotive.API/"]
COPY ["CarAutomotive.Tests/CarAutomotive.Tests.csproj", "CarAutomotive.Tests/"]

RUN dotnet restore "CarAutomotive.sln"

COPY . .

RUN dotnet publish "CarAutomotive.API/CarAutomotive.API.csproj" -c Release -o /app/publish --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet","CarAutomotive.API.dll"]
