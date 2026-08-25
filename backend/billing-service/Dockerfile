FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/NexaFiscal.Billing.Domain/NexaFiscal.Billing.Domain.csproj src/NexaFiscal.Billing.Domain/
COPY src/NexaFiscal.Billing.Application/NexaFiscal.Billing.Application.csproj src/NexaFiscal.Billing.Application/
COPY src/NexaFiscal.Billing.Infrastructure/NexaFiscal.Billing.Infrastructure.csproj src/NexaFiscal.Billing.Infrastructure/
COPY src/NexaFiscal.Billing.Api/NexaFiscal.Billing.Api.csproj src/NexaFiscal.Billing.Api/

RUN dotnet restore src/NexaFiscal.Billing.Api/NexaFiscal.Billing.Api.csproj

COPY src ./src
RUN dotnet publish src/NexaFiscal.Billing.Api/NexaFiscal.Billing.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "NexaFiscal.Billing.Api.dll"]
