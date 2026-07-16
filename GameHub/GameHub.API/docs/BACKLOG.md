# 🎮 GameHub API 2.0

## Sobre o projeto

Uma plataforma completa para gerenciamento de jogadores, jogos, conquistas, saves, rankings e compras digitais, desenvolvida
com ASP.NET Core Web API e Blazor, seguindo boas práticas modernas de arquitetura e desenvolvimento.

---

# Objetivos

- Evoluir o GameHub API 1.0
- Aplicar o conteúdo estudado na Formação .NET
- Implementar Clean Architecture
- Aplicar DDD
- Aplicar SOLID
- Criar testes automatizados
- Criar frontend em Blazor
- Containerizar a aplicação
- Implementar CI/CD
- Construir um projeto digno de portfólio profissional

---

# Roadmap

- [x] Base da API
- [ ] Documentação
- [ ] Arquitetura
- [ ] Pagamentos
- [ ] Testes
- [ ] Blazor
- [ ] Docker
- [ ] Pipeline
- [ ] Deploy
- [ ] Release final

---

## Epic 1 — Planejamento e documentação

### Objetivo

Preparar a evolução do GameHub 2.0 antes das mudanças estruturais e funcionais.

### Entregas

- [x] Criar o repositório GameHub API 2.0
- [x] Validar build e execução da API
- [x] Confirmar funcionamento do Swagger
- [x] Criar pasta de documentação
- [ ] Criar matriz de permissões
- [ ] Criar registro de decisões técnicas
- [ ] Atualizar o README da versão 2.0
- [ ] Definir o domínio de compras e pagamentos

### Critério de conclusão

A documentação inicial deve explicar o objetivo do sistema, as permissões, as decisões técnicas e a ordem de evolução do 
projeto.

---

## Epic 2 — Compras e pagamentos

### Objetivo

Criar um módulo de compras digitais e pagamentos integrado ao domínio do GameHub.

### Entregas

- [ ] Modelar produtos digitais
- [ ] Modelar compras
- [ ] Modelar pagamentos
- [ ] Implementar estados de pagamento
- [ ] Implementar intenção de pagamento
- [ ] Implementar confirmação de pagamento
- [ ] Implementar idempotência
- [ ] Impedir pagamentos ativos duplicados
- [ ] Criar gateway de pagamento simulado
- [ ] Criar endpoints de consulta
- [ ] Aplicar autorização por usuário e administrador
- [ ] Documentar endpoints no Swagger

### Critério de conclusão

Um usuário autenticado deve conseguir iniciar e consultar o pagamento de uma compra própria, enquanto o administrador 
deve conseguir auditar os pagamentos.

---

## Epic 3 — Arquitetura

### Objetivo

Evoluir o projeto para uma arquitetura organizada, testável e com baixo acoplamento.

### Entregas

- [ ] Definir as camadas da solução
- [ ] Criar projeto Domain
- [ ] Criar projeto Application
- [ ] Criar projeto Infrastructure
- [ ] Manter a API como camada de apresentação
- [ ] Separar regras de negócio dos controllers
- [ ] Criar interfaces de repositórios
- [ ] Criar interfaces de gateways
- [ ] Aplicar injeção de dependência
- [ ] Aplicar DTOs de entrada e saída
- [ ] Centralizar mapeamentos
- [ ] Padronizar tratamento de erros

### Critério de conclusão

As regras de negócio devem permanecer independentes de banco de dados, controllers e serviços externos.

---

## Epic 4 — Testes

### Objetivo

Garantir segurança para evoluir o projeto sem quebrar comportamentos existentes.

### Entregas

- [ ] Criar projeto de testes unitários
- [ ] Testar regras de pagamento
- [ ] Testar transições de estado
- [ ] Testar valores inválidos
- [ ] Testar idempotência
- [ ] Testar duplicidade de pagamento
- [ ] Criar projeto de testes de integração
- [ ] Testar autenticação
- [ ] Testar autorização
- [ ] Testar endpoints principais
- [ ] Testar persistência no banco

### Critério de conclusão

As principais regras de negócio e os fluxos críticos da API devem possuir testes automatizados.

---

## Epic 5 — Observabilidade

### Objetivo

Facilitar o diagnóstico e o acompanhamento da aplicação.

### Entregas

- [ ] Adicionar logging estruturado
- [ ] Adicionar correlation ID
- [ ] Registrar eventos importantes de pagamento
- [ ] Evitar dados sensíveis nos logs
- [ ] Adicionar health check da API
- [ ] Adicionar health check do banco
- [ ] Documentar logs e health checks

---

## Epic 6 — Frontend Blazor

### Objetivo

Criar uma interface moderna para gerenciamento das funcionalidades do GameHub.

### Entregas

- [ ] Criar projeto Blazor
- [ ] Definir identidade visual
- [ ] Criar tela de login
- [ ] Criar dashboard
- [ ] Criar tela de jogadores
- [ ] Criar tela de conquistas
- [ ] Criar tela de save games
- [ ] Criar tela de leaderboard
- [ ] Criar tela de compras
- [ ] Criar tela de pagamentos
- [ ] Criar área administrativa
- [ ] Integrar autenticação JWT

---

## Epic 7 — Containerização

### Objetivo

Padronizar a execução da aplicação em diferentes ambientes.

### Entregas

- [ ] Criar Dockerfile
- [ ] Utilizar build multi-stage
- [ ] Criar docker-compose
- [ ] Configurar API e banco
- [ ] Criar arquivo `.env.example`
- [ ] Adicionar health checks aos containers
- [ ] Documentar execução com Docker

---

## Epic 8 — Pipeline CI/CD

### Objetivo

Automatizar a validação e a entrega do projeto.

### Entregas

- [ ] Criar pipeline para pull requests
- [ ] Executar restore
- [ ] Executar build
- [ ] Executar testes
- [ ] Publicar artefatos
- [ ] Gerar imagem Docker
- [ ] Executar smoke test
- [ ] Adicionar badge ao README

---

## Epic 9 — Deploy e release

### Objetivo

Disponibilizar uma versão pública e documentada do GameHub 2.0.

### Entregas

- [ ] Definir ambiente de hospedagem
- [ ] Publicar API
- [ ] Publicar frontend
- [ ] Configurar banco de produção
- [ ] Criar changelog
- [ ] Criar tag da versão
- [ ] Criar release no GitHub
- [ ] Registrar imagens e demonstrações no README

---

# Status atual

**Fase atual:** planejamento e documentação.

**Último marco:** base do GameHub API 2.0 validada com build e Swagger funcionando.

**Próximo passo:** criar a matriz de permissões do sistema.

...