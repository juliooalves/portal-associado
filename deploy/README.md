# Deploy — Oracle Cloud

Stack de produção em uma VM (OCI free tier): app em container (GHCR) + PostgreSQL + Caddy como reverse proxy com HTTPS automático (Let's Encrypt). CD via GitHub Actions: CI verde na `main` → build da imagem → push para `ghcr.io` → SSH na VM → `docker compose pull && up -d`.

```
Internet ──443──> Caddy (TLS) ──8080──> app (Kestrel, HTTP) ──5432──> postgres
```

O app roda atrás do proxy: o Caddy termina o TLS e repassa `X-Forwarded-Proto`, que o `UseForwardedHeaders` do app consome — assim o cookie `Secure` e o HSTS funcionam normalmente.

## 1. Pré-requisitos na VM

```bash
# Docker (Ubuntu)
curl -fsSL https://get.docker.com | sudo sh
sudo usermod -aG docker $USER   # relogar depois
```

## 2. Rede na OCI

Duas camadas de firewall — as duas precisam liberar 80/443:

1. **Security List da subnet** (console OCI): ingress rules TCP 80 e 443, source `0.0.0.0/0`.
2. **iptables da VM** — as imagens Ubuntu da Oracle vêm com regra `REJECT` que bloqueia tudo além de SSH:

```bash
sudo iptables -I INPUT 6 -m state --state NEW -p tcp --dport 80 -j ACCEPT
sudo iptables -I INPUT 6 -m state --state NEW -p tcp --dport 443 -j ACCEPT
sudo netfilter-persistent save
```

## 3. DNS (DuckDNS)

1. Login em https://www.duckdns.org, criar subdomínio (ex.: `portal-associado`).
2. Apontar para o IP público da VM.
3. IP da VM na OCI é reservado por padrão (ephemeral → reservar se necessário); sem IP fixo, agendar o script de update do DuckDNS via cron.

## 4. Arquivos na VM

```bash
mkdir -p ~/portal-associado && cd ~/portal-associado
# copiar docker-compose.prod.yml e Caddyfile desta pasta (scp ou curl do raw do GitHub)
cp .env.example .env   # e editar: DOMAIN=<subdominio>.duckdns.org, POSTGRES_PASSWORD forte
```

## 5. Imagem no GHCR

O workflow publica em `ghcr.io/juliooalves/portal-associado`. Após o primeiro push, tornar o package **público** (Settings do package → Danger Zone → Change visibility) — a VM faz `docker pull` sem autenticação. Alternativa: manter privado e `docker login ghcr.io` na VM com um PAT `read:packages`.

## 6. Secrets no GitHub (Settings → Secrets and variables → Actions)

| Secret | Valor |
|---|---|
| `DEPLOY_HOST` | IP público (ou domínio) da VM |
| `DEPLOY_USER` | usuário SSH (ex.: `ubuntu`) |
| `DEPLOY_SSH_KEY` | chave privada SSH (par dedicado ao deploy; a pública em `~/.ssh/authorized_keys` da VM) |

## 7. Primeiro deploy

Push na `main` (CI verde dispara o Deploy) ou rodar o workflow **Deploy** manualmente (`workflow_dispatch`). Depois:

```bash
docker compose -f docker-compose.prod.yml logs -f app   # na VM, ver migração + seed
```

Acessar `https://<subdominio>.duckdns.org` — credenciais de teste na tabela do README raiz.

## Decisões

- **Caddy** em vez de nginx + certbot: certificado Let's Encrypt automático (emissão + renovação) com config de 3 linhas.
- **Migração + seed no startup** em produção via `Database__MigrateOnStartup`/`Database__SeedOnStartup` — adequado para instância única de demonstração; em produção real seria etapa separada do pipeline.
- **Postgres sem porta exposta no host** — só a rede interna do compose alcança o banco.
- **Imagem multi-arch** (amd64 + arm64): cobre tanto shapes AMD quanto Ampere A1 do free tier.
