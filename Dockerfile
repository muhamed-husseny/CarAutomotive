# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["CarAutomotive.sln", "./"]
COPY ["src/CarAutomotive.Core/CarAutomotive.Core.csproj", "src/CarAutomotive.Core/"]
COPY ["src/CarAutomotive.Application/CarAutomotive.Application.csproj", "src/CarAutomotive.Application/"]
COPY ["src/CarAutomotive.Infrastructure/CarAutomotive.Infrastructure.csproj", "src/CarAutomotive.Infrastructure/"]
COPY ["src/CarAutomotive.API/CarAutomotive.API.csproj", "src/CarAutomotive.API/"]
COPY ["CarAutomotive.Tests/CarAutomotive.Tests.csproj", "CarAutomotive.Tests/"]

RUN dotnet restore "CarAutomotive.sln"

COPY . .

RUN dotnet publish "src/CarAutomotive.API/CarAutomotive.API.csproj" -c Release -o /app/publish --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet","CarAutomotive.API.dll"]
