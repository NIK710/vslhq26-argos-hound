# ArgosHound Hackathon Tasks

**Scope:** A two-day end-to-end demo

**Product definition:** [PROJECT.md](./PROJECT.md)

**Technical contract:** [ARCHITECTURE.md](./ARCHITECTURE.md)

## Demo Goal

The demo must prove both opportunity funnels and the shared learning loop:

1. A discussion matches an existing builder-owned product.
2. A discussion represents a problem the builder can solve while advancing their goals.
3. A campaign event or builder outcome changes a later opportunity score.

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

- [ ] Create `frontend/` and `backend/`
- [ ] Add a repository `.gitignore`
- [ ] Add `.env.example` files without credentials
- [ ] Configure frontend and backend dependencies
- [ ] Add a backend health endpoint
- [ ] Confirm the frontend can call the backend
- [ ] Document required environment-variable names

### 2. Builder and product fixtures

- [ ] Define the `BuilderProfile` model
- [ ] Define the `Product` model
- [ ] Seed one builder
- [ ] Add current skills, learning goals, interests, and effort preferences
- [ ] Add 3–5 products owned by that builder
- [ ] Give products explicit capabilities, target users, and destination URLs
- [ ] Include direct, adjacent, and deliberately weak product fits

### 3. Source evidence

- [ ] Define `SourceDiscussion` and `SourceComment`
- [ ] Seed the three demo scenarios
- [ ] Preserve thread, comment, and community URLs
- [ ] Add a manual discussion-ingestion endpoint
- [ ] Validate required source fields
- [ ] Render a source discussion and its relevant comments in the UI
- [ ] Avoid creating a separate person or lead profile

### 4. Structured analysis

- [ ] Configure the Azure OpenAI deployment
- [ ] Configure the Foundry agent or documented fallback model client
- [ ] Keep prompts outside application business logic
- [ ] Delimit source content as untrusted input
- [ ] Define the structured analysis schema
- [ ] Extract problem, topic, sentiment, and evidence references
- [ ] Return `PRODUCT`, `BUILDER`, or `NONE`
- [ ] Return product match type only for product opportunities
- [ ] Reference product IDs and capabilities from the supplied catalog
- [ ] Include limitations, explanation, suggested action, and confidence
- [ ] Reject malformed output and unknown evidence or product IDs
- [ ] Add timeout, retry, and user-visible failure behavior

### 5. Product-opportunity flow

- [ ] Implement `POST /api/discovery`
- [ ] Match the doomscrolling fixture to the seeded product
- [ ] Calculate deterministic score factors in the backend
- [ ] Persist the opportunity and evidence references
- [ ] Implement opportunity list and detail endpoints
- [ ] Display source, relevant comments, product match, limitations, and score
- [ ] Clearly label inferred problems as inferences
- [ ] Verify the model does not invent product capabilities

### 6. Campaign attribution

- [ ] Define `CampaignLink` and `EngagementEvent`
- [ ] Generate a cryptographically random opportunity-scoped code
- [ ] Store the code safely
- [ ] Allow only configured destination hosts
- [ ] Implement `GET /r/{code}`
- [ ] Record an `OPENED` event and redirect
- [ ] Display campaign events on the opportunity detail page
- [ ] Add a measurement disclosure to the demo destination page
- [ ] Confirm the code identifies the campaign, not a source commenter

### Day 1 exit criteria

- [ ] The product fixture produces a persisted `PRODUCT` opportunity
- [ ] The report links to exact source evidence
- [ ] The campaign link redirects and records an event
- [ ] No external comment or message is sent

## Day 2 — Builder Slice and Learning

### 7. Builder-opportunity flow

- [ ] Match the chess-club fixture against skills, goals, interests, and location
- [ ] Return a builder-opportunity subtype
- [ ] Explain why the builder can help
- [ ] Explain how the opportunity advances a learning or career goal
- [ ] Estimate effort and identify limitations
- [ ] Suggest multiple safe next steps, such as investigate, interview, or prototype
- [ ] Persist and render the `BUILDER` opportunity
- [ ] Verify the unrelated fixture returns `NONE`

### 8. Decisions and outcomes

- [ ] Define `BuilderDecision`
- [ ] Define `Outcome`
- [ ] Implement save, dismiss, and pursue actions
- [ ] Allow an optional decision reason
- [ ] Record product outcomes such as activation or purchase
- [ ] Record builder outcomes such as learning value or prototype completed
- [ ] Add portfolio, collaboration, interview, and contract outcome types
- [ ] Display the event and outcome timeline

### 9. Transparent learning

- [ ] Implement explicit scoring-factor weights
- [ ] Keep model confidence separate from the final score
- [ ] Aggregate history by source, community, topic, product, and opportunity type
- [ ] Weight a click less than activation or conversion
- [ ] Incorporate saved, dismissed, and pursued decisions
- [ ] Incorporate reported learning and career outcomes
- [ ] Implement `GET /api/learning/summary`
- [ ] Display score factors and relevant history in the UI
- [ ] Re-run or rescore a fixture after adding an outcome
- [ ] Demonstrate a visible, explainable score change

### 10. Core UI

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

### 11. Reliability checks

- [ ] Reject empty discussions
- [ ] Test vague and contradictory discussions
- [ ] Test invalid structured model output
- [ ] Test source text containing prompt-injection instructions
- [ ] Test unknown product and evidence IDs
- [ ] Bound confidence and score values
- [ ] Test expired or invalid campaign codes
- [ ] Test redirect allowlisting
- [ ] Confirm secrets and campaign codes are absent from logs
- [ ] Confirm the UI never implies that an inference is a verified fact

### Day 2 exit criteria

- [ ] The builder fixture produces a persisted `BUILDER` opportunity
- [ ] The weak fixture produces `NONE`
- [ ] A builder decision or outcome is recorded
- [ ] History changes a score through visible factors
- [ ] Both demo funnels work from the UI

## Final Demo Checklist

- [ ] One builder owns every product in the catalog
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
- Device fingerprinting or cross-site tracking
- Model fine-tuning on source-platform content
