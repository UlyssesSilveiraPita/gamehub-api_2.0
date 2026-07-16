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

