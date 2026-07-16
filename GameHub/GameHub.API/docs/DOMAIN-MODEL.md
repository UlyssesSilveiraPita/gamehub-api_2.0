# 🎮 Modelo de Domínio — GameHub API 2.0

## Objetivo

Este documento descreve as principais entidades, relacionamentos e regras de negócio do GameHub API 2.0.

Ele servirá como referência para:

- criação das entidades;
- configuração do Entity Framework Core;
- criação das migrations;
- implementação dos serviços;
- criação dos endpoints;
- desenvolvimento dos testes;
- integração com o frontend Blazor.

---

# Escopo da versão 2.0

A versão 2.0 terá como foco:

- contas de usuários;
- jogadores;
- save games;
- conquistas;
- rankings;
- produtos digitais;
- compras;
- pagamentos.

Funcionalidades como avaliações, lista de desejos, sistema de amigos e notificações ficarão para versões futuras.

---

# Visão geral do domínio

```text
ApplicationUser
│
├── Players
│   ├── SaveGames
│   ├── PlayerAchievements
│   └── LeaderboardEntries
│
└── Purchases
    ├── PurchaseItems
    └── Payments

Game
│
├── Achievements
├── GameProducts
└── LeaderboardEntries