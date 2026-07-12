# portal-associado

[![CI](https://github.com/juliooalves/portal-associado/actions/workflows/ci.yml/badge.svg)](https://github.com/juliooalves/portal-associado/actions/workflows/ci.yml)

Sistema de Atualização Cadastral — desafio técnico PROAUTO.

Portal de autoatendimento do associado: autentica com **CPF + placa**, visualiza seus dados (nome, CPF, placa, telefone e endereço) e pode atualizar **apenas o endereço**. API RESTful como base, com views MVC (Razor) por cima, no mesmo host.

## Stack

| Camada | Escolha |
|---|---|
| Runtime | .NET 8 (LTS) |
| Web | ASP.NET Core MVC + Razor puro (sem framework JS) |
| Banco | PostgreSQL 16 via docker-compose |
| ORM | EF Core + Npgsql (migrations, owned entities) |
| Testes unitários | xUnit + NSubstitute |
| Testes de integração | xUnit + WebApplicationFactory + Testcontainers |
| CI | GitHub Actions (restore → build → test) |

## Como rodar

**Pré-requisitos:** [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0), [Docker](https://docs.docker.com/get-docker/) e `make`.

Na primeira vez:

```bash
make start TRUST=1
```

Nas seguintes:

```bash
make start
```

Um comando só: sobe o PostgreSQL e roda a aplicação (migrations e seed aplicados automaticamente no startup). Acesse **https://localhost:7002** e entre com uma das [credenciais de teste](#credenciais-de-teste-seed) abaixo.

- `TRUST=1` roda `dotnet dev-certs https --trust` antes de subir — o cookie de sessão é `Secure`, o login **só funciona via HTTPS**, e sem confiar no certificado de desenvolvimento o navegador bloqueia a página. Só é necessário uma vez por máquina.
- Se a porta 5432 já estiver em uso, o `make start` detecta a falha, cria um `.env` com `POSTGRES_PORT=5434` e sobe container e aplicação na porta nova — sem intervenção manual.

<details>
<summary><b>Sem make?</b></summary>

```bash
dotnet dev-certs https --trust
docker compose up -d
dotnet run --project src/ProAuto.PortalAssociado.Web --launch-profile https
```

O `dev-certs https --trust` gera o certificado HTTPS de desenvolvimento e o adiciona às autoridades confiáveis da máquina (primeira vez apenas). É necessário porque o cookie de sessão é `Secure` e só trafega via HTTPS.

Porta 5432 ocupada? Crie um `.env` na raiz com `POSTGRES_PORT=5434` e rode a aplicação com a connection string ajustada:

```bash
docker compose up -d
dotnet run --project src/ProAuto.PortalAssociado.Web --launch-profile https -- \
  ConnectionStrings:Default="Host=localhost;Port=5434;Database=portal_associado;Username=portal;Password=portal"
```

</details>

### Credenciais de teste (seed)

| Nome | CPF | Placa |
|---|---|---|
| Maria Oliveira Santos | 111.444.777-35 | ABC1234 |
| João Pedro Ferreira | 529.982.247-25 | BRA2E19 |
| Ana Carolina Lima | 390.533.447-05 | XYZ9A87 |

CPFs fictícios com dígitos verificadores válidos. Placas cobrem o formato antigo (`ABC1234`) e Mercosul (`BRA2E19`).

## Testes

```bash
dotnet test
```

- **Unitários** (60): value objects (`Cpf` com dígito verificador, `Placa` antiga + Mercosul, `Endereco`), service com repository mockado (autenticação, atualização de endereço) e controllers de API.
- **Integração** (10): Testcontainers sobe um PostgreSQL real e efêmero; cobre o fluxo completo (login → GET /me → PUT endereço), casos negativos (credencial errada → 401 genérico, sem auth → 401/redirect, payload inválido → 400), campos extras no payload ignorados e isolamento entre associados (anti-IDOR).

Os testes de integração exigem Docker em execução.

## API

| Método | Rota | Comportamento |
|---|---|---|
| POST | `/api/auth/login` | CPF + placa → cookie de sessão. Falha → 401 genérico ("CPF ou placa inválidos.") |
| POST | `/api/auth/logout` | Encerra a sessão |
| GET | `/api/associados/me` | Dados completos do associado autenticado |
| PUT | `/api/associados/me/endereco` | Atualiza somente o endereço; retorna os dados atualizados |

Views: `/login` e `/meus-dados` (form POST clássico com anti-forgery e validação server-side).

## Decisões de arquitetura

- **2 projetos + 2 de teste, camadas por pasta** (`Controllers/`, `Services/`, `Repositories/`, `Domain/`, `Contracts/`). Clean Architecture completa (4 assemblies) seria over-engineering para um domínio de uma entidade e duas operações — a testabilidade vem das interfaces (`IAssociadoService`, `IAssociadoRepository`), não da quantidade de projetos.
- **"Só o endereço é editável" garantido por construção**: `IAssociadoService.AtualizarEnderecoAsync(associadoId, novoEndereco)` recebe somente endereço; o DTO do PUT aceita apenas campos de endereço e qualquer campo extra no payload é ignorado no servidor — a regra nunca depende da UI.
- **Cookie auth, não JWT**: cookie `HttpOnly` + `Secure` + `SameSite=Strict`. JWT em `localStorage` fica exposto a roubo via XSS; cookie `HttpOnly` remove essa superfície. JWT se justificaria com backend stateless para clientes desacoplados (app mobile, SPA em outro domínio) — não é o caso: views e API no mesmo host.
- **401 genérico no login**: a resposta nunca revela se o CPF existe na base (anti-enumeração — placa é credencial semi-pública). Input malformado falha com a mesma mensagem, sem sequer consultar o banco.
- **Anti-IDOR**: o ID do associado em `GET /me` e `PUT /me/endereco` vem sempre da claim do cookie, nunca de parâmetro de URL — um associado não consegue ler/editar dados de outro.
- **Sem sign-up**: o associado pré-existe (entrou na base ao contratar a proteção veicular). O sistema é de *atualização* cadastral; cadastro aberto seria erro de modelagem. Daí o seed com credenciais documentadas acima.
- **Sem roles**: existe um único tipo de ator (o associado). A estrutura de claims permite adicionar um papel administrativo no futuro sem redesenho.
- **Value objects com validação real**: `Cpf` valida dígito verificador (não só regex); `Placa` aceita formatos antigo e Mercosul; `Endereco` é owned entity (`OwnsOne`) — sem tabela própria.
- Código em inglês, termos de domínio em português (`Associado`, `Placa`, `Endereco`).
- `Nullable` habilitado + `TreatWarningsAsErrors` em todos os projetos.

## Fora de escopo (consciente)

- **CNPJ**: o portal de produção aceita PJ; a modelagem por value objects permite adicionar `Cnpj` sem refatorar.
- **Senha + recuperação**: o portal real evoluiu de placa para senha; fora do escopo do enunciado.
- **Rate limiting no login**: necessário em produção contra enumeração de CPF.
