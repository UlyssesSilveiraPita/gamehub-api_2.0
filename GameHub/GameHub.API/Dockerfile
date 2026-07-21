# Etapa 1: restauração e compilação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["GameHub.API.csproj", "./"]

RUN dotnet restore "GameHub.API.csproj"

COPY . .

RUN dotnet publish "GameHub.API.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore


# Etapa 2: execução da aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Ferramenta utilizada pelo health check do container
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "GameHub.API.dll"]