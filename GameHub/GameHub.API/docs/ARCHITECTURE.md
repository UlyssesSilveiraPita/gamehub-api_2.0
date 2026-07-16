# 🏛️ Arquitetura — GameHub API 2.0

## Objetivo

Este documento descreve a evolução arquitetural do projeto GameHub API 2.0.

A aplicação será evoluída de forma incremental, evitando grandes refatorações e preservando a estabilidade do sistema.

---

# Arquitetura atual

Atualmente o projeto utiliza uma arquitetura baseada em camadas tradicionais do ASP.NET Core.

```text
GameHub.API
├── Controllers
├── Entities
├── Services
├── DTOs
├── Data
├── Configurations
└── Mappings