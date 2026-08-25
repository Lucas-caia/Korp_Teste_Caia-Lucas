# Backend — Nexa Fiscal

O backend será composto por **dois microsserviços independentes**, cada um responsável por seus próprios dados e regras.

```text
backend/
├── inventory-service/
│   ├── src/
│   │   ├── NexaFiscal.Inventory.Api/
│   │   ├── NexaFiscal.Inventory.Application/
│   │   ├── NexaFiscal.Inventory.Domain/
│   │   └── NexaFiscal.Inventory.Infrastructure/
│   └── tests/
└── billing-service/
    ├── src/
    │   ├── NexaFiscal.Billing.Api/
    │   ├── NexaFiscal.Billing.Application/
    │   ├── NexaFiscal.Billing.Domain/
    │   └── NexaFiscal.Billing.Infrastructure/
    └── tests/
```

## Inventory Service

Autoridade sobre produtos e saldos. Futuramente também concentrará atualização atômica, concorrência e idempotência das baixas de estoque.

## Billing Service

Autoridade sobre notas fiscais. Será responsável pela numeração sequencial, itens, status e orquestração do fechamento junto ao Inventory Service.

> Este primeiro commit contém somente a estrutura. Nenhum comportamento backend foi implementado ainda.
