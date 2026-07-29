# ArgosHound

**Status:** Hackathon MVP

**Product:** A personalized opportunity scout for builders

**Initial stack:** React + Vite, ASP.NET Core, Microsoft Foundry, Azure OpenAI, SQLite

## Vision

AI has made building software easier, but many builders have products with few or no
users. At the same time, people describe unmet needs every day in public online
communities.

ArgosHound connects those two sides. It scouts public discussions, identifies credible
problems, and recommends opportunities personalized to one builder's products, skills,
interests, goals, and previous outcomes.

ArgosHound is not a marketplace that compares products from unrelated builders. Each
workspace represents one builder and the products and capabilities that builder owns.

## Product Principles

- Find evidence before proposing an opportunity.
- Personalize every recommendation to the builder.
- Explain why an opportunity is relevant and cite its source.
- Treat model output as a recommendation, not a fact.
- Require human approval before any comment, message, or other external action.
- Track campaigns and outcomes, not people.
- Learn through transparent scoring before introducing complex machine learning.
- Collect and retain only the source data required to support the opportunity.

## The Two Opportunity Funnels

### 1. Product Opportunities

ArgosHound finds public discussions where an existing builder-owned product may solve
an expressed or reasonably inferred problem.

Example:

> Several students in a college subreddit discuss losing hours to doomscrolling.

The builder owns a doomscroll-interruption extension. ArgosHound reports the thread,
highlights the relevant subthread or comments, explains the match, and offers a
campaign-specific link to the product.

A product opportunity can be:

- `DIRECT`: the product already addresses the problem.
- `ADJACENT`: existing capabilities help in a related use case.
- `SMALL_EXTENSION`: a small, realistic enhancement would make the product useful.

If the fit is weak or would require a different product, ArgosHound should not recommend
it as a product opportunity.

### 2. Builder Opportunities

When no existing product is a good fit, ArgosHound asks whether solving the problem
would be valuable for the builder personally.

It considers:

- Current skills
- Skills the builder wants to learn
- Problem-area interests
- Preferred opportunity types
- Time and effort preferences
- Optional geographic relevance
- Previous choices and outcomes

Example:

> A local chess club is struggling with event scheduling, attendance, pairings, and
> volunteer communication.

For a computer-science student learning AI engineering, ArgosHound might recommend
interviewing the organizers, building a prototype, contributing it to the club, or
turning the recurring need into a general product.

Builder-opportunity types include:

- Freelance or consulting work
- A portfolio or learning project
- Open-source contribution
- Collaboration
- Community service
- Research
- Startup or product exploration

## Unified Decision Flow

```text
Public discussion
        |
        v
Is there a credible problem, need, or request?
        |
        v
Does one of this builder's products meaningfully address it?
        | Yes
        +------> PRODUCT opportunity
        |
        | No
        v
Can this builder address it in a way that advances their goals?
        | Yes
        +------> BUILDER opportunity
        |
        | No
        v
NONE
```

The top-level opportunity type is:

- `PRODUCT`
- `BUILDER`
- `NONE`

Product match type is a separate, nullable field. This prevents `ADJACENT` or
`SMALL_EXTENSION` from being confused with the top-level decision.

## Opportunity Report

Every surfaced opportunity should answer:

- What problem was detected?
- What source evidence supports that conclusion?
- Who is discussing it, without creating a separate profile about them?
- Why does it fit this builder?
- Which product or capabilities match?
- What are the limitations and risks?
- What action could the builder take?
- How confident is the system?

An opportunity report includes:

- Source platform, community, thread URL, title, and timestamp
- Relevant subthread or comment URLs and short excerpts
- Detected problem, topic, and sentiment
- Opportunity type and optional subtype
- Matched product or builder capabilities
- Evidence-based explanation
- Confidence and transparent opportunity score
- Suggested next step
- Campaign link when appropriate

## Outreach Boundary

ArgosHound may suggest an outreach message or action, but it does not automatically
post, comment, or send a private message in the MVP.

The builder must review the source and approve any external action. Relevant public
comments may be linked as evidence, but ArgosHound should avoid persistent dossiers,
sensitive-trait inference, or person-level behavioral tracking.

Community rules and source-platform terms always apply. A relevant public response is
generally preferable to unsolicited private outreach.

## Campaign Links and Attribution

ArgosHound can generate a random, opportunity-scoped campaign code:

```text
https://example.app/r/7Gk92P
```

The code identifies the opportunity or campaign, not a named commenter. Its destination
may be a product page, signup page, portfolio, project page, or other builder-controlled
page.

The MVP can record:

- Link opened
- Product or portfolio explored
- Signup
- Activation
- Contact initiated
- Purchase or contract, when manually reported

Destination pages should disclose measurement, avoid invasive fingerprinting, and
collect only the events needed for attribution.

## Continuous Learning

ArgosHound learns from market outcomes and builder outcomes.

### Market signals

- Source platform and community
- Topic and detected sentiment
- Matched product
- Match type
- Campaign opened
- Signup, activation, or purchase

### Builder signals

- Viewed, saved, dismissed, or pursued
- Contacted the source
- Built a prototype or completed a project
- Reported learning value
- Added work to a portfolio
- Received a collaboration, interview, contract, or other career outcome
- Requested more or fewer opportunities like it

The MVP uses explicit weights and summarized history. It does not train a model on
source-platform content.

An initial ranking can combine:

```text
profile fit
+ evidence strength
+ product or skill relevance
+ source and topic history
+ builder preference history
+ previous outcome history
- estimated effort
- outreach or privacy risk
```

The UI should expose the important factors so the builder can understand why the score
changed.

## Builder Profile

The profile contains:

- Name
- Products and their capabilities
- Current skills
- Skills the builder wants to develop
- Interests and preferred problem areas
- Goals
- Preferred opportunity types
- Optional location or geographic radius
- Effort and time preferences
- Summarized decisions and outcomes

## MVP Scope

The hackathon MVP proves that ArgosHound can:

1. Represent one builder, their products, skills, and goals.
2. Analyze a small set of seeded or pasted Reddit discussions.
3. Return `PRODUCT`, `BUILDER`, or `NONE`.
4. Show source evidence, reasoning, confidence, and a suggested action.
5. Generate an opportunity-scoped campaign link.
6. Record simulated or real first-party engagement events.
7. Record a builder decision or outcome.
8. Demonstrate a visible ranking change from that history.

Live, continuous Reddit ingestion is optional for the hackathon. The architecture must
allow a compliant connector to replace seeded data later.

## Out of Scope

- Automated comments or private messages
- Person-level lead profiles
- Sensitive-trait inference
- Multi-builder marketplace matching
- Authentication and team permissions
- Payments
- Browser extensions
- Fully autonomous agents
- Cross-site tracking or device fingerprinting
- Complex machine learning or model fine-tuning
- Production-scale ingestion

## Future Sources and Capabilities

- YouTube comments
- GitHub Issues and Discussions
- Hacker News
- Stack Overflow
- Discord communities with appropriate authorization
- Product Hunt
- Scheduled scouting reports
- Builder-controlled outreach integrations
- Additional attribution and outcome integrations
- Personalized portfolio or product landing pages

## Success Criteria

The demo is successful when a viewer can:

1. See a product opportunity supported by a real or seeded discussion.
2. See a builder opportunity matched to skills and learning goals.
3. Inspect the exact evidence behind both recommendations.
4. Understand the proposed next action and its limitations.
5. Open a campaign link and observe an attributed event.
6. Submit a builder decision or outcome.
7. See that event affect a later opportunity ranking.

## Elevator Pitch

ArgosHound is an AI opportunity scout for builders. It finds public conversations where
an existing product could gain a user or where the builder could solve a valuable
problem, then learns from which communities, topics, projects, and actions produce real
customer and career outcomes.
