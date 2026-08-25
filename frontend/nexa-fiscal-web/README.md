# Nexa Fiscal Web

Frontend Angular do Nexa Fiscal.

## Primeiro commit

Nesta etapa os serviços `InventoryService` e `BillingService` utilizam estado em memória para permitir navegar e demonstrar o fluxo antes da implementação das APIs.

A estrutura foi organizada para facilitar a substituição dos mocks por comunicação HTTP real sem alterar as páginas.

## Rotas

- `/` — Dashboard
- `/products` — Produtos
- `/invoices` — Notas fiscais
- `/invoices/new` — Nova nota
- `/invoices/:id` — Detalhes da nota

## Executar

```bash
npm install
npm start
```

## Próxima etapa

- implementar `Inventory.Api` em ASP.NET Core;
- implementar `Billing.Api` em ASP.NET Core;
- substituir os serviços locais por `HttpClient` + RxJS;
- adicionar interceptor de erros e correlation ID;
- conectar MongoDB.
