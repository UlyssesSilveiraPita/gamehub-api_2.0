# 🔐 Matriz de Permissões — GameHub API 2.0

## Objetivo

Este documento define quais funcionalidades podem ser acessadas por usuários públicos, usuários autenticados e 
administradores.

A matriz servirá como referência para:

- configuração de autenticação e autorização;
- criação de roles e policies;
- proteção dos endpoints;
- implementação das regras de propriedade dos dados;
- criação dos testes de autorização.

---

# Perfis de acesso

## Public

Representa qualquer pessoa que ainda não esteja autenticada no sistema.

Pode acessar apenas funcionalidades públicas, como registro, login, documentação pública e consulta ao leaderboard.

## User

Representa um usuário autenticado comum.

Pode criar, consultar e gerenciar apenas os próprios dados, como jogadores, saves, conquistas, compras e pagamentos.

## Admin

Representa um administrador do sistema.

Pode consultar e administrar dados de todos os usuários, além de executar operações de auditoria e manutenção.

---

# Regras gerais

- O usuário comum só pode acessar dados vinculados ao próprio `UserId`.
- O administrador pode consultar dados de qualquer usuário.
- Endpoints protegidos devem exigir token JWT válido.
- Operações administrativas devem exigir a role `Admin`.
- A aplicação não deve confiar apenas em IDs enviados pelo cliente.
- A propriedade do recurso deve ser validada utilizando o `UserId` presente no token.
- Dados sensíveis nunca devem ser retornados nas respostas da API.
- Um usuário autenticado não se torna administrador apenas por enviar uma role na requisição.

---

# Matriz de permissões

| Funcionalidade | Public | User | Admin |
|---|:---:|:---:|:---:|
| Registrar conta | ✅ | ✅ | ✅ |
| Fazer login | ✅ | ✅ | ✅ |
| Acessar Swagger | ✅ | ✅ | ✅ |
| Autorizar no Swagger com JWT | ❌ | ✅ | ✅ |
| Consultar leaderboard | ✅ | ✅ | ✅ |
| Consultar lista pública de jogos | ✅ | ✅ | ✅ |
| Consultar detalhes de um jogo | ✅ | ✅ | ✅ |
| Consultar próprio perfil | ❌ | ✅ | ✅ |
| Atualizar próprio perfil | ❌ | ✅ | ✅ |
| Consultar perfil de outro usuário | ❌ | ❌ | ✅ |
| Criar jogador próprio | ❌ | ✅ | ✅ |
| Consultar jogadores próprios | ❌ | ✅ | ✅ |
| Atualizar jogador próprio | ❌ | ✅ | ✅ |
| Excluir jogador próprio | ❌ | ✅ | ✅ |
| Consultar jogador de outro usuário | ❌ | ❌ | ✅ |
| Criar save game próprio | ❌ | ✅ | ✅ |
| Consultar saves próprios | ❌ | ✅ | ✅ |
| Atualizar save próprio | ❌ | ✅ | ✅ |
| Excluir save próprio | ❌ | ✅ | ✅ |
| Consultar save de outro usuário | ❌ | ❌ | ✅ |
| Consultar conquistas disponíveis | ✅ | ✅ | ✅ |
| Desbloquear conquista própria | ❌ | ✅ | ✅ |
| Consultar conquistas próprias | ❌ | ✅ | ✅ |
| Consultar conquistas de outro usuário | ❌ | ❌ | ✅ |
| Criar produto digital | ❌ | ❌ | ✅ |
| Atualizar produto digital | ❌ | ❌ | ✅ |
| Excluir produto digital | ❌ | ❌ | ✅ |
| Criar compra própria | ❌ | ✅ | ✅ |
| Consultar compras próprias | ❌ | ✅ | ✅ |
| Consultar compra de outro usuário | ❌ | ❌ | ✅ |
| Criar intenção de pagamento | ❌ | ✅ | ✅ |
| Confirmar pagamento próprio | ❌ | ✅ | ✅ |
| Consultar pagamentos próprios | ❌ | ✅ | ✅ |
| Consultar pagamento de outro usuário | ❌ | ❌ | ✅ |
| Listar todos os pagamentos | ❌ | ❌ | ✅ |
| Estornar pagamento | ❌ | ❌ | ✅ |
| Consultar logs administrativos | ❌ | ❌ | ✅ |
| Consultar health check público | ✅ | ✅ | ✅ |
| Consultar health check detalhado | ❌ | ❌ | ✅ |

---

# Regras de propriedade

## Players

Um usuário comum só pode acessar jogadores vinculados ao próprio `UserId`.

Exemplo:

```text
Token UserId: 10
Player UserId: 10
Resultado: acesso permitido

--->

Token UserId: 10
Player UserId: 25
Resultado: acesso negado