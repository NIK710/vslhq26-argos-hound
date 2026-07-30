# ArgosHound Hackathon Tasks

**Scope:** A two-day end-to-end demo

**Product definition:** [PROJECT.md](./PROJECT.md)

**Technical contract:** [ARCHITECTURE.md](./ARCHITECTURE.md)

## Demo Goal

The demo must prove both opportunity funnels and the shared learning loop:

1. Builder-approved context from an existing AI assistant creates a proposed profile.
2. A discussion matches an existing builder-owned product.
3. A discussion represents a problem the builder can solve while advancing their goals.
4. A campaign event or builder outcome changes a later opportunity score.

Use one seeded builder and a small fixed set of Reddit-style discussions. Live Reddit
access is optional and must not block the demo.

## Demo Scenarios

Prepare these fixtures before implementation:

- **Product opportunity:** students discuss doomscrolling; the builder owns a relevant
  extension or app.
- **Builder opportunity:** a local chess club discusses coordination problems; the
  builder wants hands-on AI engineering experience.
- **No opportunity:** a discussion with no credible fit.

Each fixture must contain a thread URL, subreddit, title, body, and a few comments with
stable IDs and URLs.

## Day 1 — Product Slice

### 1. Project foundation

- [x] Create `frontend/` and `backend/`
- [x] Add a repository `.gitignore`
- [x] Add `.env.example` files without credentials
- [x] Configure frontend and backend dependencies
- [x] Add a backend health endpoint
- [x] Confirm the frontend can call the backend
- [x] Document required environment-variable names

### 2. Builder and product fixtures

- [x] Define the `BuilderProfile` model
- [x] Define the `Product` model
- [x] Seed one builder
- [x] Add current skills, learning goals, interests, and effort preferences
- [x] Add 3–5 products owned by that builder
- [x] Give products explicit capabilities, target users, and destination URLs
- [x] Include direct, adjacent, and deliberately weak product fits

### 3. LLM-assisted profile import

- [x] Define `ProfileImport` and `ProposedBuilderProfile`
- [x] Write a provider-neutral prompt that asks an assistant for relevant builder context
- [x] Accept a pasted ChatGPT, Claude, or other assistant summary
- [x] Treat imported text as untrusted input
- [x] Extract only supported Builder Profile fields
- [x] Show a field-level preview before saving
- [x] Let the builder edit, approve, reject, and delete the proposal
- [x] Do not label identity sign-in as memory access
- [x] Expire or delete raw imported content after approval
- [x] Keep the seeded profile as a demo fallback

### 4. Source evidence

- [x] Define `SourceDiscussion` and `SourceComment`
- [x] Seed the three demo scenarios
- [x] Preserve thread, comment, and community URLs
- [x] Add a manual discussion-ingestion endpoint
- [x] Validate required source fields
- [x] Render a source discussion and its relevant comments in the UI
- [x] Avoid creating a separate person or lead profile

### 5. Structured analysis

- [x] Configure the Azure OpenAI deployment
- [x] Configure the Foundry agent or documented fallback model client
- [x] Put model calls behind an `ILlmAnalysisProvider` interface
- [x] Keep prompts outside application business logic
- [x] Delimit source content as untrusted input
- [x] Define the structured analysis schema
- [x] Extract problem, topic, sentiment, and evidence references
- [x] Return `PRODUCT`, `BUILDER`, or `NONE`
- [x] Return product match type only for product opportunities
- [x] Reference product IDs and capabilities from the supplied catalog
- [x] Include limitations, explanation, suggested action, and confidence
- [x] Reject malformed output and unknown evidence or product IDs
- [x] Add timeout, retry, and user-visible failure behavior

### 6. Product-opportunity flow

- [x] Implement `POST /api/discovery`
- [x] Match the doomscrolling fixture to the seeded product
- [x] Calculate deterministic score factors in the backend
- [x] Persist the opportunity and evidence references
- [x] Implement opportunity list and detail endpoints
- [x] Display source, relevant comments, product match, limitations, and score
- [x] Clearly label inferred problems as inferences
- [x] Verify the model does not invent product capabilities

### 7. Campaign attribution

- [x] Define `CampaignLink` and `EngagementEvent`
- [x] Generate a cryptographically random opportunity-scoped code
- [x] Store the code safely
- [x] Allow only configured destination hosts
- [x] Implement `GET /r/{code}`
- [x] Record an `OPENED` event and redirect
- [x] Display campaign events on the opportunity detail page
- [x] Add a measurement disclosure to the demo destination page
- [x] Confirm the code identifies the campaign, not a source commenter

### Day 1 exit criteria

- [ ] Pasted assistant context produces a reviewable profile proposal
- [ ] The product fixture produces a persisted `PRODUCT` opportunity
- [ ] The report links to exact source evidence
- [ ] The campaign link redirects and records an event
- [ ] No external comment or message is sent

## Day 2 — Builder Slice and Learning

### 8. Builder-opportunity flow

- [x] Match the chess-club fixture against skills, goals, interests, and location
- [x] Return a builder-opportunity subtype
- [x] Explain why the builder can help
- [x] Explain how the opportunity advances a learning or career goal
- [x] Estimate effort and identify limitations
- [x] Suggest multiple safe next steps, such as investigate, interview, or prototype
- [x] Persist and render the `BUILDER` opportunity
- [x] Verify the unrelated fixture returns `NONE`

### 9. Decisions and outcomes

- [x] Define `BuilderDecision`
- [x] Define `Outcome`
- [x] Implement save, dismiss, and pursue actions
- [x] Allow an optional decision reason
- [x] Record product outcomes such as activation or purchase
- [x] Record builder outcomes such as learning value or prototype completed
- [x] Add portfolio, collaboration, interview, and contract outcome types
- [x] Display the event and outcome timeline

### 10. Transparent learning

- [x] Implement explicit scoring-factor weights
- [x] Keep model confidence separate from the final score
- [x] Aggregate history by source, community, topic, product, and opportunity type
- [ ] Weight a click less than activation or conversion *(deferred for the MVP)*
- [x] Incorporate saved, dismissed, and pursued decisions
- [x] Incorporate reported learning and career outcomes
- [x] Implement `GET /api/learning/summary`
- [x] Display score factors and relevant history in the UI
- [x] Re-run or rescore a fixture after adding an outcome
- [x] Demonstrate a visible, explainable score change

### 11. Core UI

- [ ] Add a concise product explanation
- [ ] Add the builder-profile summary
- [ ] Add opportunity-feed filters for `PRODUCT` and `BUILDER`
- [ ] Create an opportunity card with type, problem, score, and suggested action
- [ ] Create an opportunity detail view
- [ ] Link to the original thread and exact comments
- [ ] Show product or capability match
- [ ] Show confidence separately from final score
- [ ] Show limitations and privacy/outreach risk
- [ ] Add loading, empty, and error states
- [ ] Make the primary demo view responsive

### 12. Reliability checks

- [ ] Reject empty discussions
- [ ] Test vague and contradictory discussions
- [ ] Test invalid structured model output
- [ ] Test source text containing prompt-injection instructions
- [ ] Test prompt-injection instructions inside imported assistant context
- [ ] Test unknown product and evidence IDs
- [ ] Bound confidence and score values
- [ ] Test expired or invalid campaign codes
- [ ] Test redirect allowlisting
- [ ] Confirm secrets and campaign codes are absent from logs
- [ ] Confirm the UI never implies that an inference is a verified fact
- [ ] Confirm profile fields do not change until import approval

### Day 2 exit criteria

- [ ] The builder fixture produces a persisted `BUILDER` opportunity
- [ ] The weak fixture produces `NONE`
- [ ] A builder decision or outcome is recorded
- [ ] History changes a score through visible factors
- [ ] Both demo funnels work from the UI

## Final Demo Checklist

- [ ] One builder owns every product in the catalog
- [ ] The assistant-context import requires preview and approval
- [ ] The product-opportunity example works reliably
- [ ] The builder-opportunity example works reliably
- [ ] The no-opportunity example works reliably
- [ ] Every recommendation cites actual source evidence
- [ ] Campaign attribution works without person-level tracking
- [ ] The learning change is visible and explainable
- [ ] Outreach is suggested but never automatically sent
- [ ] The deployed application works from a clean browser
- [ ] Azure credentials are not exposed
- [ ] A 2–3 minute demo script and backup screenshots are ready

## Documentation Before Presentation

- [ ] Expand `README.md` with the problem, solution, and exact MVP
- [ ] Link `PROJECT.md`, `ARCHITECTURE.md`, and `TASKS.md`
- [ ] Add local frontend and backend setup commands
- [ ] List required environment-variable names
- [ ] Explain seeded versus live source data
- [ ] Explain Foundry and Azure OpenAI responsibilities
- [ ] Explain model providers versus assistant-context providers
- [ ] Describe campaign measurement and the human-approval boundary
- [ ] Add screenshots after the UI is stable

## Stretch Goals

Only begin these after the complete demo works.

### Discovery

- [ ] Add a compliant live Reddit connector
- [ ] Add scheduled scouting runs
- [ ] Add deduplication across repeated discussions
- [ ] Add embeddings for candidate retrieval
- [ ] Add community and topic preferences

### LLM integrations

- [ ] Add an Anthropic analysis-provider implementation
- [ ] Support a narrowly selected ChatGPT data export
- [ ] Support Claude's user-controlled memory export format
- [ ] Add direct provider context connections only when officially supported
- [ ] Add least-privilege OAuth, revocation, and sync auditing
- [ ] Add periodic, user-approved profile refresh

### Outreach and destinations

- [ ] Generate editable outreach drafts
- [ ] Add builder-controlled product landing pages
- [ ] Add builder-controlled portfolio pages
- [ ] Add richer first-party activation events

### Builder learning

- [ ] Add effort estimates and time budgets
- [ ] Add skill-progression tracking
- [ ] Recommend project milestones
- [ ] Add weekly opportunity and outcome reports

### Production readiness

- [ ] Authentication and workspace authorization
- [ ] Background discovery jobs
- [ ] Idempotency and ingestion checkpoints
- [ ] Source-content retention and deletion jobs
- [ ] Connector-specific policy review
- [ ] Monitoring, alerting, and audit logs
- [ ] Comprehensive automated tests
- [ ] Production database and deployment design

## Explicitly Deferred

- Automated comments or private messages
- Tracking or profiling individual commenters
- Cross-platform identity resolution
- Sensitive-trait inference
- Multi-builder product matching
- Assuming ChatGPT or Claude sign-in provides memory access
- Background synchronization without explicit provider support and user consent
- Device fingerprinting or cross-site tracking
- Model fine-tuning on source-platform content
