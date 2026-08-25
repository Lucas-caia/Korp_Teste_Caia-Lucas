using System.Net;
using System.Net.Http.Json;
using NexaFiscal.Billing.Application.Abstractions;
using NexaFiscal.Billing.Application.Exceptions;
using NexaFiscal.Billing.Application.Models;

namespace NexaFiscal.Billing.Infrastructure.Http;

public sealed class InventoryGateway(HttpClient httpClient) : IInventoryGateway
{
    public async Task ConsumeAsync(
        IReadOnlyCollection<StockConsumptionItem> items,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClient.PostAsJsonAsync(
                "api/stock/consume",
                new ConsumeStockRequest(items),
                cancellationToken);

            if (response.IsSuccessStatusCode)
                return;

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var error = await response.Content.ReadFromJsonAsync<StockConflictResponse>(
                    cancellationToken: cancellationToken);

                throw new StockInsufficientException(
                    error?.Items ?? Array.Empty<StockShortage>());
            }

            if ((int)response.StatusCode >= 500)
            {
                throw new InventoryUnavailableException(
                    "O serviço de estoque está temporariamente indisponível.");
            }

            var detail = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InventoryUnavailableException(
                $"Não foi possível concluir a operação de estoque. HTTP {(int)response.StatusCode}. {detail}");
        }
        catch (StockInsufficientException)
        {
            throw;
        }
        catch (InventoryUnavailableException)
        {
            throw;
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            throw new InventoryUnavailableException(
                "O serviço de estoque não respondeu dentro do tempo esperado.",
                exception);
        }
        catch (HttpRequestException exception)
        {
            throw new InventoryUnavailableException(
                "Não foi possível conectar ao serviço de estoque.",
                exception);
        }
    }

    private sealed record ConsumeStockRequest(IReadOnlyCollection<StockConsumptionItem> Items);

    private sealed record StockConflictResponse(
        string? Title,
        string? Detail,
        int Status,
        IReadOnlyCollection<StockShortage> Items);
}
