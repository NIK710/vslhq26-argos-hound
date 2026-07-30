# ArgosHound

ArgosHound is an AI talent agent that helps builders discover opportunities hidden in online discussions.

## Team

- **Solo**
- **Member:** Nikhar Khamesra ([@NIK710](https://github.com/NIK710))

## Category

- **Primary:** AI agent/workflow automation
- **Secondary:** Azure OpenAI/LLM app

## What it does

ArgosHound is an AI talent agent for builders that scans online discussions to discover opportunities others miss. It analyzes real user problems and determines whether they represent a Product Opportunity for your own product, a Builder Opportunity you should pursue, or a new insight that improves future recommendations.

Core Features

1. Product Opportunity Discovery
Find people who could benefit from your own product—whether it's a direct match, an adjacent use case, or a simple feature you could build to solve their problem.

2. Builder Opportunity Discovery
Discover collaborators, customers, projects, hackathons, jobs, or other opportunities that match your skills and interests.

3. Continuous Learning
Learn from every recommendation and user interaction that sticks to continuously improve future opportunity matching and prioritization.

The MVP includes:

- A review and approval flow for profile summaries exported from an AI assistant
- Structured analysis with source and product validation
- Persisted opportunity reports with deterministic scores
- Campaign links that record campaign-level opens without profiling commenters
- Builder decisions, outcomes, history summaries, and explainable rescoring
- A deterministic local demo provider and an optional Microsoft Foundry provider

## Architecture

```text
                         +--------------------------+
                         | Public source            |
                         | (Reddit, YouTube, etc)   |
                         +----------+---------------+
                                    |
                                    v
+---------+      +----------------------------------------------+
| React   |<---->| ASP.NET Core API                             |
| UI      |      |                                              |
+---------+      | Source | Discovery | Scoring | Attribution   |
                 | Decisions | Outcomes | Learning | Validation |
                 +-----------+----------------------+-----------+
                             |                      |
                             v                      v
                 +----------------------+   +---------------+
                 |                      |   | SQLite        |
                 | Foundry agent        |   | opportunities |
                 | + Azure OpenAI       |   | and events    |
                 +----------------------+   +---------------+
```

The backend handles validation, scoring, persistence, and external side effects. Model
output is treated as a proposal and must reference supplied evidence and known product
capabilities before it can be presented.

More detail:

- [Product definition](docs/PROJECT.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Implementation checklist](docs/TASKS.md)
- [Environment reference](docs/ENVIRONMENT.md)

## Tech stack

- **Languages:** TypeScript, C#
- **Frontend:** React 19, Vite 8
- **Backend:** ASP.NET Core 9, Entity Framework Core 9
- **Database:** SQLite
- **AI:** Microsoft Foundry agent and Azure OpenAI
- **Hosting:** Local development in the current MVP

## Getting started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- A recent Node.js release with npm
- Git


Never commit Azure API keys or other secrets.

### Setup

Clone and install dependencies:

```bash
git clone https://github.com/NIK710/vslhq26-argos-hound.git
cd vslhq26-argos-hound

dotnet restore backend/ArgosHound.Api.csproj
cd frontend
npm install
cd ..
```

Create a private backend environment file from the provided example:

```bash
cp backend/.env.example backend/.env
```

Edit `backend/.env`, replace the placeholders with your Foundry and Azure OpenAI
values, and keep `Analysis__UseDemoProvider=false`. Never commit this file.

Authenticate with Azure, load the environment file, and start the backend:

```bash
az login

cd backend
set -a
source .env
set +a
dotnet run --no-launch-profile
```

The signed-in Azure identity must have permission to invoke the configured Foundry
agent. ASP.NET Core does not load `.env` automatically, which is why the `source`
command is required. The API listens at `http://localhost:5080`.

In a second terminal, start the frontend:

```bash
cd frontend
npm run dev
```

Open `http://localhost:5173`.

### Configuration

For an offline fixture demo, run `dotnet run`; the development launch profile selects
the deterministic provider.

Quote values containing spaces in `backend/.env`, for example:

```bash
ConnectionStrings__ArgosHound="Data Source=argoshound.db"
```

See [.env.example](.env.example) and [docs/ENVIRONMENT.md](docs/ENVIRONMENT.md).

### Verification

```bash
dotnet test backend.Tests/ArgosHound.Api.Tests.csproj

cd frontend
npm run build
```

## Demo

1. Review the seeded builder profile.
2. Open the profile-export prompt, copy it into an AI assistant, and paste the returned
   JSON into ArgosHound.
3. Review and approve the field-level profile proposal.
4. Analyze the doomscrolling discussion to identify a potential user.
5. Analyze the chess-club discussion to produce a `BUILDER` opportunity.
6. Analyze the keyboard showcase to demonstrate a `NONE` result.
7. Save or pursue an opportunity, report an outcome, and observe the explainable
   history score factor.

- **Video:** Not included yet


## Known limitations

- The MVP uses pre-determined discussions rather than live Reddit data.
- Profiles and source data are in memory; opportunities and events use local SQLite.
  There are no accounts, multi user isolation, or production migrations.
- Profile import accepts a user pasted JSON summary instead of connecting directly to
  ChatGPT or Claude memory.
- Learning uses explicit weights, and non click outcomes are manually reported.

## License

MIT
