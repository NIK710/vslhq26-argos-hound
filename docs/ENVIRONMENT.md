# Environment Configuration

ArgosHound has separate frontend and backend configuration. Example files contain
placeholder values only and are safe to commit. Real `.env` files and secrets are
ignored by Git.

## Frontend

Copy the frontend example:

```bash
cp frontend/.env.example frontend/.env
```

Vite loads `frontend/.env` when commands run from the `frontend/` directory.

| Variable | Required | Purpose | Default |
|---|---:|---|---|
| `VITE_API_BASE_URL` | No | Base URL used by the browser to call the API | `http://localhost:5080` |

Every variable prefixed with `VITE_` is bundled into browser code. Never put API keys,
tokens, connection strings, or other secrets in a `VITE_*` variable.

## Backend

ASP.NET Core reads configuration from environment variables. It does not automatically
load `backend/.env`. Export the variables in the shell, configure the IDE launch
profile, or use an approved local secret manager.

Current backend variables:

| Variable | Required | Purpose | Default |
|---|---:|---|---|
| `ASPNETCORE_ENVIRONMENT` | No | Selects the ASP.NET environment | `Production`; the launch profile uses `Development` |
| `ASPNETCORE_URLS` | No | Addresses on which the API listens | The launch profile uses `http://localhost:5080` |
| `Cors__AllowedOrigins__0` | No | First browser origin allowed by the CORS policy | `http://localhost:5173` |
| `Cors__AllowedOrigins__1` | No | Optional additional allowed origin | `http://127.0.0.1:5173` |

Double underscores map environment variables to nested ASP.NET configuration. For
example, `Cors__AllowedOrigins__0` maps to `Cors:AllowedOrigins:0`.

Multiple CORS origins can be configured by incrementing the final array index. Origins
must be explicit; do not configure a wildcard when browser credentials or private data
are introduced.

## Planned AI Configuration

These settings are documented now but are not required until the structured-analysis
integration is implemented:

| Variable | Secret | Purpose |
|---|---:|---|
| `AzureOpenAI__Endpoint` | No | Azure OpenAI resource endpoint |
| `AzureOpenAI__DeploymentName` | No | Model deployment used for analysis |
| `AzureOpenAI__ApiKey` | Yes | Local API credential when managed identity is unavailable |
| `Foundry__ProjectEndpoint` | No | Microsoft Foundry project endpoint |

Prefer managed identity in deployed environments. Never commit a real API key. Local
credentials should be supplied through shell environment variables, .NET user secrets,
or the deployment platform's secret store.

## Local Development

One shell can start the backend with explicit local configuration:

```bash
cd backend
ASPNETCORE_ENVIRONMENT=Development \
ASPNETCORE_URLS=http://localhost:5080 \
Cors__AllowedOrigins__0=http://localhost:5173 \
dotnet run
```

In another shell:

```bash
cd frontend
npm run dev
```

The frontend should report `Connected: Healthy` when both services are running.
