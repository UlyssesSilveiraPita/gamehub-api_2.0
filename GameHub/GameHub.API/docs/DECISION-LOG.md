# 📘 Architecture Decision Log (ADR)

## Objetivo

Este documento registra as principais decisões técnicas e arquiteturais tomadas durante o desenvolvimento do 
GameHub API 2.0.

Cada decisão contém:

- Contexto
- Decisão
- Justificativa
- Consequências

O objetivo é facilitar a manutenção do projeto e documentar a evolução da arquitetura.

---

# ADR-001

## Título

Evolução do GameHub API 1.0 para GameHub API 2.0

### Status

Aceita

### Data

Julho de 2026

### Contexto

O projeto GameHub API 1.0 já possuía uma base funcional contendo autenticação JWT, gerenciamento de jogadores, conquistas, 
save games e leaderboard.

A nova etapa da formação .NET exigia a criação de um novo projeto utilizando pagamentos, testes, arquitetura limpa, 
observabilidade e containerização.

### Decisão

Ao invés de iniciar um novo projeto do zero, decidiu-se evoluir o GameHub API existente para a versão 2.0.

### Justificativa

A evolução da aplicação aproxima o projeto de um cenário real encontrado no mercado, onde sistemas normalmente recebem 
novas funcionalidades ao longo do tempo.

Além disso, essa abordagem preserva o histórico de desenvolvimento e permite aplicar novos conceitos sobre uma base 
já consolidada.

### Consequências

O projeto continuará crescendo de forma incremental.

Novas funcionalidades deverão respeitar a arquitetura existente e serão refatoradas sempre que necessário para manter 
baixo acoplamento e alta coesão.

---

# ADR-002

## Título

Adaptação do domínio da Alura para o domínio GameHub

### Status

Aceita

### Data

Julho de 2026

### Contexto

O desafio original da formação .NET utiliza um domínio voltado para cursos, estudantes, matrículas e pagamentos.

O objetivo do GameHub API é representar uma plataforma de gerenciamento de jogos.

### Decisão

Os conceitos do desafio serão adaptados para o domínio do GameHub.

Enrollment será substituído por Purchase.

Student será substituído por User.

Payments continuarão existindo, porém vinculados às compras realizadas pelo usuário.

### Justificativa

A adaptação mantém o projeto coerente com o objetivo principal do GameHub e torna o portfólio mais alinhado à área de 
desenvolvimento de jogos.

### Consequências

Toda nova funcionalidade deverá utilizar a linguagem do domínio GameHub em vez da linguagem utilizada pelo curso.

---

# ADR-003

## Título

Frontend desenvolvido com Blazor

### Status

Aceita

### Data

Julho de 2026

### Contexto

O GameHub API inicialmente era apenas uma Web API.

Para transformar o projeto em uma aplicação completa seria necessário um frontend.

### Decisão

Utilizar Blazor como tecnologia oficial da interface do GameHub.

### Justificativa

Blazor utiliza C#, permitindo compartilhar conhecimentos, modelos e boas práticas com a Web API.

Além disso, demonstra domínio do ecossistema .NET completo.

### Consequências

O projeto passará a possuir uma interface moderna integrada à API utilizando autenticação JWT.

---

# ADR-004

## Título

Aplicação incremental da Clean Architecture

### Status

Aceita

### Data

Julho de 2026

### Contexto

O GameHub API 1.0 já possui uma estrutura funcional baseada em camadas tradicionais.

Migrar toda a solução para Clean Architecture em uma única etapa aumentaria o risco de erros e dificultaria o aprendizado.

### Decisão

Aplicar a Clean Architecture de forma incremental durante a evolução do projeto.

### Justificativa

Essa abordagem permite compreender cada alteração, manter a aplicação funcional e validar cada etapa antes da próxima 
refatoração.

### Consequências

A arquitetura será evoluída gradualmente, preservando estabilidade e facilitando testes.

---

## ADR-005 — Centralização do acesso ao usuário autenticado

### Status

Accepted

### Contexto

Os controllers estavam acessando diretamente as claims do usuário autenticado por meio de `HttpContext.User`.

Esse padrão gerava repetição de código e fazia os controllers conhecerem detalhes da implementação de autenticação, como `ClaimTypes` e a estrutura do token JWT.

### Decisão

Foi criada a abstração:

`ICurrentUser`

E sua implementação:

`CurrentUserService`

A implementação utiliza `IHttpContextAccessor` para obter os dados do usuário autenticado na requisição atual.

O serviço foi registrado com ciclo de vida `Scoped`, garantindo uma instância por requisição HTTP.

### Consequências

- Controllers deixam de acessar claims diretamente.
- A leitura do usuário autenticado fica centralizada.
- Mudanças futuras no mecanismo de autenticação ficam isoladas.
- O código fica mais simples de testar.
- Reduzimos duplicação e acoplamento com o ASP.NET Core.

---

# ADR-006

## Título

Introdução do Result Pattern para tratamento de falhas de negócio

### Status

Aceita

### Data

Julho de 2026

### Contexto

Os serviços da aplicação lançavam exceções para representar falhas previsíveis de negócio, como produto inexistente ou quantidade inválida.

Essa abordagem misturava erros inesperados com regras de negócio e fazia o middleware tratar situações que não eram exceções reais.

### Decisão

Foi introduzido um Result Pattern composto pelas classes:

- `Result`
- `Result<T>`
- `Error`

Os serviços passaram a retornar resultados explícitos de sucesso ou falha, substituindo exceções para cenários previsíveis.

### Justificativa

Essa abordagem torna o fluxo da aplicação mais explícito, facilita os testes unitários e reduz o uso de exceções para controle de fluxo.

Além disso, separa claramente falhas de negócio de erros inesperados da aplicação.

### Consequências

- Serviços retornam `Result<T>`.
- Controllers interpretam os resultados retornados pelos serviços.
- O middleware permanece responsável apenas por exceções inesperadas.
- A arquitetura torna-se mais previsível e preparada para testes.

---

# ADR-007

## Título

Criação de uma infraestrutura própria de validação

### Status

Aceita

### Data

Julho de 2026

### Contexto

Os controllers continham validações estruturais repetidas utilizando instruções `if`, enquanto os serviços também precisavam validar regras básicas de entrada.

Essa abordagem aumentava a duplicação de código e misturava responsabilidades.

### Decisão

Foi criada uma infraestrutura própria de validação composta por:

- `IValidator<T>`
- `ValidationErrors`
- Validators específicos por caso de uso

Todos os validators passaram a ser registrados através da extensão:

```csharp
builder.Services.AddValidation();
```

### Justificativa

A infraestrutura permite separar validação estrutural das regras de negócio, reduz duplicação e mantém os serviços focados apenas na lógica da aplicação.

Também prepara a arquitetura para crescimento futuro sem depender inicialmente de bibliotecas externas.

### Consequências

- Controllers executam validações antes dos serviços.
- Serviços continuam protegendo as regras de negócio.
- Validações tornam-se reutilizáveis.
- Novos módulos seguem o mesmo padrão arquitetural.

---

# ADR-008

## Título

Padronização das respostas HTTP da API

### Status

Aceita

### Data

Julho de 2026

### Contexto

Os controllers retornavam objetos anônimos para representar erros de validação e regras de negócio.

Isso dificultava a padronização das respostas e aumentava a repetição de código.

### Decisão

Foram criados os DTOs:

- `ValidationErrorResponse`
- `ApiErrorResponse`

Também foi criada a classe:

- `ControllerExtensions`

responsável por centralizar respostas HTTP reutilizáveis, como:

- `ValidationFailed()`
- `BadRequestError()`
- `NotFoundError()`

### Justificativa

Centralizar a criação das respostas reduz duplicação, melhora a documentação do Swagger e facilita futuras alterações no formato das respostas.

### Consequências

- Todos os controllers podem reutilizar o mesmo padrão de resposta.
- O formato das respostas torna-se consistente em toda a API.
- Mudanças futuras serão realizadas em apenas um local.

---

# ADR-009

## Título

Centralização do mapeamento entre entidades e DTOs

### Status

Aceita

### Data

Julho de 2026

### Contexto

O mapeamento entre entidades e DTOs estava sendo repetido em diversos endpoints, aumentando o tamanho dos controllers e dificultando futuras alterações.

### Decisão

Foi criada a classe:

- `PurchaseMappings`

utilizando Extension Methods para converter entidades em DTOs.

Exemplo:

```csharp
purchase.ToResponse();
```

### Justificativa

Centralizar os mapeamentos reduz duplicação, melhora a legibilidade dos controllers e facilita futuras alterações nos contratos da API.

### Consequências

- Controllers deixam de conter lógica de mapeamento.
- O código torna-se mais limpo e reutilizável.
- Alterações em DTOs passam a ser realizadas em apenas um local.
- O padrão poderá ser reutilizado para Players, Games, Achievements, SaveGames e demais módulos da aplicação.