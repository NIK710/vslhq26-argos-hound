# ArgosHound

**Tech Stack:** React + Vite · ASP.NET Core · Azure OpenAI · SQLite (Azure Cosmos DB later)

---

# Vision

ArgosHound is an AI talent agent for builders.

Instead of searching for jobs or generating generic leads, ArgosHound continuously scans online communities, recognizes hidden opportunities, and recommends the highest value actions based on a builder's skills, products, interests, and historical success.

The long-term goal is to become an AI representative that works on behalf of a builder: finding customers, discovering opportunities, and learning how to make increasingly better recommendations over time.

---

# Inspiration

The name **ArgosHound** comes from Argos in *The Odyssey*.

Argos was the only one who recognized Odysseus despite his disguise.

Likewise, ArgosHound recognizes opportunities hidden inside conversations that most people overlook.

The "Hound" represents relentlessly tracking opportunities across the internet.

---

# Core Philosophy

Most people don't explicitly ask for products, employees, founders, or collaborators.

Instead, they describe problems.

ArgosHound's job is to understand those problems and determine whether they represent an opportunity for the builder.

Every discussion is treated as a potential opportunity.

---

# The Three Core Features

Everything in ArgosHound revolves around three connected capabilities powered by a shared Builder Profile.

---

## Feature 1 — Product Opportunity Discovery

### Purpose

Identify conversations where the builder has an existing product that already solves someone's problem.

Example:

A Reddit user writes:

> "I spend way too much time scrolling YouTube."

ArgosHound recognizes:

- Pain point
- Matching product
- Confidence
- Reasoning
- Suggested outreach

Goal:

Help builders naturally discover potential customers instead of relying on traditional outbound marketing.

Questions ArgosHound asks:

- What problem is this person experiencing?
- Does one of my products solve it?
- Should I engage?
- Why is this a good opportunity?

Output:

- Product recommendation
- Suggested action
- Confidence score
- Explanation

---

## Feature 2 — Builder Opportunity Discovery

Not every problem should become a customer.

Sometimes the opportunity is for the builder themselves.

ArgosHound should recognize opportunities including:

- Freelance work
- Consulting
- Open source contributions
- Startup ideas
- Collaborations
- Research
- Community involvement

Example:

A founder posts:

> "Looking for someone experienced with React and Firebase."

ArgosHound recognizes:

This is not a product opportunity.

Instead, it matches the builder's skills and recommends pursuing the opportunity.

Questions ArgosHound asks:

- Can the builder personally solve this?
- Is this worth pursuing?
- How valuable is it?
- Why does it fit?

Output:

- Opportunity type
- Confidence
- Reasoning
- Suggested action

---

## Feature 3 — Continuous Learning

This is the long term differentiator.

ArgosHound should continuously learn what creates successful outcomes for each individual builder.

Rather than optimizing products, it optimizes opportunities.

Marketing learning:

- Which communities convert
- Which messaging works
- Which audiences respond
- Which products succeed

Builder learning:

- Which opportunity types lead to success
- Which technologies repeatedly produce value
- Which communities fit the builder
- Which projects become worthwhile

Examples of tracked outcomes:

Products

- Click
- Signup
- Active user
- Purchase

Builder Opportunities

- Portfolio viewed
- GitHub viewed
- Interview
- Collaboration
- Contract
- PR merged

Initially these outcomes can be entered manually.

No machine learning is required for the MVP.

Simple scoring is sufficient.

---

# Unifying Decision Tree

Every discussion follows the same logic.

```
Internet Discussion
        │
        ▼
Recognize Pain Point
        │
        ▼
Does the builder have an existing product that solves it?
        │
   Yes ───────► Customer Opportunity
        │
        No
        │
        ▼
Can the builder solve it?
        │
   Yes ───────► Builder Opportunity
        │
        No
        │
        ▼
Ignore
```

Every outcome feeds back into the Continuous Learning system.

---

# Builder Profile

Everything revolves around a Builder Profile.

Example attributes:

- Name
- Skills
- Products
- Interests
- Goals
- Preferred technologies
- Industries
- Historical outcomes
- Successful communities

Every recommendation is personalized against this profile.

---

# Core Workflow

```
Online Discussion
        │
        ▼
Extract Pain Point
        │
        ▼
Determine Opportunity Type
        │
        ▼
Match Builder Profile
        │
        ▼
Generate Recommendation
        │
        ▼
Present to User
        │
        ▼
Receive Feedback
        │
        ▼
Learn
```

---

# MVP Scope

The hackathon MVP only needs to prove one thing:

**ArgosHound can recognize opportunities hidden inside online discussions.**

Supported source:

- Reddit

Future sources:

- YouTube
- GitHub Issues
- Hacker News
- Stack Overflow
- Discord
- Product Hunt

---

# System Architecture

```
React + Vite
       │
ASP.NET Core API
       │
 ├── Builder Service
 ├── Opportunity Service
 ├── Reddit Service
 ├── LLM Service
 └── Learning Service
       │
Azure OpenAI
       │
SQLite
```

Future:

- Azure Cosmos DB
- Azure AI Foundry Agents
- Additional data connectors

---

# Backend Responsibilities

The backend owns:

- Reddit ingestion
- Opportunity generation
- AI prompting
- Confidence scoring
- Persistence
- Feedback collection
- Learning

The frontend should remain mostly presentational.

---

# Initial API

```
GET  /builder
PUT  /builder

POST /discover

GET  /opportunities

POST /feedback
```

---

# Initial Models

## Builder

- Id
- Name
- Skills
- Products
- Interests
- Goals

---

## Opportunity

- Id
- Source
- Content
- OpportunityType
- Confidence
- Score
- Explanation
- SuggestedAction

---

## Feedback

- OpportunityId
- Outcome
- Timestamp

---

# Prompt Philosophy

Prompts are source code.

Store prompts separately from application logic.

Example:

```
Prompts/

product_match.txt

builder_match.txt

score.txt

summarize.txt
```

Prompt iteration should never require modifying business logic.

---

# UI

Initial pages:

- Dashboard
- Builder Profile
- Opportunity Feed
- Opportunity Details

The Opportunity Feed is the most important screen.

Every opportunity card should clearly communicate:

- Pain Point
- Opportunity Type
- Confidence
- Reasoning
- Suggested Action

---

# Out of Scope

For now:

- Authentication
- Payments
- OAuth
- Browser extensions
- Discord integration
- GitHub integration
- Dynamic portfolio pages
- Automatic posting
- Email
- Notifications
- Analytics dashboards
- Complex machine learning

---

# Future Roadmap

- GitHub issue discovery
- Personalized outreach generation
- Portfolio optimization
- Daily Argos Report
- Opportunity ranking
- Personalized landing pages
- Autonomous agent workflows
- Continuous Builder Profile evolution
- Additional community connectors

---

# Design Principles

Every feature should answer one question:

> **Does this help the builder discover, prioritize, or act on valuable opportunities?**

If not, it probably does not belong in ArgosHound.

---

# Success Criteria

The MVP is successful if someone can:

1. Create a Builder Profile.
2. Analyze a Reddit discussion.
3. Receive a high-quality recommendation.
4. Understand exactly why it was recommended.
5. Provide feedback.
6. Observe recommendations improving over time.

---

# Elevator Pitch

ArgosHound is an AI talent agent for builders.

It continuously scans online communities, recognizes hidden opportunities, and determines whether they represent:

- a customer for one of your products,
- an opportunity for you personally,
- or neither.

Over time, it learns which opportunities create the most value for you, becoming an increasingly effective representative that works on your behalf.