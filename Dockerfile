# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["exampleAPI.Api/exampleAPI.Api.csproj", "exampleAPI.Api/"]
COPY ["exampleAPI.Application/exampleAPI.Application.csproj", "exampleAPI.Application/"]
COPY ["exampleAPI.Infrastructure/exampleAPI.Infrastructure.csproj", "exampleAPI.Infrastructure/"]
COPY ["exampleAPI.Domain/exampleAPI.Domain.csproj", "exampleAPI.Domain/"]

RUN dotnet restore "exampleAPI.Api/exampleAPI.Api.csproj"

COPY . .
WORKDIR "/src/exampleAPI.Api"
RUN dotnet build "exampleAPI.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "exampleAPI.Api.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "exampleAPI.Api.dll"]
