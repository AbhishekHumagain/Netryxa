# Architecture audit

## Fixed in this pass

- Error transport status codes now reflect failure; trace IDs correlate errors with logs.
- Error diagnostics no longer dump credentials, query strings, claims, or validation attempted values; stack details are Development-only.
- Performance timing uses total milliseconds; request payload logging is removed.
- CQRS spans use a shared ActivitySource registered with OpenTelemetry; cancellation propagates and is not logged as an unexpected failure.
- Cache behavior no longer changes with the build configuration. A safe memory implementation is registered, and keys must explicitly include caller/tenant/query scope.
- Automatic anti-XSS mutation is removed from the default pipeline. The previous response generic constraint meant it did not apply to normal commands anyway. Rich HTML sanitization belongs at explicit boundaries.
- Private NuGet feeds are removed. GitHub CI covers build, regression checks, rename/build, Compose, and Docker. GitLab mirror no longer triggers nonexistent deployments.
- Example configuration is recreated from demonstration values. SDK/config files are no longer accidentally gitignored.
- Keycloak validates the API audience rather than accepting any token for the shared `account` audience; realm import includes the API audience mapper and explicit local redirects.
- Docker issuer/discovery addresses are separated. Swagger and sensitive EF logs require explicit configuration. Application database migrations are now exclusively developer-managed.
- The local subprocess code runner is disabled by default.
- MIT licensing, contribution/security instructions, download generation, and portal integration are supplied.

## Fresh database baseline

No seeders, seed data, migration files, or model snapshot are distributed. Startup no longer creates or migrates the application database. Consumers add their domain models and generate their own initial migration; the README documents the commands. Existing database volumes are not modified by this source cleanup.

## Remaining work before calling this a minimal Clean Architecture framework

1. **Domain boundary:** Domain references EF Core base contracts and helpers with infrastructure dependencies. Move entity/audit contracts into Domain (or a dependency-free primitives project) and make persistence implement those contracts. This requires a coordinated API/migration refactor; it is not safe to claim strict dependency independence yet.
2. **Remaining legacy adapters:** Playground, UML, SRS, WBS models/mappings, all inherited migrations/snapshots, and all seeders/seed content have been removed. Legacy educational-service filters/events and optional cloud adapters still need a separate modularization review.
3. **Authorization tests:** Add end-to-end Keycloak login, refresh, revocation, role/audience rejection, and cross-user resource access tests against isolated services. Legacy symmetric-token configuration remains registered for compatibility; retire it after consumers migrate.
4. **Infrastructure options:** Cloud, RabbitMQ, email, Hangfire, and legacy encryption are configured even when unused. Replace static configuration with validated typed options and explicit module registration. Avoid requiring unrelated secrets at startup.
5. **Persistence integration tests:** Test rollback, nested mediator calls, concurrency, soft delete/auditing, and migrations against real PostgreSQL. Current checks cover error/diagnostic behavior; they are not a substitute for database integration coverage.
6. **Release operations:** Enable branch protection, private security reporting, and reviewed tagged releases. Review AutoMapper/MediatR license eligibility separately from the repository's MIT license. Pin container versions/digests as part of an update process.
7. **Production execution:** The sample Python subprocess runner is not a sandbox. Replace it with a separately isolated execution service before exposing it to untrusted users.
8. **Portal deployment:** Source for the existing portfolio and its deployment environment is required to install the prepared page and reverse proxy. The standalone downloader is ready for integration; the live portal has not been changed.

This audit describes the actual code state rather than certifying the project as production-ready.
