# JwtAuthStudy — Autenticação JWT com ASP.NET Core MVC

Projeto de estudo sobre autenticação e autorização baseada em tokens JWT, implementado com **ASP.NET Core MVC** (.NET 8), **Views Razor**, **EF Core** e **RBAC**.

Documentação complementar: [`DEMONSTRACAO.md`](DEMONSTRACAO.md) (guia do código para apresentação)

## Objetivo

Demonstrar o fluxo completo de autenticação JWT no navegador:

1. **Login** — formulário MVC valida credenciais e grava JWT em cookies HttpOnly.
2. **Área protegida** — páginas com `[Authorize]` exigem token válido.
3. **RBAC** — painel Admin exige role `Admin`.
4. **Refresh token** — renovação automática de sessão via middleware.
5. **Persistência** — usuários e refresh tokens no SQL Server via EF Core.

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server LocalDB (incluído no Visual Studio) **ou** SQL Server via Docker

## Estrutura do projeto (MVC)

```
basicWebSystem/
├── JwtAuthStudy.sln
├── src/JwtAuthStudy/
│   ├── Controllers/       # Home, Account, Protegido, Admin
│   ├── Models/          # Entities, ViewModels, Settings
│   ├── Views/           # Razor (.cshtml)
│   ├── Services/        # AuthService, JwtTokenGenerator, Repositories
│   ├── Data/            # AppDbContext, DatabaseSeeder
│   ├── Middleware/      # JwtRefreshMiddleware
│   └── Program.cs
└── README.md
```

## Como executar

### 1. Restaurar dependências

```bash
dotnet restore
```

### 2. (Opcional) Configurar chave JWT via User Secrets

```bash
cd src/JwtAuthStudy
dotnet user-secrets set "Jwt:Key" "sua-chave-secreta-com-pelo-menos-32-caracteres!!"
```

A chave deve ter **no mínimo 32 caracteres** (256 bits) para o algoritmo HS256.

### 3. Iniciar a aplicação

```bash
dotnet run --project src/JwtAuthStudy
```

Abra no navegador:
- `https://localhost:7xxx` ou `http://localhost:5xxx` (portas em `launchSettings.json`)

O banco é criado automaticamente na primeira execução (`EnsureCreated`), com usuários de teste inseridos.

## Usuários de teste

| Usuário | Senha | Role |
|---------|-------|------|
| `admin` | `admin123` | Admin |
| `user` | `user123` | User |

## Rotas MVC

| Rota | Controller | Auth | Descrição |
|------|------------|------|-----------|
| `/` | `Account/Login` | Público | Tela de login (rota padrão) |
| `/Account/Login` | `Account/Login` | Público | Formulário de login |
| `/Account/Logout` | `Account/Logout` POST | Autenticado | Encerra sessão |
| `/Protegido` | `Protegido/Index` | `[Authorize]` | Área autenticada |
| `/Admin` | `Admin/Index` | `[Authorize(Roles="Admin")]` | Painel administrativo |

## Fluxo de teste no navegador

1. Acesse `/` — abre direto a tela de login
2. Entre com `admin` / `admin123` → redireciona para `/Protegido`
3. Acesse `/Admin` → painel admin visível
4. Faça logout, entre com `user` / `user123`
5. Acesse `/Protegido` → OK; `/Admin` → 403 Forbidden
6. Clique em **Sair** → cookies removidos, `/Protegido` redireciona para login

## Autenticação JWT com cookies

| Cookie | Conteúdo |
|--------|----------|
| `access_token` | JWT de curta duração (15 min) |
| `refresh_token` | Token opaco para renovar sessão (7 dias) |

O middleware `JwtBearer` lê o JWT do cookie ou do header `Authorization: Bearer`. O `JwtRefreshMiddleware` renova tokens expirados automaticamente (cookies).

## Swagger (testes via API)

Com a aplicação rodando em **Development**, acesse:

```
https://localhost:7xxx/swagger
```

### Fluxo no Swagger UI

1. **POST `/api/Auth/login`** com body:
   ```json
   { "usuario": "admin", "senha": "admin123" }
   ```
2. Copie o `accessToken` da resposta.
3. Clique em **Authorize** (cadeado) e informe: `Bearer SEU_ACCESS_TOKEN`
4. Teste **GET `/api/protegido`** e **GET `/api/admin`**
5. **POST `/api/Auth/refresh`** com `{ "refreshToken": "..." }`
6. **POST `/api/Auth/logout`** com `{ "refreshToken": "..." }` (requer Authorize)

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| POST | `/api/Auth/login` | — | Login JSON |
| POST | `/api/Auth/refresh` | — | Renovar tokens |
| POST | `/api/Auth/logout` | Bearer | Revogar refresh token |
| GET | `/api/protegido` | Bearer | Área autenticada |
| GET | `/api/admin` | Bearer + Admin | Painel admin |

## SQL Server via Docker (alternativa)

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Your_password123" \
  -p 1433:1433 --name sql-jwt -d mcr.microsoft.com/mssql/server:2022-latest
```

Atualize `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=JwtAuthStudy;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
}
```
