# Business Requirements Document — Nexa Fiscal

**Projeto:** Nexa Fiscal  
**Versão:** 1.0  
**Autor:** Caiã Lucas Leite Oliveira  
**Período:** 19/08/2026 - 26/08/2026  
**Status:** Planejamento inicial

---

## 1. Visão Geral

### 1.1 Contexto

O **Nexa Fiscal** é uma aplicação web para gerenciamento simplificado de produtos, controle de estoque e emissão de notas fiscais. A solução será desenvolvida como um projeto técnico, com frontend em Angular e backend em C#, estruturado em microsserviços independentes para Estoque e Faturamento.

O sistema prioriza clareza de uso, consistência dos dados e comportamento previsível diante de falhas, incluindo cenários de concorrência, repetição de requisições e indisponibilidade temporária entre serviços.

### 1.2 Problema

Operações de faturamento dependem diretamente da disponibilidade e da consistência do estoque. Sem mecanismos adequados, duas notas podem consumir simultaneamente o mesmo saldo, uma requisição repetida pode baixar o estoque mais de uma vez ou uma falha entre serviços pode deixar o sistema em estado inconsistente.

O Nexa Fiscal busca resolver esses problemas em um fluxo simples de emissão de notas, mantendo responsabilidades bem separadas entre Estoque e Faturamento.

### 1.3 Objetivo

Desenvolver uma solução web para cadastro de produtos, controle de estoque e emissão de notas fiscais, garantindo:

- persistência real dos dados;
- numeração sequencial das notas;
- inclusão de múltiplos produtos;
- atualização consistente do estoque;
- fechamento seguro das notas;
- recuperação adequada de falhas;
- proteção contra concorrência e requisições duplicadas;
- feedback claro ao usuário durante operações e situações de erro.

---

## 2. Objetivos de Negócio

- Centralizar o cadastro e a consulta de produtos disponíveis para faturamento.
- Permitir a criação e o fechamento de notas fiscais de forma simples e confiável.
- Garantir que o saldo de estoque permaneça consistente durante operações simultâneas.
- Fornecer uma experiência clara ao usuário, com feedback visual durante processamento, sucesso e falhas.
- Demonstrar uma arquitetura preparada para cenários reais de comunicação entre serviços.

### Critérios de sucesso

O projeto será considerado bem-sucedido quando:

- os requisitos funcionais obrigatórios estiverem implementados e demonstráveis;
- os dados forem persistidos fisicamente em banco de dados;
- Estoque e Faturamento estiverem separados em microsserviços;
- uma falha de comunicação entre microsserviços puder ser simulada e recuperada sem corromper o estado do sistema;
- o fechamento de uma nota atualizar corretamente os saldos dos produtos;
- uma nota fechada não puder ser processada novamente;
- operações concorrentes não permitirem saldo negativo;
- requisições repetidas de fechamento não provocarem múltiplas baixas no estoque;
- a funcionalidade opcional de Inteligência Artificial estiver integrada sem tornar o fluxo principal dependente dela;
- a aplicação apresentar estados claros de carregamento, sucesso e erro.

---

## 3. Escopo

### 3.1 Dentro do escopo

- Cadastro de produtos.
- Consulta de produtos.
- Controle de saldo disponível.
- Criação de notas fiscais.
- Numeração sequencial de notas.
- Status `Aberta` e `Fechada`.
- Inclusão de múltiplos produtos e respectivas quantidades.
- Consulta de notas fiscais.
- Fechamento e impressão de notas.
- Atualização do estoque durante o fechamento.
- Validação de estoque suficiente.
- Tratamento de indisponibilidade entre serviços.
- Persistência dos dados em MongoDB.
- Tratamento de concorrência sobre o saldo.
- Idempotência no fechamento de notas.
- Feedback visual de processamento e falhas.
- Logs estruturados e health checks.
- Documentação das APIs.
- Funcionalidade complementar utilizando Inteligência Artificial.
- Execução local dos componentes por containers.

### 3.2 Fora do escopo

Para manter o projeto alinhado ao desafio e evitar complexidade sem valor para a demonstração, não fazem parte desta versão:

- autenticação e autorização de usuários;
- integração oficial com SEFAZ;
- emissão de NF-e com validade fiscal;
- geração ou assinatura de XML fiscal;
- cálculo de tributos;
- cadastro de clientes, fornecedores ou transportadoras;
- meios de pagamento;
- contas a pagar ou receber;
- funcionalidades completas de ERP;
- gestão de múltiplos depósitos;
- aplicação mobile nativa.

---

## 4. Stakeholders

| Stakeholder | Responsabilidade / Interesse |
| --- | --- |
| Operador do sistema | Cadastrar produtos, consultar estoque, criar e fechar notas |
| Setor de faturamento | Gerenciar notas fiscais e acompanhar seu status |
| Estoque | Manter saldo correto e impedir consumo acima da disponibilidade |
| Equipe técnica | Manter os serviços, analisar falhas, logs e saúde da aplicação |

---

## 5. Requisitos Funcionais

### RF-01 — Cadastro de Produto

O sistema deve permitir o cadastro de produtos contendo:

- Código;
- Descrição;
- Saldo disponível.

**Critérios de aceitação:**

- Código é obrigatório.
- Descrição é obrigatória.
- Saldo é obrigatório.
- O saldo inicial não pode ser negativo.
- O código do produto deve ser único.
- Após o cadastro, o produto deve estar disponível para utilização em notas fiscais.

---

### RF-02 — Consulta de Produtos

O sistema deve permitir visualizar os produtos cadastrados e seus respectivos saldos.

**Critérios de aceitação:**

- A listagem deve exibir código, descrição e saldo.
- O usuário deve conseguir identificar produtos com saldo baixo ou zerado.
- Os dados apresentados devem refletir o estado persistido no serviço de Estoque.

---

### RF-03 — Criação de Nota Fiscal

O sistema deve permitir a criação de notas fiscais com numeração sequencial.

**Critérios de aceitação:**

- A numeração deve ser gerada pelo sistema.
- Cada número de nota deve ser único.
- Uma nova nota deve possuir status `Aberta`.
- Deve ser possível incluir múltiplos produtos.
- Cada produto deve possuir uma quantidade maior que zero.
- A nota deve ser persistida antes do fechamento.

---

### RF-04 — Fechamento de Nota Fiscal

O sistema deve permitir o fechamento de uma nota fiscal aberta.

**Critérios de aceitação:**

- Apenas notas com status `Aberta` podem ser fechadas.
- O estoque deve ser validado antes da confirmação do fechamento.
- Todos os itens da nota devem possuir saldo suficiente.
- O saldo dos produtos deve ser atualizado conforme as quantidades utilizadas.
- Após a confirmação da operação, a nota deve ser alterada para `Fechada`.
- Uma falha durante o processo não deve causar nova baixa de estoque ao repetir a requisição.
- Em caso de indisponibilidade temporária, a nota deve permanecer em um estado recuperável.

---

### RF-05 — Impressão de Nota Fiscal

O sistema deve disponibilizar uma ação de impressão para notas elegíveis.

**Critérios de aceitação:**

- O botão de impressão deve ser claramente identificável.
- Ao iniciar a operação, a interface deve exibir um indicador de processamento.
- Apenas notas com status `Aberta` podem iniciar o processo de fechamento/impressão.
- Após a conclusão do fechamento, a visualização de impressão deve apresentar os dados da nota.
- Notas já fechadas podem ser consultadas novamente, sem realizar uma nova baixa no estoque.

---

### RF-06 — Tratamento de Falhas

O sistema deve lidar adequadamente com indisponibilidade entre os microsserviços.

**Critérios de aceitação:**

- O usuário deve receber uma mensagem compreensível quando a operação não puder ser concluída.
- Erros técnicos internos não devem ser expostos diretamente ao usuário.
- A indisponibilidade do serviço de Estoque não deve fechar incorretamente a nota.
- O sistema deve permitir nova tentativa após a recuperação do serviço.
- A repetição da operação após uma resposta incerta não deve duplicar a baixa de estoque.

---

### RF-07 — Tratamento de Concorrência

O sistema deve impedir que operações simultâneas consumam o mesmo saldo além da quantidade disponível.

**Critérios de aceitação:**

- Um produto com saldo `1` utilizado simultaneamente por duas notas não pode resultar em saldo negativo.
- Apenas a operação que conseguir reservar/consumir o saldo disponível deve ser concluída.
- A operação concorrente sem saldo suficiente deve receber uma resposta de conflito ou estoque insuficiente.
- A atualização do saldo deve ocorrer de forma atômica.

---

### RF-08 — Idempotência

O fechamento de uma nota deve ser idempotente.

**Critérios de aceitação:**

- Uma mesma operação deve possuir uma chave de idempotência.
- Requisições repetidas com a mesma chave não podem baixar o estoque novamente.
- Quando uma operação já tiver sido concluída, uma nova chamada equivalente deve retornar o resultado previamente registrado ou um estado semanticamente equivalente.
- A idempotência deve funcionar também após falhas de comunicação entre os serviços.

---

### RF-09 — Assistente Inteligente

O sistema deve oferecer uma funcionalidade complementar baseada em Inteligência Artificial para gerar um resumo operacional da nota e dos itens envolvidos.

**Objetivo da funcionalidade:**

A IA poderá gerar um pequeno insight textual, por exemplo:

- destacar produtos que ficarão com saldo baixo após o fechamento;
- resumir o impacto da nota no estoque;
- indicar itens que merecem atenção operacional.

**Critérios de aceitação:**

- A funcionalidade deve ser claramente identificada como assistência complementar.
- A IA não pode alterar saldos, fechar notas ou executar regras críticas do sistema.
- Falhas na integração de IA não devem impedir o uso das funcionalidades principais.
- A resposta gerada deve utilizar somente os dados necessários ao contexto da nota.

---

## 6. Regras de Negócio

### RN-01 — Status inicial

Toda nova nota fiscal deve possuir status `Aberta`.

### RN-02 — Fechamento

Somente uma nota com status `Aberta` poderá ser fechada.

### RN-03 — Estoque

Uma nota não poderá consumir quantidade superior ao saldo disponível.

### RN-04 — Atualização de saldo

Ao fechar uma nota, o estoque dos produtos deverá ser reduzido pelas respectivas quantidades utilizadas.

### RN-05 — Imutabilidade

Após ser fechada, uma nota fiscal não poderá ter seus itens, quantidades ou estado de faturamento alterados.

A nota poderá continuar disponível para consulta e reimpressão, mas qualquer tentativa de novo processamento não deverá provocar efeitos adicionais no estoque.

### RN-06 — Concorrência

A redução de saldo deve utilizar uma operação atômica e condicionada à existência de estoque suficiente.

Quando duas operações concorrentes disputarem o mesmo saldo, somente aquela que conseguir satisfazer a condição de disponibilidade poderá concluir o consumo. A outra operação deverá falhar de maneira controlada, sem permitir saldo negativo.

### RN-07 — Idempotência

Cada tentativa de fechamento deverá utilizar uma identificação única da operação.

O serviço de Estoque deve registrar operações de baixa já processadas. Caso receba novamente a mesma operação, deverá reconhecer a repetição e impedir uma segunda alteração de saldo.

Para o fechamento da nota, o identificador da nota poderá participar da composição da chave idempotente, garantindo que uma repetição causada por timeout, retry ou ação duplicada do usuário não produza efeitos colaterais.

### RN-08 — Numeração da nota

A numeração da nota fiscal deve ser sequencial e única.

### RN-09 — Quantidades

As quantidades adicionadas aos itens de uma nota devem ser números inteiros maiores que zero.

### RN-10 — Integridade do fechamento

A nota somente poderá ser marcada como `Fechada` após o serviço de Estoque confirmar com sucesso o processamento da baixa correspondente.

---

## 7. Requisitos Não Funcionais

### RNF-01 — Confiabilidade

O sistema deve evitar inconsistências entre notas fiscais e estoque, inclusive após timeouts, retries ou falhas temporárias de comunicação.

### RNF-02 — Persistência

Os dados devem ser armazenados de forma persistente em MongoDB.

### RNF-03 — Disponibilidade e Resiliência

Falhas temporárias em um microsserviço não devem causar corrupção de dados.

Chamadas entre serviços devem possuir timeout definido e política controlada de recuperação, evitando esperas indefinidas e tentativas excessivas.

### RNF-04 — Usabilidade

A interface deve:

- apresentar feedback visual durante operações assíncronas;
- comunicar erros em linguagem compreensível;
- diferenciar claramente notas abertas e fechadas;
- impedir ações inválidas quando possível;
- manter uma navegação simples entre Produtos e Notas.

### RNF-05 — Manutenibilidade

A solução deve possuir separação clara de responsabilidades entre frontend, serviço de Estoque e serviço de Faturamento.

As regras de negócio devem permanecer desacopladas de detalhes de infraestrutura sempre que viável.

### RNF-06 — Observabilidade

Os microsserviços devem fornecer mecanismos básicos de observabilidade, incluindo:

- logs estruturados;
- identificador de correlação por requisição;
- registro de erros com contexto suficiente para diagnóstico;
- health checks;
- logs das comunicações críticas entre Faturamento e Estoque.

Dados sensíveis, chaves de API e segredos não devem ser registrados nos logs.

### RNF-07 — Testabilidade

As principais regras de negócio devem ser cobertas por testes automatizados.

Devem existir testes para:

- criação de notas;
- validação de saldo;
- fechamento;
- idempotência;
- concorrência;
- tratamento de falhas críticas.

### RNF-08 — Segurança básica

Segredos de integração e strings de conexão não devem ser versionados diretamente no repositório.

Configurações sensíveis devem ser fornecidas por variáveis de ambiente ou mecanismo equivalente.

### RNF-09 — Documentação

As APIs devem possuir documentação OpenAPI acessível no ambiente de desenvolvimento, permitindo inspecionar e testar seus endpoints.

---

## 8. Arquitetura Proposta

A solução será estruturada com frontend desacoplado e dois microsserviços de backend.

### Frontend — Angular

Responsabilidades:

- apresentar a interface ao usuário;
- gerenciar formulários;
- validar dados de entrada;
- consumir as APIs;
- controlar estados de carregamento;
- apresentar mensagens de sucesso e erro;
- utilizar RxJS nos fluxos assíncronos de comunicação.

### Inventory Service — ASP.NET Core

Responsável por:

- cadastro e consulta de produtos;
- armazenamento do saldo;
- validação de disponibilidade;
- atualização atômica do estoque;
- proteção contra concorrência;
- registro das operações idempotentes de baixa;
- exposição de health check.

### Billing Service — ASP.NET Core

Responsável por:

- criação e consulta de notas;
- geração da numeração sequencial;
- gerenciamento dos itens;
- controle do status da nota;
- coordenação do processo de fechamento;
- comunicação resiliente com o Inventory Service;
- integração opcional com o assistente de IA;
- exposição de health check.

### Persistência — MongoDB

A persistência será realizada em MongoDB.

Cada microsserviço será responsável pelos seus próprios dados lógicos, evitando acesso direto à coleção pertencente ao outro serviço.

Estrutura proposta:

- `inventory-db`: produtos e operações idempotentes de estoque;
- `billing-db`: notas fiscais e controle da sequência de numeração.

Para garantir atomicidade em operações que envolvam múltiplos documentos do Estoque, o ambiente poderá utilizar MongoDB configurado como replica set de nó único durante o desenvolvimento local, permitindo o uso de transações quando necessário.

### Comunicação entre serviços

A comunicação principal entre Billing Service e Inventory Service será síncrona via HTTP.

O fechamento seguirá a estratégia:

1. Billing valida o estado da nota.
2. Billing cria/reutiliza o identificador idempotente da operação.
3. Billing solicita ao Inventory Service a baixa dos produtos.
4. Inventory valida o saldo e processa a baixa atomicamente.
5. Inventory registra a operação como processada.
6. Billing recebe a confirmação.
7. Billing altera a nota para `Fechada`.

Se houver uma falha após o Estoque processar a operação, a nova tentativa utilizará a mesma chave idempotente, permitindo que o Billing Service recupere o resultado sem realizar uma nova baixa.

---

## 9. Modelo de Dados

### Product

| Campo | Descrição |
| --- | --- |
| `id` | Identificador interno |
| `code` | Código único do produto |
| `description` | Nome ou descrição |
| `balance` | Quantidade disponível |
| `createdAt` | Data de criação |
| `updatedAt` | Data da última alteração |

### Invoice

| Campo | Descrição |
| --- | --- |
| `id` | Identificador interno |
| `number` | Número sequencial |
| `status` | `OPEN` / `CLOSED` |
| `items` | Itens pertencentes à nota |
| `operationId` | Identificador da operação de fechamento |
| `createdAt` | Data de criação |
| `closedAt` | Data de fechamento |

### InvoiceItem

| Campo | Descrição |
| --- | --- |
| `productId` | Identificador do produto |
| `productCode` | Código do produto no momento da inclusão |
| `description` | Descrição necessária para apresentação da nota |
| `quantity` | Quantidade utilizada |

### InventoryOperation

| Campo | Descrição |
| --- | --- |
| `id` | Identificador interno |
| `operationId` | Chave idempotente única |
| `invoiceId` | Nota associada |
| `status` | Estado da operação |
| `items` | Produtos e quantidades processadas |
| `processedAt` | Data de processamento |

---

## 10. Fluxo Principal

### Emissão de Nota Fiscal

1. O usuário acessa a área de notas.
2. O usuário cria uma nova nota.
3. O Billing Service gera a numeração sequencial.
4. A nota é persistida com status `Aberta`.
5. O usuário seleciona produtos e informa as quantidades.
6. O usuário solicita o fechamento/impressão.
7. A interface exibe um indicador de processamento.
8. O Billing Service valida o estado da nota.
9. O Billing Service solicita ao Inventory Service a baixa idempotente do estoque.
10. O Inventory Service verifica a disponibilidade.
11. O Inventory Service realiza a atualização de saldo de forma atômica.
12. O Inventory Service confirma a operação.
13. O Billing Service altera a nota para `Fechada`.
14. A interface informa o sucesso.
15. O sistema disponibiliza a visualização apropriada para impressão.

---

## 11. Fluxos de Exceção

### 11.1 Estoque insuficiente

1. O usuário solicita o fechamento.
2. O Inventory Service identifica saldo insuficiente em um ou mais produtos.
3. Nenhum saldo deve ser parcialmente consumido.
4. O Inventory Service retorna uma resposta de negócio indicando os itens sem disponibilidade.
5. A nota permanece `Aberta`.
6. O frontend apresenta uma mensagem clara ao usuário.

### 11.2 Inventory Service indisponível

1. O Billing Service tenta consultar/processar o Estoque.
2. A comunicação excede o timeout ou retorna indisponibilidade.
3. A política de resiliência realiza apenas as tentativas configuradas.
4. O fechamento não é confirmado.
5. A nota permanece `Aberta`.
6. O usuário recebe feedback de indisponibilidade temporária.
7. Após a recuperação do Inventory Service, a operação poderá ser repetida utilizando a mesma identificação idempotente quando aplicável.

### 11.3 Requisição duplicada

1. Uma operação de fechamento é enviada.
2. O Estoque processa a baixa utilizando uma chave idempotente.
3. A mesma requisição é enviada novamente.
4. O Inventory Service identifica que a operação já foi processada.
5. Nenhuma nova alteração de saldo é realizada.
6. O resultado compatível com a operação original é retornado.

### 11.4 Concorrência

1. Duas notas tentam consumir simultaneamente o último item disponível.
2. Ambas iniciam o processo de fechamento.
3. O Inventory Service executa a validação e a atualização de forma atômica.
4. Uma das operações consome o saldo.
5. A segunda não satisfaz mais a condição de disponibilidade.
6. A primeira nota é concluída.
7. A segunda permanece aberta e recebe a informação de estoque insuficiente.

### 11.5 Falha após a baixa do estoque

1. O Inventory Service conclui a baixa.
2. O Billing Service não recebe a resposta ou falha antes de atualizar a nota.
3. Uma nova tentativa é realizada com a mesma chave.
4. O Inventory Service reconhece a operação anterior e não altera o estoque novamente.
5. O Billing Service recebe a confirmação da operação já processada.
6. A nota pode concluir o fechamento de maneira segura.

---

## 12. Estratégia de Tratamento de Falhas

A comunicação entre microsserviços deverá possuir tratamento explícito para falhas transitórias e falhas de negócio.

### Backend

A estratégia inclui:

- tratamento centralizado de exceções;
- respostas HTTP padronizadas;
- `ProblemDetails` para erros de API;
- timeouts para chamadas externas;
- retries apenas para falhas transitórias e operações seguras;
- circuit breaker na comunicação entre serviços;
- logs estruturados;
- correlation ID;
- health checks;
- idempotência no fluxo crítico de fechamento.

Retries não deverão ser utilizados indiscriminadamente em operações que possam produzir efeitos colaterais. O mecanismo de idempotência será utilizado para tornar a repetição do fechamento segura.

### Frontend

O frontend deverá:

- apresentar estado de loading durante operações;
- impedir cliques repetidos enquanto uma operação estiver em andamento;
- interpretar respostas de negócio;
- apresentar mensagens amigáveis;
- permitir nova tentativa quando apropriado;
- diferenciar erros de validação, conflito de estoque e indisponibilidade temporária.

---

## 13. Estratégia de Concorrência

O Inventory Service será a única autoridade responsável pela alteração do saldo.

A baixa de um produto deverá utilizar uma condição atômica equivalente a:

`saldo atual >= quantidade solicitada`

A atualização somente deverá ser confirmada caso a condição continue verdadeira no momento da escrita.

Quando uma nota possuir múltiplos produtos, a operação deverá evitar consumo parcial. Para isso, a implementação poderá utilizar transação do MongoDB no contexto do Inventory Service.

Essa estratégia impede que duas requisições concorrentes observem o mesmo saldo e concluam ambas de forma inválida.

---

## 14. Estratégia de Idempotência

O fluxo de fechamento utilizará uma chave idempotente estável para cada operação.

A chave poderá ser enviada pelo Billing Service ao Inventory Service por meio do cabeçalho:

`Idempotency-Key`

O Inventory Service manterá um registro das operações processadas.

Comportamento esperado:

- chave nova: processa a operação e registra o resultado;
- chave já concluída: não altera o estoque e retorna o resultado anterior;
- mesma chave com conteúdo incompatível: rejeita a requisição;
- timeout após processamento: a repetição continua segura.

A estratégia será especialmente importante para o cenário em que o Inventory Service conclua a baixa, mas a resposta seja perdida antes de o Billing Service atualizar o status da nota.

---

## 15. Tecnologias

| Camada | Tecnologia |
| --- | --- |
| Frontend | Angular |
| Programação reativa | RxJS |
| Componentes visuais | Design system próprio baseado no protótipo do Figma |
| Backend | C# |
| Framework backend | ASP.NET Core Web API |
| Persistência | MongoDB |
| Driver MongoDB | MongoDB.Driver |
| Comunicação HTTP | `HttpClient` / `IHttpClientFactory` |
| Resiliência | `Microsoft.Extensions.Http.Resilience` / Polly |
| Validação | FluentValidation ou validação nativa, conforme necessidade |
| Documentação API | OpenAPI / Swagger |
| Logs | Serilog |
| Testes backend | xUnit |
| Testes de integração | xUnit + ambiente MongoDB de teste |
| Containers | Docker / Docker Compose |
| CI | GitHub Actions |
| IA | Integração com API de LLM por interface desacoplada |
| Controle de versão | Git / GitHub |

---

## 16. Estratégia de Testes

### 16.1 Testes unitários

Os testes unitários deverão validar regras isoladas de negócio, especialmente:

- criação de produto com valores válidos e inválidos;
- impossibilidade de saldo negativo;
- criação de nota com status inicial correto;
- validação de quantidade de item;
- impossibilidade de fechar nota já fechada;
- comportamento esperado diante de estoque insuficiente.

### 16.2 Testes de integração

Os testes de integração deverão validar componentes cuja confiabilidade depende da persistência ou da comunicação entre camadas.

Cenários prioritários:

- persistência de produtos no MongoDB;
- persistência de notas;
- geração de numeração única;
- baixa de estoque;
- comportamento idempotente;
- operação concorrente sobre o último item disponível;
- respostas de erro da API.

### 16.3 Teste de falha entre serviços

Um cenário de demonstração deverá ser preparado:

1. manter Billing Service disponível;
2. interromper o Inventory Service;
3. tentar fechar uma nota;
4. verificar o feedback ao usuário e a permanência da nota como `Aberta`;
5. restaurar o Inventory Service;
6. repetir a operação;
7. verificar a conclusão correta do fechamento.

### 16.4 Casos críticos

- fechamento normal de nota;
- estoque insuficiente;
- Inventory Service indisponível;
- requisição duplicada;
- falha após baixa de estoque;
- duas notas concorrendo pelo mesmo produto;
- tentativa de fechar nota já fechada;
- indisponibilidade da funcionalidade de IA sem impacto sobre o fluxo principal.

---

## 17. Premissas

- O sistema representa um fluxo simplificado de faturamento e não uma NF-e oficial.
- O produto deve estar previamente cadastrado para ser utilizado em uma nota.
- O saldo é representado por quantidade inteira.
- O Inventory Service é a autoridade sobre o estoque.
- O Billing Service é a autoridade sobre as notas fiscais.
- A comunicação principal entre os microsserviços será HTTP.
- A aplicação será executada localmente por Docker Compose durante desenvolvimento e demonstração.
- A integração de IA é complementar e não participa de decisões transacionais.
- O ambiente de desenvolvimento poderá utilizar um MongoDB em replica set de nó único caso sejam necessárias transações multi-documento.

---

## 18. Decisões Técnicas

### DEC-01 — Separação entre Estoque e Faturamento

**Decisão:**  
Manter Produtos/Estoque e Notas/Faturamento em microsserviços independentes.

**Motivação:**  
Estoque e Faturamento possuem responsabilidades e regras de negócio diferentes. A separação reduz acoplamento e permite demonstrar comunicação e tratamento de falhas entre serviços.

**Alternativas consideradas:**  
Uma aplicação monolítica seria mais simples, porém não atenderia ao requisito arquitetural do projeto.

---

### DEC-02 — ASP.NET Core como backend

**Decisão:**  
Utilizar C# com ASP.NET Core Web API.

**Motivação:**  
A plataforma oferece suporte maduro para APIs HTTP, injeção de dependência, health checks, tratamento de erros, testes e integração com mecanismos de resiliência.

---

### DEC-03 — MongoDB como banco de dados

**Decisão:**  
Utilizar MongoDB como mecanismo de persistência.

**Motivação:**  
O modelo documental se adapta bem à representação de notas com seus itens e permite manter documentos coesos. O MongoDB também oferece operações atômicas e suporte a transações quando necessário.

**Restrição:**  
Os limites de propriedade dos dados entre microsserviços serão preservados. Um serviço não acessará diretamente as coleções do outro.

---

### DEC-04 — Consistência do fechamento

**Decisão:**  
Utilizar comunicação síncrona entre Billing e Inventory durante o fechamento, combinada com idempotência.

**Motivação:**  
Para o escopo do projeto, a abordagem mantém o fluxo fácil de compreender e demonstrar, sem introduzir infraestrutura adicional de mensageria.

A idempotência cobre o principal risco da comunicação síncrona: a possibilidade de o Estoque processar a operação enquanto a resposta é perdida.

**Alternativas consideradas:**  
Mensageria e arquiteturas orientadas a eventos foram consideradas como evolução futura, mas adicionariam complexidade operacional não necessária para o escopo atual.

---

### DEC-05 — Concorrência no serviço de Estoque

**Decisão:**  
Centralizar a alteração de saldo no Inventory Service e utilizar atualização condicional atômica, com transação quando a operação envolver múltiplos documentos.

**Motivação:**  
Essa abordagem impede saldo negativo e reduz condições de corrida sem transferir regras de estoque para o serviço de Faturamento.

---

### DEC-06 — Idempotência no fechamento

**Decisão:**  
Registrar cada operação de baixa por uma chave idempotente única.

**Motivação:**  
A solução permite retries seguros e protege o sistema contra cliques duplicados, timeouts e respostas perdidas entre os microsserviços.

---

### DEC-07 — Inteligência Artificial desacoplada

**Decisão:**  
Utilizar IA apenas como funcionalidade de apoio para geração de insights textuais.

**Motivação:**  
A funcionalidade demonstra integração com IA sem permitir que um serviço probabilístico participe de regras críticas de estoque ou faturamento.

Caso a integração esteja indisponível, o sistema continua plenamente funcional.

---

### DEC-08 — Design system próprio baseado no protótipo do Figma no frontend

**Decisão:**  
Utilizar Design system próprio baseado no protótipo do Figma como base dos componentes visuais, com customização de identidade própria.

**Motivação:**  
A biblioteca reduz o tempo gasto em componentes básicos, melhora consistência visual e acessibilidade e permite concentrar o esforço na experiência e nas regras do sistema.

---

## 19. Melhorias Futuras

Possíveis evoluções posteriores ao escopo inicial:

- comunicação assíncrona baseada em eventos;
- padrão Outbox para publicação confiável de eventos;
- autenticação e autorização;
- paginação e filtros avançados;
- auditoria de alterações;
- métricas e dashboards de observabilidade;
- cache para consultas de alta frequência;
- suporte a múltiplos depósitos;
- integração com sistemas externos de faturamento;
- testes end-to-end completos;
- implantação em ambiente de nuvem.

---

## 20. Conclusão

O Nexa Fiscal propõe uma solução compacta para cadastro de produtos, controle de estoque e emissão de notas, priorizando não apenas o funcionamento do fluxo principal, mas também sua confiabilidade.

A arquitetura separa Estoque e Faturamento em microsserviços independentes, utiliza persistência real em MongoDB e trata explicitamente situações relevantes para sistemas distribuídos, como indisponibilidade temporária, concorrência e repetição de requisições.

A implementação de idempotência, atualizações atômicas, observabilidade básica e tratamento de erros busca manter o comportamento do sistema previsível mesmo em cenários de falha. A funcionalidade de Inteligência Artificial permanece desacoplada das operações transacionais, atuando apenas como recurso complementar de apoio ao usuário.

O objetivo da solução é demonstrar uma base tecnicamente consistente, simples de compreender e compatível com o escopo proposto, mantendo espaço para evoluções arquiteturais futuras.
