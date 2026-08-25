# Backend — Nexa Fiscal

O backend do Nexa Fiscal utiliza **C# com .NET 8 e ASP.NET Core Web API**, organizado em microsserviços com responsabilidades e persistência independentes.

## Serviços

### Auth Service

Responsável por autenticação e identidade da aplicação.

- registro de usuários;
- login;
- hash de senha com PBKDF2 e salt aleatório;
- emissão de tokens JWT;
- persistência própria no MongoDB.

Endpoints principais:

```text
POST /api/auth/register
POST /api/auth/login
```

### Inventory Service

Autoridade sobre produtos e estoque.

- cadastro e consulta de produtos;
- código único, descrição e saldo;
- validação de disponibilidade;
- baixa condicional de estoque;
- resposta detalhada para estoque insuficiente;
- persistência própria no MongoDB.

Endpoints principais:

```text
GET  /api/products
POST /api/products
POST /api/stock/consume
```

### Billing Service

Autoridade sobre o ciclo de vida das notas fiscais.

- criação e consulta de notas;
- numeração sequencial;
- múltiplos produtos e quantidades;
- status `OPEN` e `CLOSED`;
- fechamento da nota;
- comunicação HTTP com o Inventory Service;
- persistência própria no MongoDB.

Endpoints principais:

```text
GET  /api/invoices
GET  /api/invoices/{id}
POST /api/invoices
POST /api/invoices/{id}/close
```

## Organização interna

Cada serviço separa o código em projetos com responsabilidades distintas:

```text
Api             -> endpoints, autenticação e configuração HTTP
Application     -> casos de uso e contratos
Domain          -> entidades e regras de negócio
Infrastructure  -> MongoDB, segurança e integrações externas
```

Os nomes dos projetos incluem o contexto do serviço, por exemplo `NexaFiscal.Billing.Domain`, para manter assemblies e namespaces identificáveis mesmo quando vários serviços são abertos na mesma solução ou pipeline.

## Comunicação entre serviços

Durante o fechamento de uma nota:

1. o Billing Service confirma que a nota está aberta;
2. envia os produtos e quantidades ao Inventory Service;
3. o Inventory Service valida e reduz os saldos;
4. após confirmação, o Billing Service altera a nota para `CLOSED`;
5. em caso de estoque insuficiente ou indisponibilidade, a nota permanece aberta.

O JWT recebido pelo Billing Service é encaminhado ao Inventory Service, mantendo a chamada protegida.

## Persistência

Cada microsserviço é dono dos próprios dados:

```text
auth-service      -> NexaFiscalAuth
inventory-service -> NexaFiscalInventory
billing-service   -> NexaFiscalBilling
```

Os serviços não acessam diretamente as coleções pertencentes a outro contexto.

## Tratamento de erros

As APIs utilizam códigos HTTP semânticos e respostas controladas, incluindo:

- `400 Bad Request` para entrada inválida;
- `404 Not Found` para recursos inexistentes;
- `409 Conflict` para conflitos de negócio, como estoque insuficiente;
- `503 Service Unavailable` quando um serviço dependente ou banco não está disponível.

Erros conhecidos são convertidos em respostas adequadas ao cliente, evitando expor detalhes internos da aplicação.
