FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/NexaFiscal.Inventory.Domain/NexaFiscal.Inventory.Domain.csproj src/NexaFiscal.Inventory.Domain/
COPY src/NexaFiscal.Inventory.Application/NexaFiscal.Inventory.Application.csproj src/NexaFiscal.Inventory.Application/
COPY src/NexaFiscal.Inventory.Infrastructure/NexaFiscal.Inventory.Infrastructure.csproj src/NexaFiscal.Inventory.Infrastructure/
COPY src/NexaFiscal.Inventory.Api/NexaFiscal.Inventory.Api.csproj src/NexaFiscal.Inventory.Api/

RUN dotnet restore src/NexaFiscal.Inventory.Api/NexaFiscal.Inventory.Api.csproj

COPY src ./src
RUN dotnet publish src/NexaFiscal.Inventory.Api/NexaFiscal.Inventory.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "NexaFiscal.Inventory.Api.dll"]
