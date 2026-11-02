# ────────────────────────────────────────────────────────────────────────────
# Stage 1 — build
# Force linux/amd64: Grpc.Tools protoc binary is amd64-only.
# ────────────────────────────────────────────────────────────────────────────
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:10.0 AS builder

WORKDIR /src

# ── 1. Copy solution manifests for cached restore layer ───────────────────────
COPY Static.sln               .
COPY Directory.Packages.props .
COPY nuget.config global.json ./

# Projects that Static.API depends on (direct + transitive)
COPY Static.API/Static.API.csproj                                 Static.API/
COPY Static.Application/Static.Application.csproj                 Static.Application/
COPY Static.Domain/Static.Domain.csproj                           Static.Domain/
COPY Static.Identity/Static.Identity.csproj                       Static.Identity/
COPY Static.Infrastructure/Static.Infrastructure.csproj           Static.Infrastructure/
COPY Static.Helper/Static.Helper.csproj                           Static.Helper/
COPY Static.Results/Static.Results.csproj                         Static.Results/
COPY Static.QueryUtils/Static.QueryUtils.csproj                   Static.QueryUtils/
COPY Static.OpenTelemetry/Static.OpenTelemetry.csproj             Static.OpenTelemetry/
COPY Static.Cryptography/Static.Cryptography.csproj               Static.Cryptography/
COPY Static.DateTimeProvider/Static.DateTimeProvider.csproj       Static.DateTimeProvider/
COPY Static.MediatR/Static.MediatR.csproj                         Static.MediatR/
COPY Static.Redis/Static.Redis.csproj                             Static.Redis/
COPY Static.UserSession/Static.UserSession.csproj                 Static.UserSession/
COPY Static.EntityFrameworkCore/Static.EntityFrameworkCore.csproj Static.EntityFrameworkCore/
COPY Static.Protos/Static.Protos.csproj                           Static.Protos/

RUN dotnet restore Static.API/Static.API.csproj

# ── 2. Copy all source and publish ────────────────────────────────────────────
COPY Static.API/               Static.API/
COPY Static.Application/       Static.Application/
COPY Static.Domain/            Static.Domain/
COPY Static.Identity/          Static.Identity/
COPY Static.Infrastructure/    Static.Infrastructure/
COPY Static.Helper/            Static.Helper/
COPY Static.Results/           Static.Results/
COPY Static.QueryUtils/        Static.QueryUtils/
COPY Static.OpenTelemetry/     Static.OpenTelemetry/
COPY Static.Cryptography/      Static.Cryptography/
COPY Static.DateTimeProvider/  Static.DateTimeProvider/
COPY Static.MediatR/           Static.MediatR/
COPY Static.Redis/             Static.Redis/
COPY Static.UserSession/       Static.UserSession/
COPY Static.EntityFrameworkCore/ Static.EntityFrameworkCore/
COPY Static.Protos/            Static.Protos/

RUN dotnet publish Static.API/Static.API.csproj \
    -c Release \
    --no-restore \
    -o /app/publish

# ────────────────────────────────────────────────────────────────────────────
# Stage 2 — runtime (lean image, no SDK)
# ────────────────────────────────────────────────────────────────────────────
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

# Install Python 3 for the code execution sandbox
RUN apt-get update \
 && apt-get install -y --no-install-recommends python3 \
 && ln -sf /usr/bin/python3 /usr/local/bin/python \
 && rm -rf /var/lib/apt/lists/*

RUN groupadd --system --gid 1001 appgroup \
 && useradd  --system --uid 1001 --gid appgroup --shell /bin/sh --no-create-home appuser

WORKDIR /app

COPY --from=builder --chown=appuser:appgroup /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

USER appuser

ENTRYPOINT ["dotnet", "Static.API.dll"]
