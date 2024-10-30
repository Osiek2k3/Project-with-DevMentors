FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MySpot.Api/MySpot.Api.csproj", "MySpot.Api/"]
COPY ["MySpot.Application/MySpot.Application.csproj", "MySpot.Application/"]
COPY ["MySpot.Core/MySpot.Core.csproj", "MySpot.Core/"]
COPY ["MySpot.Infrastructure/MySpot.Infrastructure.csproj", "MySpot.Infrastructure/"]
RUN dotnet restore "MySpot.Api/MySpot.Api.csproj"

COPY . .
WORKDIR /src
RUN dotnet build "MySpot.Api/MySpot.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MySpot.Api/MySpot.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MySpot.Api.dll"]
