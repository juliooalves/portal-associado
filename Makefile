-include .env
POSTGRES_PORT ?= 5432

start:
ifdef TRUST
	dotnet dev-certs https --trust
endif
	docker compose up -d || (echo POSTGRES_PORT=5434> .env && docker compose up -d)
	$(MAKE) run

run:
	dotnet run --project src/ProAuto.PortalAssociado.Web --launch-profile https -- ConnectionStrings:Default="Host=localhost;Port=$(POSTGRES_PORT);Database=portal_associado;Username=portal;Password=portal"

.PHONY: start run
