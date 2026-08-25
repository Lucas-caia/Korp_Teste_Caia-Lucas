# Nexa Fiscal

Nexa Fiscal é uma aplicação web para cadastro de produtos, controle de estoque e emissão simplificada de notas fiscais.

Este repositório foi iniciado para o desafio técnico e está organizado desde o primeiro commit para evoluir com **Angular no frontend** e **microsserviços em C# / ASP.NET Core no backend**.

### Frontend

O frontend já é navegável e funcional utilizando dados locais em memória. Ele inclui:

- Dashboard com indicadores;
- Cadastro e consulta de produtos;
- Busca e estados de estoque;
- Listagem e filtros de notas fiscais;
- Criação de nota com múltiplos produtos;
- Fechamento da nota e atualização simulada de estoque;
- Tratamento visual de estoque insuficiente;
- Detalhes e impressão da nota;
- Nexa Insights com análise local demonstrativa;
- Layout responsivo;
- Login e registro integrados ao serviço de autenticação.

### Backend

Os dois microsserviços obrigatórios continuam separados:

- `inventory-service`: produtos e estoque;
- `billing-service`: notas fiscais e faturamento.

Além deles, a autenticação foi isolada em um serviço de suporte:

- `auth-service`: registro de usuários, login e emissão de JWT.

Cada serviço possui seus próprios limites de responsabilidade. Inventory e Billing já estão preparados para validar os tokens emitidos pelo Auth Service.

## Executando com Docker

Com Docker e Docker Compose instalados, toda a estrutura pode ser iniciada a partir da raiz do repositório:

```bash
docker compose up --build
```

Serviços disponíveis:

- Frontend: `http://localhost:4200`
- Auth API: `http://localhost:5103`
- Inventory API: `http://localhost:5101`
- Billing API: `http://localhost:5102`
- Auth MongoDB: `localhost:27017`
- Inventory MongoDB: `localhost:27018`
- Billing MongoDB: `localhost:27019`

Para encerrar os containers:

```bash
docker compose down
```

Os bancos utilizam volumes separados, mantendo a propriedade dos dados de cada microsserviço.

O Compose possui uma chave JWT apenas para desenvolvimento local. Para fornecer uma chave própria, copie `.env.example` para `.env` e altere `JWT_SECRET`.

## Executando o frontend

Requisitos sugeridos:

- Node.js 20+
- npm 10+

```bash
cd frontend/nexa-fiscal-web
npm install
npm start
```

Depois acesse `http://localhost:4200`.

Para utilizar login e registro executando apenas o Angular localmente, o Auth Service também deve estar disponível em `http://localhost:5103`.

## Stack planejada

- Angular + TypeScript + RxJS
- C# + ASP.NET Core Web API
- JWT Bearer Authentication
- MongoDB
- OpenAPI / Swagger
- Docker / Docker Compose
- xUnit
- Serilog
- Resiliência HTTP entre os microsserviços

## Documentação

O BRD inicial está disponível em [`docs/BRD.md`](docs/BRD.md).
