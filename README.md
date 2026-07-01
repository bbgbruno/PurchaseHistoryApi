# PurchaseHistoryApi

API RESTful em C# (.NET 8) para gerenciamento de histórico de compras. Responsável por importar cupons fiscais brasileiros (NFC-e) nos formatos HTML e PDF, gerenciar produtos, categorias e fornecer dashboards de gastos.

## Funcionalidades

- **Autenticação** — Cadastro e autenticação de usuários com senhas hasheadas (BCrypt)
- **Importação de Cupons** — Upload e parsing de cupons fiscais NFC-e nos formatos HTML (HtmlAgilityPack) e PDF (UglyToad.PdfPig) com extração de loja, itens, valores e chave de acesso
- **Normalização de Produtos** — Sistema de regras para normalização de nomes de produtos: remoção de acentos, upper case, substituição de palavras-chave (TRAD/UND/KG) e mapeamento de textos (exato/contém)
- **Gerenciamento de Compras** — Listagem, visualização de itens, exclusão e atualização de data de compra
- **Itens de Compra** — Atribuição de categoria a produtos, aplicação de descontos com recálculo automático do total da compra
- **Categorias** — CRUD completo de categorias vinculadas ao usuário
- **Produtos** — Busca textual (ILIKE) e histórico de preços com estatísticas (menor/maior/média)
- **Dashboard** — Resumo de gastos mensais (atual vs anterior), detalhamento por categoria, evolução mensal em 12 meses e comparação de produtos entre períodos
- **Importação por Chave** — Registro e gerenciamento de fila de importação de cupons via chave de 44 dígitos

## Arquitetura

Clean Architecture com 4 camadas:

```
src/
├── PurchaseHistory.Domain/           # Entidades, DTOs, interfaces (sem dependências)
├── PurchaseHistory.Application/      # Use cases e regras de negócio
├── PurchaseHistory.Infrastructure/   # Repositórios (Dapper), parsers, serviços
├── PurchaseHistory.Api/              # Controllers REST, middlewares, DI
└── PurchaseHistory.Migrate/          # Migrations (FluentMigrator)
```

## Tecnologias

- **.NET 8** + ASP.NET Core
- **Dapper** + **Npgsql** (PostgreSQL)
- **FluentMigrator** para migrations
- **BCrypt.Net-Next** para hash de senhas
- **HtmlAgilityPack** para parsing de HTML
- **UglyToad.PdfPig** para parsing de PDF
- **Swashbuckle** (Swagger) para documentação
- **Serilog** para logging
- **FluentValidation** para validação

## Endpoints (31)

### Autenticação e Usuários
| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/auth/login` | Login |
| POST | `/api/auth/forgot-password` | Recuperação de senha |
| GET | `/api/users` | Lista usuários |
| GET | `/api/users/{id}` | Busca usuário |
| POST | `/api/users` | Cria usuário |
| PUT | `/api/users/{id}` | Atualiza usuário |
| DELETE | `/api/users/{id}` | Exclui usuário |

### Categorias
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/categories` | Lista categorias |
| GET | `/api/categories/{id}` | Busca categoria |
| POST | `/api/categories` | Cria categoria |
| PUT | `/api/categories/{id}` | Atualiza categoria |
| DELETE | `/api/categories/{id}` | Exclui categoria |

### Cupons
| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/cupons/upload-pdf` | Upload de cupom PDF |
| POST | `/api/cupons/upload-html` | Upload de cupom HTML |
| GET | `/api/cupons/imports/pending` | Importações pendentes |
| POST | `/api/cupons/imports` | Cria importação |
| DELETE | `/api/cupons/imports/{id}` | Exclui importação |
| PATCH | `/api/cupons/imports/{id}/status` | Atualiza status |

### Compras e Itens
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/purchases` | Lista compras |
| GET | `/api/purchases/{id}/items` | Itens da compra |
| DELETE | `/api/purchases/{id}` | Exclui compra |
| PATCH | `/api/purchases/{id}/purchase-date` | Atualiza data |
| PATCH | `/api/purchase-items/{id}/product-category` | Define categoria |
| PATCH | `/api/purchase-items/{id}/discount` | Aplica desconto |

### Produtos
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/products/search` | Busca produtos |
| GET | `/api/products/history/{productId}` | Histórico de preços |

### Normalização
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/normalization/names` | Lista mapeamentos |
| POST | `/api/normalization/names` | Cria mapeamento |
| DELETE | `/api/normalization/names/{id}` | Exclui mapeamento |
| POST | `/api/normalization/names/apply` | Aplica normalização |
| GET | `/api/normalization/keywords` | Lista substituições |
| POST | `/api/normalization/keywords` | Cria substituição |
| DELETE | `/api/normalization/keywords/{id}` | Exclui substituição |

### Dashboard
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/dashboard` | Resumo mensal |
| GET | `/api/dashboard/category/{id}/monthly` | Evolução 12 meses |
| GET | `/api/dashboard/category/{id}/products` | Produtos por período |

## Configuração

1. Configure a connection string do PostgreSQL em `appsettings.json` ou variável de ambiente `ConnectionStrings__DefaultConnection`
2. Execute as migrations com o projeto `PurchaseHistory.Migrate`
3. Execute `dotnet run` no projeto `PurchaseHistory.Api`

### Docker

```bash
docker build -t purchasehistory-api .
docker run -p 5299:5299 -e ConnectionStrings__DefaultConnection="..." purchasehistory-api
```

### Render.com

O deployment é feito via imagem Docker no Render. Configure a variável de ambiente `ConnectionStrings__DefaultConnection` com a string de conexão do PostgreSQL.
