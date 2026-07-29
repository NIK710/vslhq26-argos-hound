# ArgosHound Architecture

**Status:** Hackathon MVP

**Canonical product definition:** [PROJECT.md](./PROJECT.md)

## 1. Goals

The system must support two end-to-end funnels:

1. Detect a discussion that matches a builder-owned product.
2. Detect a discussion that represents a valuable builder opportunity.

Both funnels share source ingestion, evidence extraction, ranking, campaign attribution,
feedback, and outcome learning.

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

POST /api/sources/discussions
GET  /api/sources/discussions/{id}

POST /api/discovery

GET  /api/opportunities
GET  /api/opportunities/{id}
POST /api/opportunities/{id}/decisions
POST /api/opportunities/{id}/outcomes

POST /api/opportunities/{id}/campaign-links
GET  /r/{code}
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

## 8. Structured AI Contract

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

## 9. Scoring and Learning

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

Examples:

- Repeated activations from a subreddit increase its future source-history factor.
- Repeated dismissals of high-effort ideas reduce similar recommendations.
- High reported learning value increases related builder opportunities.
- A click alone has less weight than activation, project completion, or a career result.

The learning summary given to the model contains aggregates, not a list of tracked
individuals.

## 10. Safety, Privacy, and Reliability

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

Platform terms and applicable law must be reviewed before enabling a live or commercial
connector.

## 11. Repository Shape

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

## 12. MVP Validation

The architecture is proven when:

1. A seeded discussion produces a product opportunity with valid evidence.
2. A different discussion produces a builder opportunity matched to learning goals.
3. A weak discussion produces `NONE`.
4. A campaign link records an attributed event and redirects safely.
5. A builder decision or outcome changes a later score through visible factors.
6. No external outreach occurs without the builder.
