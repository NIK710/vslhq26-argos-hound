# ArgosHound Architecture

**Document status:** Initial MVP architecture  
**Primary goal:** Define how ArgosHound will be implemented for the hackathon MVP  
**Related documents:** `PROJECT.md`, `TASKS.md`, `DECISIONS.md`

---

## 1. System Overview

ArgosHound is an AI talent agent for builders.

It analyzes public online discussions, identifies the underlying pain point, and determines whether the discussion represents:

1. A product opportunity for one of the builder's own products.
2. A personal opportunity for the builder based on their skills, interests, and goals.
3. No meaningful opportunity.

Every reviewed opportunity can produce feedback and outcomes that improve future ranking.

The MVP uses:

- React
- Vite
- TypeScript
- ASP.NET Core Web API
- Azure OpenAI
- SQLite
- Reddit as the initial discussion source

The system is designed so additional sources, opportunity types, models, and storage providers can be added later without rewriting the core matching pipeline.

---

## 2. Architectural Principles

### 2.1 Human approval before action

ArgosHound recommends actions but does not automatically contact users, post comments, submit applications, or modify products.

The user remains responsible for approving and performing the recommended action.

### 2.2 AI for interpretation, code for control

Azure OpenAI is responsible for:

- Extracting pain points.
- Interpreting intent.
- Matching discussions to products or builder capabilities.
- Explaining recommendations.
- Suggesting next actions.

Deterministic application code is responsible for:

- Validation.
- Persistence.
- Filtering.
- Ranking formulas.
- Duplicate detection.
- API contracts.
- Security.
- Rate limiting.
- Feedback and outcome tracking.

### 2.3 Structured AI output

The backend must request structured JSON from the model and deserialize it into typed C# objects.

Free-form model output should not be passed directly to the frontend.

### 2.4 Source-independent pipeline

Reddit is the first source, but the opportunity engine must operate on a normalized discussion model rather than Reddit-specific objects.

### 2.5 Simple MVP, extensible boundaries

The MVP should use the simplest practical implementation while preserving clear interfaces for future changes.

Examples:

- SQLite now, Cosmos DB later.
- Manual discovery trigger now, scheduled discovery later.
- Manual feedback now, tracked links and external outcomes later.
- One builder now, multiple authenticated users later.

### 2.6 Explain every recommendation

Every opportunity must include:

- The detected pain point.
- The match type.
- Why it matches.
- Any important limitations.
- A recommended next action.
- A confidence score.

---

## 3. High-Level Architecture

```text
┌──────────────────────────────────────────────────────────┐
│                    React Frontend                        │
│                                                          │
│  Builder Profile  Opportunity Feed  Opportunity Details  │
│  Feedback Controls  Learning Summary                     │
└──────────────────────────┬───────────────────────────────┘
                           │ HTTPS / JSON
                           ▼
┌──────────────────────────────────────────────────────────┐
│                ASP.NET Core Web API                      │
│                                                          │
│  Controllers                                             │
│      │                                                   │
│  Application Services                                    │
│      ├── BuilderService                                  │
│      ├── DiscoveryService                                │
│      ├── OpportunityService                              │
│      ├── LearningService                                 │
│      └── Source Services                                 │
│                                                          │
│  AI Layer                                                │
│      ├── Prompt Loader                                   │
│      ├── LLM Client                                      │
│      └── Response Validator                              │
│                                                          │
│  Persistence Layer                                       │
│      ├── Entity Framework Core                           │
│      └── SQLite                                          │
└───────────────┬───────────────────────┬──────────────────┘
                │                       │
                ▼                       ▼
       ┌────────────────┐      ┌─────────────────┐
       │  Azure OpenAI  │      │ Reddit Source   │
       │                │      │ Public/API data │
       └────────────────┘      └─────────────────┘
```

---

## 4. Core Product Capabilities

### 4.1 Feature 1: Product Opportunity Discovery

Feature 1 searches for discussions where one of the builder's own products could provide value.

The matching spectrum is not binary.

#### Direct Match

The product already solves the user's stated problem.

#### Adjacent Match

The product does not exactly solve the problem but provides meaningful related value.

#### Product Evolution

A small feature, integration, configuration, or extension could allow the product to solve the problem.

The system must not claim that a product fully solves a problem when it only partially matches.

The AI output should include:

- Matching product ID.
- Product-match subtype.
- Detected pain point.
- Relevant product capabilities.
- Gaps or limitations.
- Suggested outreach.
- Suggested product enhancement when applicable.
- Estimated implementation effort for an enhancement.
- Confidence.

### 4.2 Feature 2: Builder Opportunity Discovery

Feature 2 finds opportunities the builder could personally pursue.

Initial opportunity categories:

- Freelance.
- Consulting.
- Collaboration.
- Open source.
- Startup idea.
- Community involvement.
- Research or learning opportunity.

The AI output should include:

- Opportunity category.
- Matching skills.
- Matching interests or goals.
- Missing qualifications or risks.
- Recommended action.
- Confidence.

### 4.3 Feature 3: Continuous Learning

Feature 3 records outcomes and adjusts future ranking.

The MVP will not train a custom machine-learning model.

Instead, the application will use transparent scoring adjustments based on historical outcomes.

Examples:

- Increase scores for communities with positive outcomes.
- Increase scores for products that convert well.
- Increase scores for opportunity categories the builder accepts.
- Decrease scores for repeatedly ignored recommendations.
- Track success independently for product opportunities and builder opportunities.

---

## 5. Repository Structure

```text
argoshound/
├── frontend/
│   ├── public/
│   ├── src/
│   │   ├── api/
│   │   ├── components/
│   │   ├── features/
│   │   │   ├── builder/
│   │   │   ├── discovery/
│   │   │   ├── opportunities/
│   │   │   └── feedback/
│   │   ├── hooks/
│   │   ├── pages/
│   │   ├── types/
│   │   ├── utils/
│   │   ├── App.tsx
│   │   └── main.tsx
│   ├── .env.example
│   ├── package.json
│   └── vite.config.ts
│
├── backend/
│   ├── ArgosHound.Api/
│   │   ├── Controllers/
│   │   ├── Contracts/
│   │   ├── Middleware/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── ArgosHound.Application/
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   ├── Scoring/
│   │   └── Validation/
│   │
│   ├── ArgosHound.Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── ValueObjects/
│   │
│   ├── ArgosHound.Infrastructure/
│   │   ├── AI/
│   │   ├── Data/
│   │   ├── Prompts/
│   │   ├── Repositories/
│   │   └── Sources/
│   │
│   └── ArgosHound.Tests/
│
├── docs/
│   ├── PROJECT.md
│   ├── ARCHITECTURE.md
│   ├── TASKS.md
│   └── DECISIONS.md
│
├── .gitignore
├── README.md
└── ArgosHound.sln
```

For a faster hackathon start, the backend may initially use one ASP.NET Core project with the same folders. The multi-project structure above is the preferred target, not a blocker for the first working demo.

---

## 6. Domain Model

### 6.1 Builder

Represents the person ArgosHound serves.

```csharp
public sealed class Builder
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public List<BuilderSkill> Skills { get; set; } = [];
    public List<Product> Products { get; set; } = [];
    public List<string> Interests { get; set; } = [];
    public List<string> Goals { get; set; } = [];
    public List<string> PreferredOpportunityTypes { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
```

### 6.2 BuilderSkill

```csharp
public sealed class BuilderSkill
{
    public Guid Id { get; set; }
    public Guid BuilderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public SkillLevel Level { get; set; }
    public int YearsOfExperience { get; set; }
}
```

### 6.3 Product

Represents a product owned or actively built by the builder.

```csharp
public sealed class Product
{
    public Guid Id { get; set; }
    public Guid BuilderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TargetAudience { get; set; } = string.Empty;
    public List<string> Capabilities { get; set; } = [];
    public List<string> KnownLimitations { get; set; } = [];
    public string? Url { get; set; }
    public ProductStatus Status { get; set; }
}
```

### 6.4 SourceDiscussion

A source-independent representation of an online discussion.

```csharp
public sealed class SourceDiscussion
{
    public Guid Id { get; set; }
    public SourcePlatform Platform { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string? Community { get; set; }
    public string? Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
    public string SourceUrl { get; set; } = string.Empty;
    public DateTimeOffset? PublishedAt { get; set; }
    public DateTimeOffset IngestedAt { get; set; }
    public string ContentHash { get; set; } = string.Empty;
}
```

### 6.5 Opportunity

```csharp
public sealed class Opportunity
{
    public Guid Id { get; set; }
    public Guid BuilderId { get; set; }
    public Guid SourceDiscussionId { get; set; }

    public OpportunityKind Kind { get; set; }
    public ProductMatchType? ProductMatchType { get; set; }
    public BuilderOpportunityType? BuilderOpportunityType { get; set; }

    public Guid? MatchedProductId { get; set; }

    public string PainPoint { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public string SuggestedAction { get; set; } = string.Empty;
    public string? SuggestedProductChange { get; set; }

    public int ConfidenceScore { get; set; }
    public decimal BaseScore { get; set; }
    public decimal LearningAdjustment { get; set; }
    public decimal FinalScore { get; set; }

    public OpportunityStatus Status { get; set; }
    public string PromptVersion { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
```

### 6.6 Feedback

```csharp
public sealed class OpportunityFeedback
{
    public Guid Id { get; set; }
    public Guid OpportunityId { get; set; }
    public FeedbackAction Action { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
```

### 6.7 Outcome

Feedback records immediate user judgment. Outcomes record what happened after pursuing an opportunity.

```csharp
public sealed class OpportunityOutcome
{
    public Guid Id { get; set; }
    public Guid OpportunityId { get; set; }
    public OutcomeType Type { get; set; }
    public decimal? Value { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
}
```

---

## 7. Important Enums

```csharp
public enum OpportunityKind
{
    Product,
    Builder,
    None
}

public enum ProductMatchType
{
    Direct,
    Adjacent,
    ProductEvolution
}

public enum BuilderOpportunityType
{
    Freelance,
    Consulting,
    Collaboration,
    OpenSource,
    StartupIdea,
    Community,
    Research,
    Learning
}

public enum OpportunityStatus
{
    New,
    Saved,
    Dismissed,
    Pursuing,
    Completed
}

public enum FeedbackAction
{
    Relevant,
    NotRelevant,
    Save,
    Dismiss,
    Pursue
}

public enum OutcomeType
{
    Click,
    Signup,
    Activation,
    Purchase,
    PortfolioView,
    GitHubView,
    Interview,
    Collaboration,
    Contract,
    PullRequestMerged,
    NoResponse,
    Rejected
}

public enum SourcePlatform
{
    Reddit,
    YouTube,
    GitHub,
    HackerNews,
    StackOverflow,
    Discord,
    Other
}
```

---

## 8. Backend Components

### 8.1 BuilderService

Responsibilities:

- Load the current builder profile.
- Create or update profile fields.
- Validate products and skills.
- Produce the compact builder context used by AI prompts.

### 8.2 RedditSourceService

Responsibilities:

- Fetch discussions from configured subreddits or search terms.
- Map Reddit-specific data into `SourceDiscussion`.
- Respect source limits and errors.
- Avoid duplicate ingestion.
- Return normalized discussions only.

The rest of the application must not depend on Reddit SDK types.

### 8.3 DiscoveryService

Orchestrates the full discovery pipeline.

Responsibilities:

1. Load builder profile.
2. Fetch or accept source discussions.
3. Remove duplicates.
4. Apply inexpensive prefilters.
5. Call the opportunity analyzer.
6. Validate structured AI output.
7. Calculate deterministic scores.
8. Persist accepted opportunities.
9. Return discovery results.

### 8.4 OpportunityAnalyzer

Responsible for AI interpretation.

Inputs:

- Builder profile.
- Builder-owned products.
- Source discussion.
- Prompt version.

Outputs:

- Opportunity kind.
- Pain point.
- Product or builder match details.
- Confidence.
- Reasoning.
- Suggested action.
- Limitations.
- Possible product evolution.

### 8.5 OpportunityService

Responsibilities:

- Retrieve opportunities.
- Filter by type, status, community, product, and score.
- Get opportunity details.
- Update opportunity status.
- Prevent duplicate active opportunities.

### 8.6 LearningService

Responsibilities:

- Record feedback.
- Record outcomes.
- Aggregate historical statistics.
- Calculate learning adjustments.
- Expose a summary for the UI.

### 8.7 ScoringService

Produces a final deterministic score.

```text
Final Score =
    Base AI Match Score
  + Community History Adjustment
  + Product History Adjustment
  + Opportunity-Type Adjustment
  + Recency Adjustment
  - Risk or Effort Penalty
```

The exact MVP weights should be stored in configuration.

### 8.8 PromptService

Responsibilities:

- Load prompt templates from files.
- Insert serialized builder and discussion data.
- Track prompt versions.
- Keep model instructions separate from application logic.

### 8.9 LlmClient

Responsibilities:

- Call Azure OpenAI.
- Request structured JSON.
- Apply timeouts and retries.
- Return raw usage and response metadata.
- Never expose API keys to the frontend.

---

## 9. AI Analysis Contract

The preferred model response is a single structured object.

```json
{
  "opportunityKind": "Product",
  "painPoint": "The user struggles to avoid long periods of YouTube scrolling.",
  "summary": "The builder's focus-interruption extension may help.",
  "confidenceScore": 88,
  "productMatch": {
    "productId": "PRODUCT_GUID",
    "matchType": "Adjacent",
    "matchingCapabilities": [
      "Interrupts selected distracting websites",
      "Requires a short learning task before continuing"
    ],
    "limitations": [
      "The current version may not cover every social platform"
    ],
    "suggestedProductChange": null,
    "estimatedChangeEffort": null
  },
  "builderOpportunity": null,
  "reasoning": "The stated pain point overlaps with the product's core behavior, although the exact site coverage should be verified.",
  "suggestedAction": "Reply with a helpful explanation and mention the product only if self-promotion is allowed.",
  "riskFlags": [
    "Check community self-promotion rules"
  ]
}
```

For a non-opportunity:

```json
{
  "opportunityKind": "None",
  "painPoint": "The post does not describe an actionable problem.",
  "summary": "No strong match.",
  "confidenceScore": 92,
  "productMatch": null,
  "builderOpportunity": null,
  "reasoning": "The discussion is informational and does not indicate a need the builder can address.",
  "suggestedAction": "Ignore.",
  "riskFlags": []
}
```

The backend must validate:

- Enum values.
- Confidence range from 0 to 100.
- Product ID belongs to the builder.
- Product match exists only for product opportunities.
- Builder match exists only for builder opportunities.
- Required strings are non-empty.
- Suggested claims do not exceed product capabilities.

Invalid responses should be retried once with a repair prompt, then rejected safely.

---

## 10. Prompt Design

The initial prompt may perform classification and matching in one model call for speed.

Prompt sections:

1. System role and safety constraints.
2. Builder profile.
3. Builder-owned products.
4. Source discussion.
5. Opportunity decision rules.
6. Product-match rules.
7. Builder-opportunity rules.
8. Output JSON schema.
9. Examples.

Important product-match instruction:

> Only evaluate products owned by this builder. A match may be direct, adjacent, or achievable through a small product evolution. Clearly state limitations and never claim a complete solution when the product only partially fits.

Prompt files:

```text
Infrastructure/Prompts/
├── opportunity-analysis-v1.md
├── opportunity-repair-v1.md
└── discussion-prefilter-v1.md
```

Every persisted opportunity should store its prompt version.

---

## 11. Discovery Pipeline

```text
User clicks Discover
        │
        ▼
Load Builder Profile
        │
        ▼
Fetch Reddit Discussions
        │
        ▼
Normalize Discussions
        │
        ▼
Remove Existing Content Hashes
        │
        ▼
Apply Basic Prefilters
        │
        ▼
Analyze Each Candidate with Azure OpenAI
        │
        ▼
Validate AI Response
        │
        ▼
Discard "None" or Low-Confidence Results
        │
        ▼
Calculate Learning Adjustment
        │
        ▼
Persist Opportunity
        │
        ▼
Return Ranked Results
```

For the MVP, analysis may run sequentially or with a small concurrency limit.

Do not send an unlimited batch of discussions to Azure OpenAI.

Suggested initial limits:

- 10 to 25 discussions per discovery run.
- Maximum 3 concurrent model calls.
- Configurable minimum confidence threshold.
- Cancellation support from the HTTP request.

---

## 12. API Design

Base path:

```text
/api
```

### Builder

```http
GET /api/builder
```

Returns the current builder profile.

```http
PUT /api/builder
```

Creates or updates the current builder profile.

### Products

```http
POST /api/builder/products
PUT /api/builder/products/{productId}
DELETE /api/builder/products/{productId}
```

These endpoints may be deferred if products are edited through the full builder update.

### Discovery

```http
POST /api/discovery
```

Example request:

```json
{
  "subreddits": ["startups", "productivity"],
  "searchTerms": ["doomscrolling", "focus"],
  "maxDiscussions": 15
}
```

Example response:

```json
{
  "analyzed": 15,
  "created": 4,
  "ignored": 11,
  "opportunityIds": [
    "GUID_1",
    "GUID_2"
  ]
}
```

For an early demo, the endpoint may also accept manually pasted discussion text.

```http
POST /api/discovery/analyze
```

```json
{
  "title": "Optional title",
  "content": "Discussion text",
  "sourceUrl": "https://example.com/post"
}
```

This manual endpoint is strongly recommended as a demo fallback.

### Opportunities

```http
GET /api/opportunities
GET /api/opportunities/{id}
PATCH /api/opportunities/{id}/status
```

Supported query parameters may include:

- `kind`
- `status`
- `productId`
- `community`
- `minimumScore`
- `page`
- `pageSize`

### Feedback and Outcomes

```http
POST /api/opportunities/{id}/feedback
POST /api/opportunities/{id}/outcomes
GET  /api/learning/summary
```

---

## 13. Frontend Architecture

The frontend should be organized by product feature rather than by generic component type alone.

### Initial pages

#### Dashboard

Shows:

- Builder summary.
- Product count.
- Recent opportunity count.
- High-confidence opportunities.
- Basic learning summary.

#### Builder Profile

Allows editing:

- Name.
- Bio.
- Skills.
- Interests.
- Goals.
- Owned products.
- Product capabilities and limitations.

#### Discovery

Allows:

- Selecting configured communities.
- Entering search terms.
- Starting discovery.
- Manually pasting a discussion for analysis.
- Viewing progress and errors.

#### Opportunity Feed

Displays ranked opportunity cards.

Each card should show:

- Product or builder opportunity.
- Direct, adjacent, or product-evolution badge when applicable.
- Pain point.
- Matched product or skills.
- Confidence.
- Final score.
- Recommended action.

#### Opportunity Details

Displays:

- Original source discussion.
- Full reasoning.
- Product limitations.
- Suggested product change.
- Risk flags.
- Feedback controls.
- Outcome controls.

#### Learning Summary

For MVP, this may be a section on the dashboard rather than a separate page.

### Frontend data access

Use a centralized API client.

```text
src/api/
├── client.ts
├── builderApi.ts
├── discoveryApi.ts
├── opportunityApi.ts
└── learningApi.ts
```

The frontend must not call Reddit or Azure OpenAI directly.

---

## 14. Persistence

### MVP database

SQLite with Entity Framework Core.

Suggested tables:

- Builders.
- BuilderSkills.
- Products.
- ProductCapabilities.
- ProductLimitations.
- SourceDiscussions.
- Opportunities.
- OpportunityFeedback.
- OpportunityOutcomes.
- LearningAggregates.

### Duplicate detection

Store a normalized content hash based on:

- Platform.
- External ID when available.
- Normalized title and content.

Use a unique index on platform plus external ID when possible.

### Future Cosmos DB migration

Repository interfaces should isolate persistence details where practical.

Do not introduce Cosmos-specific concepts into domain entities during the MVP.

---

## 15. Learning and Ranking

### Base score

The initial base score should combine:

- Model confidence.
- Strength of product or skill overlap.
- Specificity of the detected need.
- Actionability.
- Estimated effort.
- Recency.

Example:

```text
BaseScore =
    Confidence × 0.40
  + MatchStrength × 0.25
  + Actionability × 0.20
  + Recency × 0.10
  - EffortPenalty × 0.05
```

All components can be normalized to a 0-100 scale.

### Learning adjustment

Example signals:

```text
Relevant feedback                 +3
Saved                             +2
Pursued                           +5
Successful outcome               +10
Dismissed                         -4
Not relevant                      -8
Repeated no response              -2
```

Adjustments should be aggregated by:

- Source community.
- Matched product.
- Opportunity kind.
- Builder-opportunity subtype.
- Product-match subtype.

The MVP must keep the scoring transparent and easy to explain in the demo.

---

## 16. Configuration and Secrets

Backend configuration:

```text
AzureOpenAI__Endpoint
AzureOpenAI__ApiKey
AzureOpenAI__DeploymentName
Reddit__ClientId
Reddit__ClientSecret
Reddit__UserAgent
ConnectionStrings__Default
Discovery__MinimumConfidence
Discovery__MaximumDiscussions
Discovery__MaximumConcurrency
```

Local secrets should use:

- .NET user secrets, or
- an uncommitted `.env` mechanism.

Production secrets should use Azure-managed configuration or Key Vault.

Never commit:

- API keys.
- Client secrets.
- Access tokens.
- Production connection strings.

Include `.env.example` or configuration documentation with placeholder values.

---

## 17. Error Handling

Use centralized exception-handling middleware.

Return consistent problem details.

```json
{
  "type": "https://argoshound.dev/errors/ai-response-invalid",
  "title": "The AI response could not be validated.",
  "status": 502,
  "detail": "The analysis was not saved.",
  "traceId": "..."
}
```

Expected error categories:

- Validation error.
- Source unavailable.
- Source rate limited.
- Azure OpenAI unavailable.
- AI response invalid.
- Database error.
- Opportunity not found.
- Configuration missing.

The UI should provide a retry action for discovery failures.

---

## 18. Logging and Observability

Use structured ASP.NET Core logging.

Log:

- Discovery run ID.
- Number of discussions fetched.
- Number skipped as duplicates.
- AI request duration.
- AI response validation failures.
- Opportunities created.
- Prompt version.
- Token usage when available.
- Estimated request cost when practical.

Do not log:

- Secrets.
- Full API keys.
- Sensitive private profile fields.
- Unnecessarily large raw model responses.

A discovery run should have a correlation ID shared across logs.

---

## 19. Security and Responsible Use

The MVP uses public discussions only.

Requirements:

- Do not scrape or access private communities without permission.
- Respect source API terms and rate limits.
- Preserve source attribution and links.
- Avoid storing more personal data than necessary.
- Do not automatically send outreach.
- Flag communities that prohibit promotion.
- Make limitations clear for adjacent product matches.
- Avoid deceptive messaging or impersonation.
- Allow deletion of imported discussions and generated opportunities.

Before multi-user deployment, add:

- Authentication.
- Authorization.
- Tenant isolation.
- Encryption review.
- Data-retention policy.
- Abuse prevention.
- Audit logging.

---

## 20. Testing Strategy

### Unit tests

Prioritize:

- Scoring formulas.
- Learning adjustments.
- AI response validation.
- Product ownership validation.
- Duplicate detection.
- Status transitions.
- Mapping source objects into `SourceDiscussion`.

### Integration tests

Test:

- API endpoints with an in-memory or temporary SQLite database.
- Repository behavior.
- Full discovery pipeline with mocked source and LLM clients.
- Invalid AI response repair and failure handling.

### Prompt evaluation

Maintain a small evaluation dataset.

```text
tests/evals/
├── direct-product-matches.json
├── adjacent-product-matches.json
├── product-evolution-matches.json
├── builder-opportunities.json
└── negative-examples.json
```

Each case should include:

- Builder profile.
- Products.
- Discussion.
- Expected opportunity kind.
- Expected product or skill match.
- Minimum acceptable confidence.
- Notes.

Prompt changes should be tested against this dataset before replacement.

### End-to-end test

The critical demo flow:

1. Edit builder profile.
2. Add an owned product.
3. Paste or retrieve a discussion.
4. Run analysis.
5. Receive a valid opportunity.
6. View details.
7. Submit feedback.
8. Record an outcome.
9. See the learning summary update.

---

## 21. Deployment

### MVP deployment target

- Frontend: Azure Static Web Apps or served separately.
- Backend: Azure Container Apps or Azure App Service.
- Database: SQLite for local/demo use.
- AI: Azure OpenAI.
- Secrets: Azure configuration or Key Vault.

SQLite is acceptable for a single-instance hackathon demo.

For multi-instance or persistent production deployment, migrate to Cosmos DB, Azure SQL, or PostgreSQL before relying on horizontal scaling.

### Containerization

Backend should include a Dockerfile.

Optional root-level development setup:

```text
docker-compose.yml
```

The initial local workflow should not require Docker if that slows development.

---

## 22. MVP Implementation Order

### Milestone 1: Local vertical slice

- Builder profile stored locally.
- One owned product.
- Manual discussion input.
- Azure OpenAI analysis.
- Structured opportunity response.
- Opportunity card displayed.

### Milestone 2: Persistence and feed

- SQLite.
- Opportunity storage.
- Opportunity feed.
- Opportunity details.
- Duplicate prevention.

### Milestone 3: Three feature types

- Direct product matches.
- Adjacent product matches.
- Product evolution suggestions.
- Builder opportunities.
- None classification.

### Milestone 4: Feedback loop

- Feedback controls.
- Outcome recording.
- Transparent score adjustments.
- Learning summary.

### Milestone 5: Reddit integration

- Fetch Reddit discussions.
- Normalize source data.
- Configurable communities or search terms.
- Discovery run limits.
- Source attribution.

### Milestone 6: Demo polish

- Loading and empty states.
- Error handling.
- Seeded builder profile.
- Seeded evaluation discussions.
- Reliable manual-input fallback.
- Deployment to Azure.

---

## 23. Explicitly Deferred Work

Do not prioritize these before the complete MVP flow works:

- Authentication.
- Multiple builders.
- Automated outreach.
- Reddit posting.
- Browser extension.
- Discord integration.
- GitHub integration.
- YouTube integration.
- Dynamic portfolio pages.
- Payments.
- Notifications.
- Scheduled background discovery.
- Custom model fine-tuning.
- Complex machine learning.
- Vector database.
- Multi-agent orchestration.
- Production-scale analytics.

---

## 24. Architectural Success Criteria

The architecture is successful when:

- The frontend can analyze a discussion through one backend API.
- The AI returns typed, validated opportunity data.
- Product matches only use the builder's own products.
- Direct, adjacent, and product-evolution matches are distinguishable.
- Builder opportunities are separate from product opportunities.
- Invalid or weak matches can be safely ignored.
- Feedback and outcomes influence ranking transparently.
- Reddit can be replaced or supplemented without changing the opportunity engine.
- Azure OpenAI and database implementations can evolve behind clear interfaces.
- The full demo remains understandable and reliable.

---

## 25. Initial Technical Decisions

These decisions should also be copied into `DECISIONS.md`.

1. **React + Vite + TypeScript** for a fast, strongly typed frontend.
2. **ASP.NET Core** because the hackathon emphasizes Microsoft technologies and Azure integration.
3. **Azure OpenAI** for pain-point extraction and opportunity reasoning.
4. **SQLite** for MVP development speed.
5. **Reddit** as the first public discussion source.
6. **Structured JSON AI responses** instead of free-form text.
7. **Human approval before any external action.**
8. **One builder and no authentication** for the initial demo.
9. **Simple transparent learning scores** instead of custom ML.
10. **Manual discussion analysis endpoint** as a reliable demo fallback.
