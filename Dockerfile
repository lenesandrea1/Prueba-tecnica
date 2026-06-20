FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY EventosVivos.sln ./
COPY global.json ./
COPY src/EventosVivos.Domain/EventosVivos.Domain.csproj src/EventosVivos.Domain/
COPY src/EventosVivos.Application/EventosVivos.Application.csproj src/EventosVivos.Application/
COPY src/EventosVivos.Infrastructure/EventosVivos.Infrastructure.csproj src/EventosVivos.Infrastructure/
COPY src/EventosVivos.Api/EventosVivos.Api.csproj src/EventosVivos.Api/

RUN dotnet restore src/EventosVivos.Api/EventosVivos.Api.csproj

COPY src/ src/
RUN dotnet publish src/EventosVivos.Api/EventosVivos.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "EventosVivos.Api.dll"]
