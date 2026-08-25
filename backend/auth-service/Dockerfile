FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/NexaFiscal.Auth.Domain/NexaFiscal.Auth.Domain.csproj src/NexaFiscal.Auth.Domain/
COPY src/NexaFiscal.Auth.Application/NexaFiscal.Auth.Application.csproj src/NexaFiscal.Auth.Application/
COPY src/NexaFiscal.Auth.Infrastructure/NexaFiscal.Auth.Infrastructure.csproj src/NexaFiscal.Auth.Infrastructure/
COPY src/NexaFiscal.Auth.Api/NexaFiscal.Auth.Api.csproj src/NexaFiscal.Auth.Api/

RUN dotnet restore src/NexaFiscal.Auth.Api/NexaFiscal.Auth.Api.csproj

COPY src ./src
RUN dotnet publish src/NexaFiscal.Auth.Api/NexaFiscal.Auth.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "NexaFiscal.Auth.Api.dll"]
