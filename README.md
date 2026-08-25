# Nexa Fiscal

Nexa Fiscal é uma aplicação web para cadastro de produtos, controle de estoque e emissão simplificada de notas fiscais, construída com **Angular** no frontend e **microsserviços em C# / ASP.NET Core** no backend.

## Frontend

A aplicação inclui:

- login e registro com autenticação JWT;
- dashboard com indicadores;
- cadastro e consulta de produtos com controle de saldo;
- criação de notas fiscais com numeração sequencial, múltiplos produtos e quantidades;
- fechamento de notas com atualização real do estoque;
- feedback para estoque insuficiente e indisponibilidade do serviço de estoque;
- impressão da nota após o fechamento.

## Backend

O backend é dividido em três serviços independentes:

- `auth-service`: usuários, login e emissão de JWT;
- `inventory-service`: produtos, saldos e baixa de estoque;
- `billing-service`: notas fiscais, itens, numeração e fechamento.

Cada serviço possui persistência própria em MongoDB. O Billing Service se comunica com o Inventory Service durante o fechamento da nota.

Mais detalhes em [`backend/README.md`](backend/README.md) e [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md).

## Executando com Docker

Requisito: Docker com Docker Compose.

```bash
docker compose up --build
```

Serviços disponíveis:

| Serviço | Endereço |
| --- | --- |
| Frontend | `http://localhost:4200` |
| Inventory API | `http://localhost:5101` |
| Billing API | `http://localhost:5102` |
| Auth API | `http://localhost:5103` |

Para encerrar:

```bash
docker compose down
```

Os bancos utilizam volumes separados. A variável `JWT_SECRET` pode ser configurada através de um arquivo `.env` baseado em `.env.example`.

## Stack

- Angular 20 + TypeScript + RxJS
- C# + .NET 8 + ASP.NET Core Web API
- MongoDB
- JWT Bearer Authentication
- Docker / Docker Compose

## Documentação

- [Business Requirements Document](docs/BRD.md)
- [Arquitetura da solução](docs/ARCHITECTURE.md)
- [Backend](backend/README.md)
- [Detalhamento técnico](docs/TECHNICAL_DETAILS.md)
