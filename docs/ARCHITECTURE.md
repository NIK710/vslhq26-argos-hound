# ArgosHound Architecture

**Status:** Hackathon MVP

## 1. Overview

ArgosHound is an AI talent agent for builders.

It analyzes online discussions and determines whether they represent:

1. A Product Opportunity for one of the builder's own products.
2. A Builder Opportunity for the builder personally.
3. No meaningful opportunity.

The system combines **Microsoft Foundry Agent Service** for agent orchestration with **Azure OpenAI** for reasoning.

---

## 2. Technology Stack

### Frontend
- React
- Vite
- TypeScript

### Backend
- ASP.NET Core Web API
- Entity Framework Core
- SQLite (MVP)

### AI Platform
- **Microsoft Foundry Agent Service**
  - Agent runtime
  - Tool orchestration
  - Agent state
  - Tracing & evaluation
- **Azure OpenAI**
  - GPT model powering the ArgosHound agent's reasoning
  - Structured opportunity analysis
  - Product and builder matching

### Initial Source
- Reddit

---

## 3. High-Level Architecture

```text
                  React Frontend
                         │
                         ▼
                ASP.NET Core API
                         │
                         ▼
          Microsoft Foundry Agent Service
                         │
              (Agent Orchestration)
                         │
                         ▼
          Azure OpenAI GPT Deployment
                         │
         Decides which backend tools to call
                         │
      ┌──────────────────┼──────────────────┐
      ▼                  ▼                  ▼
 Builder Tool      Product Tool      Learning Tool
      │                  │                  │
      └──────────────┬───┴──────────────────┘
                     ▼
              Reddit Search Tool
                     │
                     ▼
          SQLite / Application Data
```

The ASP.NET backend owns all business logic, validation, persistence, and tool implementations. The Foundry agent decides **what information it needs**; the backend decides **how that information is retrieved and stored**.

---

## 4. Agent Workflow

```text
Discovery Request
        │
        ▼
Retrieve Candidate Discussions
        │
        ▼
Start Foundry Agent
        │
        ├── Read Builder Profile
        ├── Read Builder Products
        ├── Read Learning Context
        └── Read Reddit Discussion
        │
        ▼
Azure OpenAI reasons about:
    • Pain point
    • Product opportunity?
    • Builder opportunity?
    • Product evolution?
        │
        ▼
Structured JSON Response
        │
        ▼
Backend Validation
        │
        ▼
Scoring + Persistence
        │
        ▼
Frontend
```

---

## 5. Core Features

### Feature 1 — Product Opportunity Discovery

Evaluates **only products owned by the builder**.

Possible outcomes:

- Direct Match
- Adjacent Match
- Product Evolution (small feature or enhancement)

Returns:

- Matched product
- Match type
- Reasoning
- Product limitations
- Suggested enhancement
- Suggested action
- Confidence

### Feature 2 — Builder Opportunity Discovery

If no owned product is a good fit, determine whether the builder can personally solve the problem.

Examples:

- Freelance
- Consulting
- Collaboration
- Open source
- Startup idea
- Research

### Feature 3 — Continuous Learning

Stores:

- Feedback
- Outcomes
- Successful communities
- Successful products
- Successful opportunity types

The backend adjusts future ranking using transparent scoring. The agent receives summarized learning context during future analyses.

---

## 6. Agent Tools

- Builder Profile Tool
- Product Catalog Tool
- Reddit Search Tool
- Learning Context Tool
- Opportunity History Tool

These are implemented in ASP.NET Core and exposed to the Foundry agent as controlled function tools.

---

## 7. Backend Components

- DiscoveryController
- BuilderController
- OpportunityController
- FeedbackController

Services:

- DiscoveryService
- FoundryAgentService
- RedditService
- LearningService
- ScoringService

---

## 8. Core Models

- Builder
- Product
- SourceDiscussion
- Opportunity
- Feedback
- Outcome

---

## 9. API

```
GET  /api/builder
PUT  /api/builder

POST /api/discovery
POST /api/discovery/analyze

GET  /api/opportunities
GET  /api/opportunities/{id}

POST /api/opportunities/{id}/feedback
POST /api/opportunities/{id}/outcomes

GET  /api/learning/summary
```

---

## 10. Repository

```text
argoshound/
├── frontend/
├── backend/
│   ├── Controllers/
│   ├── Agents/
│   │   └── FoundryAgentService.cs
│   ├── Tools/
│   ├── Services/
│   ├── Models/
│   ├── Data/
│   └── Prompts/
├── docs/
│   ├── PROJECT.md
│   ├── ARCHITECTURE.md
│   ├── TASKS.md
└── README.md
```

---

## 11. Design Principles

- Foundry orchestrates; Azure OpenAI reasons.
- Backend owns business logic and validation.
- Products are always the builder's own products.
- Every recommendation must include an explanation.
- Learning improves ranking over time without requiring model fine-tuning.
- Human approval is required before any external action.

---

## 12. MVP Success

The MVP is complete when a builder can:

1. Create a profile and products.
2. Analyze a Reddit discussion.
3. Receive a Product Opportunity, Builder Opportunity, or No Opportunity.
4. View reasoning and suggested actions.
5. Submit feedback.
6. See future recommendations improve from learning.
