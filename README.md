# Netryxa

A .NET 10 starter by Abhishek Humagain with CQRS, Keycloak authentication, PostgreSQL, validation, transactions, structured errors, and OpenTelemetry. Source is MIT licensed. Dependency licenses apply separately.

**Status:** fresh database starter with no application migration history, seed data, or inherited Playground/SRS/WBS/UML models. See [the architecture audit](docs/ARCHITECTURE-AUDIT.md) for remaining design work and [portal integration](docs/PORTAL.md) for deployment.

## Create a project

Requires Python 3.10+ to generate, .NET 10 SDK to develop, and Docker Compose for local dependencies.

```sh
python3 tools/generate_project.py MyProject --output artifacts/MyProject.zip
python3 -m zipfile -e artifacts/MyProject.zip ./generated
cd generated/MyProject
cp .env.example .env
docker compose up --build
```

The ZIP renames solution/project filenames, namespaces, project references, Docker entrypoints, Keycloak realm/client identifiers, and service configuration. Use 2–50 ASCII letters/digits, beginning with uppercase; reserved names are rejected. Local secrets, IDE metadata, build outputs, and unrecognized root files are excluded. Generated projects include the same build workflow and regression checks.

Open `http://localhost:5200/swagger` or `/scalar`, and Keycloak at `http://localhost:8080` (development admin: `admin` / `admin`). The API database is on host port 5433. Local Compose is a development stack with demonstration credentials and `start-dev` Keycloak; replace these for deployment.

For native API debugging, start dependencies with `docker compose up -d keycloak-db keycloak static-api-db plantuml-server`, then `dotnet run --project Static.API` from the solution root. Environment variables take precedence over `.env`. Do not commit `.env`.

## Create your database schema

The download contains no migrations, model snapshot, seeders, or sample records. PostgreSQL starts with an empty application database; identity/user and shared notification/settings models remain as reusable building blocks. Add or adapt your own domain models before creating the first migration.

From the solution root, after copying `.env.example` to `.env` and starting PostgreSQL:

```sh
dotnet new tool-manifest
dotnet tool install dotnet-ef --version 10.0.10
dotnet ef migrations add InitialCreate --project Static.Infrastructure --startup-project Static.API --output-dir Migrations
dotnet ef database update --project Static.Infrastructure --startup-project Static.API
```

Use your renamed project prefix in these paths after downloading. Commit the resulting migration and snapshot to your own repository. `/health/live` is available before database setup; `/health/ready` checks database connectivity, not whether your application schema has been applied. Existing database volumes are retained; removing source migrations does not alter an existing database.

## Extend the application

- `Static.Domain`: entities and domain events. Currently shares EF persistence base abstractions; see audit.
- `Static.Application`: commands, queries, handlers, validators, and application contracts.
- `Static.Infrastructure`: EF Core/PostgreSQL persistence, identity wiring, external adapters.
- `Static.API`: HTTP/gRPC entry points, authentication, error responses, OpenAPI.
- `Static.MediatR`: reusable pipeline behaviors.
- Supporting projects provide results, time, identity, query helpers, caching, and telemetry.

Add an entity, its EF configuration/DbSet, a command/query implementing the existing contracts, a handler, and a validator. Register assemblies through the existing extension methods. Use `INonTransactionalCommand` for remote operations that must not hold database transactions. Queries bypass the unit of work. Failed commands roll back unless their error explicitly opts into committing.

Pipeline order: tracing → request logging → unhandled exception logging → validation → performance → optional query caching → command unit of work. Generic automatic input sanitization is not registered: it can corrupt passwords/source code and does not replace contextual output encoding. Sanitize rich HTML explicitly at its boundary.

Caching is consistent between Debug and Release and uses in-process distributed-memory storage by default. Only queries implementing `ICacheable` **with an explicit key** are cached. Include user/tenant scope and every query input in that key, and design invalidation before using it. Register Redis-backed `IDistributedCache` when needed.

## Debugging

Errors use HTTP 400 for malformed models, 422 for validation, and 500 for unexpected failures, with a trace ID. Structured handler logs preserve exception stack traces. Validation attempted values, request payloads, cookies, headers, and claims are omitted from diagnostic responses. Detailed exceptions are available only in Development.

`/health/live` checks the process; `/health/ready` checks PostgreSQL; `/health` is the compatibility endpoint. Keycloak discovery/signing keys use a dedicated issuer and API audience. In Docker, metadata is fetched through the internal service address while the public issuer stays `http://localhost:8080/realms/static`.

Set `OpenTelemetry__CollectorEndpoint` to your collector to enable export. CQRS spans use source `Static.MediatR`. Slow requests use full elapsed milliseconds and omit payloads. EF sensitive-data logging is disabled unless explicitly enabled with `Debugging__EnableSensitiveDataLogging=true` outside Production.

The API never creates, migrates, or seeds the application database at startup. Create and apply your own initial migration as described below. For production, run reviewed migrations separately before rollout. Swagger is Development-only unless `Swagger__Enabled=true`. HTTPS redirection can be enabled with `HttpsRedirection__Enabled=true` when the hosting environment supplies an HTTPS endpoint; configure forwarded headers at your trusted reverse proxy.

The optional local Python runner is disabled. Configure an isolated execution backend before exposing code execution; setting `CodeExecution__EnableUnsafeLocalRunner=true` is only for trusted local experiments.

## Validation and CI

```sh
dotnet restore Static.sln --configfile nuget.config
dotnet build Static.sln -c Release --no-restore
dotnet run --project tests/Static.RegressionTests -c Release
python3 -m unittest discover -s tools/tests -v
```

GitHub Actions builds, runs behavioral/generator checks, generates, compiles, and smoke-tests `Acme`, validates Compose, and builds the Docker image. High/critical NuGet advisories fail restore. Dependabot checks packages, Actions, and Docker weekly. GitLab mirror CI no longer references missing identity/gateway deployments or private feeds.

To publish under [AbhishekHumagain/Netryxa](https://github.com/AbhishekHumagain/Netryxa), use the reviewed source checkout, enable Actions and private vulnerability reporting, and optionally enable GitHub's Template repository setting. Portal deployment requires the existing portfolio checkout and a same-origin reverse proxy to the standalone download service. See [deployment instructions](docs/PORTAL.md).
