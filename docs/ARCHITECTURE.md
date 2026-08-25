# Arquitetura inicial — Nexa Fiscal

## Visão macro

```text
Angular Web
   ├── HTTP -> Inventory Service -> inventory-db (MongoDB)
   └── HTTP -> Billing Service   -> billing-db (MongoDB)
                     |
                     └── HTTP -> Inventory Service
```

O frontend nunca deverá alterar saldo ou status de faturamento diretamente. Essas regras ficarão no backend quando os microsserviços forem implementados.

## Limites dos serviços

### Inventory Service

- produtos;
- saldo;
- validação de disponibilidade;
- baixa atômica;
- concorrência;
- registro idempotente de operações.

### Billing Service

- notas fiscais;
- itens;
- numeração sequencial;
- status `OPEN` / `CLOSED`;
- coordenação do fechamento.

## Banco de dados

Cada serviço será dono do próprio conjunto de dados. Mesmo utilizando uma única instância MongoDB no desenvolvimento local, serão utilizados bancos lógicos separados (`inventory-db` e `billing-db`).

## Estado atual

O Angular utiliza mocks em memória somente para tornar o primeiro commit demonstrável. Eles não representam a arquitetura final e serão substituídos por chamadas HTTP reais.
