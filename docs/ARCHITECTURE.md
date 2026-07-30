# ArgosHound Architecture

**Status:** Hackathon MVP

**Canonical product definition:** [PROJECT.md](./PROJECT.md)

## 1. Goals

The system must support two end-to-end funnels:

1. Detect a discussion that matches a builder-owned product.
2. Detect a discussion that represents a valuable builder opportunity.

Both funnels share source ingestion, evidence extraction, ranking, campaign attribution,
feedback, and outcome learning.

Builder context can be entered directly or imported from a user-approved summary
produced by an external AI assistant. Consumer-assistant memory is not assumed to be
available through model APIs or identity sign-in.

## 2. Technology Stack

### Frontend

- React
- Vite
- TypeScript

### Backend

- ASP.NET Core Web API
- Entity Framework Core
- SQLite for the MVP

### AI

- Microsoft Foundry for agent orchestration
- Azure OpenAI for structured analysis and reasoning
- A provider abstraction so another supported LLM can perform analysis later

### Initial source

- Seeded or manually pasted Reddit discussions
- A live Reddit connector is optional and must use approved access methods

## 3. System Context

```text
                         +----------------------+
                         | Public source        |
                         | Reddit initially     |
                         +----------+-----------+
                                    |
                                    v
+---------+      +---------------------------------------------+
| React   |<---->| ASP.NET Core API                            |
| UI      |      |                                             |
+---------+      | Source | Discovery | Scoring | Attribution  |
                 | Feedback | Learning | Validation             |
                 +-----------+----------------------+----------+
                             |                      |
                             v                      v
                 +----------------------+   +---------------+
                 | Foundry agent        |   | SQLite        |
                 | + Azure OpenAI       |   | application   |
                 | structured analysis  |   | data/events   |
                 +----------------------+   +---------------+
```

The backend owns validation, persistence, scoring, policy boundaries, and all external
side effects. The model analyzes supplied content and returns structured proposals.

### Structured-analysis boundary

`POST /api/analysis/discussions/{discussionId}` is the non-persisting analysis preview
used before discovery creates an opportunity. The controller supplies the current
Builder Profile, product catalog, and selected discussion to `ILlmAnalysisProvider`.
The Foundry implementation invokes the pinned prompt-agent version with a per-attempt
timeout and bounded transient retries.

Prompt templates and the JSON schema live outside application business logic under
`backend/Prompts` and `backend/Schemas`. Source content is serialized inside an explicit
untrusted-input boundary. After the model responds, backend validation rejects malformed
JSON, unsupported enums, invalid product-match combinations, unknown evidence or
product IDs, invented capability references, and confidence outside zero to one.
Only validated analysis is returned to the review UI; this endpoint does not persist an
opportunity or perform external outreach.

`POST /api/discovery` applies the same validated provider boundary, calculates
deterministic score factors, and persists an opportunity plus normalized source-evidence
references in SQLite. Discovery is idempotent per discussion for the MVP: repeating a
request returns the stored opportunity rather than invoking the model again. The list
and detail reports are available from `GET /api/opportunities` and
`GET /api/opportunities/{id}`.

## 4. Discovery Pipeline

```text
Ingest discussion
       |
       v
Normalize source and comments
       |
       v
Select potentially relevant content
       |
       v
Analyze problem, topic, sentiment, and evidence
       |
       v
Match against builder products
       |
       +-- strong fit --> PRODUCT
       |
       v
Match against skills, goals, interests, and constraints
       |
       +-- strong fit --> BUILDER
       |
       +-- no fit -----> NONE
       |
       v
Validate model output
       |
       v
Calculate transparent score
       |
       v
Persist and present report
```

The model must identify supporting source content. Backend validation must reject
unknown product IDs, unsupported enum values, invalid score ranges, and evidence that
does not exist in the supplied discussion.

## 5. Components

### Controllers

- `BuilderController`
- `SourcesController`
- `DiscoveryController`
- `OpportunitiesController`
- `CampaignsController`
- `EventsController`
- `FeedbackController`
- `LearningController`

### Services

- `BuilderService`
- `ProfileImportService`
- `SourceIngestionService`
- `DiscoveryService`
- `FoundryAgentService`
- `OpportunityValidationService`
- `ScoringService`
- `CampaignLinkService`
- `EngagementService`
- `LearningService`

### Agent tools

The Foundry agent may receive controlled, read-only tools for:

- Builder profile lookup
- Product catalog lookup
- Learning-context lookup
- Source-discussion lookup
- Opportunity-history lookup

Writes are performed only by validated backend workflows. The agent cannot send
messages, publish comments, or fabricate source records.

## 6. Domain Model

### BuilderProfile

- `id`
- `name`
- `currentSkills[]`
- `learningGoals[]`
- `interests[]`
- `preferredOpportunityTypes[]`
- `location` (optional and coarse)
- `effortPreferences`

### ProfileImport

- `id`
- `builderId`
- `provider`: `CHATGPT | CLAUDE | OTHER`
- `method`: `PASTED_SUMMARY | UPLOADED_EXPORT | AUTHORIZED_CONNECTION`
- `status`: `UPLOADED | EXTRACTED | APPROVED | REJECTED | DELETED`
- `proposedProfile`
- `createdAt`
- `approvedAt` (optional)
- `rawContentExpiresAt`

Imported context is untrusted input. Extracted fields are proposals until the builder
reviews and approves them. Provider credentials, if supported in the future, are stored
separately from the import record.

### Product

- `id`
- `builderId`
- `name`
- `description`
- `capabilities[]`
- `targetUsers[]`
- `productUrl`

### SourceDiscussion

- `id`
- `platform`
- `externalId`
- `community`
- `title`
- `body`
- `url`
- `authorHandle` (optional)
- `publishedAt`
- `retrievedAt`

### SourceComment

- `id`
- `discussionId`
- `externalId`
- `parentExternalId` (optional)
- `body`
- `url`
- `authorHandle` (optional)
- `publishedAt`

Handles are retained only when needed to link back to source evidence. They are not a
separate lead profile and must not be joined into cross-platform identities.

### Opportunity

- `id`
- `discussionId`
- `type`: `PRODUCT | BUILDER | NONE`
- `productMatchType`: `DIRECT | ADJACENT | SMALL_EXTENSION | null`
- `builderOpportunityType` (nullable)
- `problem`
- `topic`
- `sentiment`
- `matchedProductId` (nullable)
- `matchedCapabilities[]`
- `limitations[]`
- `evidenceReferences[]`
- `explanation`
- `suggestedAction`
- `confidence`
- `score`
- `scoreFactors`
- `status`
- `createdAt`

### CampaignLink

- `id`
- `opportunityId`
- `codeHash`
- `destinationUrl`
- `purpose`: `PRODUCT | PORTFOLIO | PROJECT`
- `createdAt`
- `expiresAt` (optional)

Only a hash of the random code should be persisted when practical. The code represents
an opportunity or campaign, not an individual source author.

The MVP generates 32 random bytes and encodes them as a 43-character base64url code.
Only its SHA-256 hash is stored. The raw redirect URL is shown once when the campaign is
created and cannot be reconstructed from storage. Redirect destinations must match an
exact configured host; non-local destinations require HTTPS. Successful redirects add
an `OPENED` event with empty metadata and return `no-store` and `no-referrer` headers.
No IP address, user agent, cookie, source handle, or commenter identifier is retained.

### EngagementEvent

- `id`
- `campaignLinkId`
- `eventType`: `OPENED | EXPLORED | SIGNED_UP | ACTIVATED | CONTACTED | CONVERTED`
- `occurredAt`
- `metadata` (allowlisted, non-sensitive)

### BuilderDecision

- `id`
- `opportunityId`
- `decision`: `SAVED | DISMISSED | PURSUED`
- `reason` (optional)
- `createdAt`

### Outcome

- `id`
- `opportunityId`
- `outcomeType`
- `value`
- `notes` (optional)
- `occurredAt`

Examples include learning value, prototype completed, portfolio added, collaboration,
interview, contract, active user, or purchase.

## 7. API

```text
GET  /api/builder
PUT  /api/builder
POST /api/builder/profile-imports
GET  /api/builder/profile-imports/{id}
POST /api/builder/profile-imports/{id}/approve
DELETE /api/builder/profile-imports/{id}

POST /api/sources/discussions
GET  /api/sources/discussions/{id}

POST /api/discovery

GET  /api/opportunities
GET  /api/opportunities/{id}
POST /api/opportunities/{id}/campaign-links
GET  /r/{code}
POST /api/opportunities/{id}/decisions
POST /api/opportunities/{id}/outcomes

POST /api/events

GET  /api/learning/summary
```

`POST /api/discovery` accepts one or more stored discussion IDs and returns persisted,
validated opportunities. For the demo it may run synchronously. A production system
would use background jobs and idempotency keys.

`GET /r/{code}` records an `OPENED` event and redirects to the allowlisted destination.
More meaningful destination events may be sent to `POST /api/events`.

All error responses should use one documented problem-details format. List endpoints
should support pagination before production ingestion is enabled.

## 8. LLM Provider and Context Integration

The application must distinguish three concepts:

- **ArgosHound authentication:** who is using this application.
- **Model authorization:** which API or managed deployment ArgosHound uses for analysis.
- **Assistant-context authorization:** whether a provider permits ArgosHound to read
  user-selected memory or conversation context.

One authorization must not be treated as another. A “Sign in with ChatGPT” or similar
identity flow cannot imply memory access without an official provider scope and API.

### Model provider boundary

`ILlmAnalysisProvider` owns structured opportunity and profile extraction:

```text
AnalyzeOpportunity(request) -> OpportunityAnalysis
ExtractProfile(import)       -> ProposedBuilderProfile
```

The first implementation uses Foundry and Azure OpenAI. Provider-specific SDK types,
model names, and authentication remain behind the interface so a future Anthropic or
other implementation does not alter the domain model.

### Context import boundary

The MVP accepts a narrow pasted summary generated by the user's preferred assistant.
The backend:

1. Treats imported text as untrusted.
2. Extracts only Builder Profile fields.
3. Shows a field-level preview and source provider.
4. Saves nothing to the active profile until explicit approval.
5. Deletes raw content after approval or a short expiration period.

Full account exports are a fallback, not the default. If supported, processing should
occur with an explicit selection step and should exclude unrelated conversations and
sensitive personal data.

Direct synchronization is a future connector. It requires documented provider support,
least-privilege OAuth scopes, revocation, refresh-token protection, audit events, and a
clear “last synchronized” state.

## 9. Structured AI Contract

The agent response must be valid structured data containing:

- Top-level opportunity type
- Optional product or builder subtype
- Problem, topic, and sentiment
- Source evidence references
- Matched product ID or capabilities
- Limitations
- Explanation
- Suggested action
- Confidence

The model does not assign the final persisted score. The backend combines model
confidence with deterministic profile, history, effort, and risk factors.

Source discussions are untrusted data. Prompts must clearly delimit them from
instructions, and source text must never be allowed to request tools, secrets, or
external actions.

## 10. Scoring and Learning

The MVP stores explicit factor values:

```text
score =
  evidenceStrength
  + profileFit
  + productOrSkillFit
  + sourceHistory
  + topicHistory
  + preferenceHistory
  + outcomeHistory
  - effortCost
  - outreachRisk
```

Each factor is normalized before the result is bounded to a documented range. Learning
context is an aggregate over prior events, decisions, and outcomes; it does not require
training or fine-tuning a model.

Until history and outcome learning are implemented, the deterministic MVP score is
bounded to `0..100` and consists of:

- Evidence strength: up to 20 points from validated source references
- Explicit or inferred problem clarity: 15 or 8 points
- Product fit: 35 direct, 25 adjacent, or 18 small-extension points
- Builder fit: 20 to 30 points from available profile signals
- Actionability: 15 points for a reviewable next action
- Uncertainty: a penalty of up to 15 points from stated limitations

`NONE` receives score zero. Model confidence is persisted and displayed separately; it
does not change the deterministic score.

Examples:

- Repeated activations from a subreddit increase its future source-history factor.
- Repeated dismissals of high-effort ideas reduce similar recommendations.
- High reported learning value increases related builder opportunities.
- A click alone has less weight than activation, project completion, or a career result.

The learning summary given to the model contains aggregates, not a list of tracked
individuals.

## 11. Safety, Privacy, and Reliability

- Use approved source APIs or builder-provided content; do not scrape around controls.
- Keep original source links and attribution.
- Minimize stored comments and honor source deletion requirements.
- Do not derive sensitive traits or build cross-platform person profiles.
- Do not expose tracking codes in logs.
- Use random, unguessable campaign codes and allowlisted redirect destinations.
- Disclose first-party campaign measurement on destination pages.
- Require human approval for outreach.
- Apply request limits, timeouts, retries, and model-output schema validation.
- Log model/version and prompt version for reproducibility without logging secrets.
- Treat confidence as uncertainty, not probability of conversion.
- Do not claim that LLM account sign-in grants access to memory or conversations.
- Require preview and approval before imported assistant context changes the profile.
- Minimize and expire raw memory or conversation imports.
- Allow the builder to disconnect a provider and delete imported context.

Platform terms and applicable law must be reviewed before enabling a live or commercial
connector.

## 12. Repository Shape

```text
argoshound/
|-- frontend/
|-- backend/
|   |-- Controllers/
|   |-- Agents/
|   |-- Tools/
|   |-- Services/
|   |-- Models/
|   |-- Data/
|   `-- Prompts/
|-- docs/
|   |-- PROJECT.md
|   |-- ARCHITECTURE.md
|   `-- TASKS.md
`-- README.md
```

## 13. MVP Validation

The architecture is proven when:

1. Pasted assistant context produces a proposed, reviewable Builder Profile.
2. A seeded discussion produces a product opportunity with valid evidence.
3. A different discussion produces a builder opportunity matched to learning goals.
4. A weak discussion produces `NONE`.
5. A campaign link records an attributed event and redirects safely.
6. A builder decision or outcome changes a later score through visible factors.
7. No external outreach occurs without the builder.
