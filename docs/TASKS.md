# ArgosHound — TASKS.md

This checklist is scoped for a 2-day hackathon.

The goal is to build a working end-to-end demo, not a production-ready platform. Tasks that are not required for the demo are listed under Stretch Goals.

---

# MVP Demo Flow

By the end of the hackathon, a user should be able to:

1. Submit a problem, need, or opportunity.
2. Have ArgosHound analyze the request.
3. Search through a small set of builder products.
4. Identify which builder’s own product is the best match.
5. Recommend:

   * using the existing product,
   * using an adjacent product,
   * or adding a small feature to make the product fit.
6. View a clear AI-generated match explanation.

---

# Day 1 — Core System

## 1. Project Setup

* [ ] Confirm frontend and backend folders are committed to GitHub
* [ ] Add `.gitignore`
* [ ] Create `.env` files
* [ ] Add `.env.example`
* [ ] Configure frontend dependencies
* [ ] Configure backend dependencies
* [ ] Add backend health-check endpoint
* [ ] Confirm frontend can call the backend

---

## 2. Define Demo Data

Use a small fixed dataset instead of building full user accounts or product management.

* [ ] Define the builder data structure
* [ ] Define the product data structure
* [ ] Create 5–10 example builders
* [ ] Give each builder at least one existing product
* [ ] Add product descriptions
* [ ] Add product capabilities
* [ ] Add target users
* [ ] Add product links or placeholder links
* [ ] Include products that demonstrate exact and adjacent matches

Example product fields:

```json
{
  "builder_name": "Example Builder",
  "product_name": "StudySprint",
  "description": "A tool that generates short daily study reviews.",
  "capabilities": [
    "content summarization",
    "quiz generation",
    "scheduled delivery"
  ],
  "target_users": [
    "college students"
  ]
}
```

---

## 3. Opportunity Submission

* [ ] Create an opportunity input form
* [ ] Add a title field
* [ ] Add a problem or need description field
* [ ] Add an optional target-user field
* [ ] Add a submit button
* [ ] Validate that the description is not empty
* [ ] Send the opportunity to the backend

Do not build:

* user accounts,
* file uploads,
* categories,
* advanced filters,
* or multi-step onboarding.

---

## 4. Azure OpenAI Integration

* [ ] Create or confirm the Azure OpenAI resource
* [ ] Add the Azure OpenAI endpoint
* [ ] Add the deployment name
* [ ] Store credentials in environment variables
* [ ] Test a basic model request
* [ ] Create a reusable Azure OpenAI client
* [ ] Handle API errors
* [ ] Request structured JSON output

---

## 5. Opportunity Analysis

Create a model call that converts the user’s request into structured information.

* [ ] Extract the core problem
* [ ] Extract the target user
* [ ] Extract required capabilities
* [ ] Extract optional capabilities
* [ ] Identify the likely product category
* [ ] Identify key constraints
* [ ] Return structured JSON
* [ ] Validate the returned JSON

Example output:

```json
{
  "problem": "Students forget lecture content between classes.",
  "target_users": [
    "college students"
  ],
  "required_capabilities": [
    "content summarization",
    "quiz generation"
  ],
  "optional_capabilities": [
    "scheduled reminders"
  ],
  "constraints": [
    "must take less than five minutes"
  ]
}
```

---

## 6. Product Analysis

For the hackathon, product information may already be structured in the demo dataset.

* [ ] Read each product’s description
* [ ] Identify its current capabilities
* [ ] Identify its target users
* [ ] Identify adjacent use cases
* [ ] Identify features that could be added quickly
* [ ] Create a consistent product-analysis structure

Avoid separate model calls for every field unless necessary.

---

## 7. Matching Logic

* [ ] Compare the opportunity requirements with each product
* [ ] Calculate a simple compatibility score
* [ ] Identify matching capabilities
* [ ] Identify missing capabilities
* [ ] Identify target-user overlap
* [ ] Rank the products
* [ ] Return the top three matches
* [ ] Select the strongest overall recommendation

The score does not need to be mathematically advanced. A model-generated score or simple weighted score is enough for the demo.

Suggested factors:

* capability overlap,
* target-user overlap,
* amount of additional work required,
* usefulness of the existing product,
* strength of the adjacent use case.

---

## 8. Recommendation Types

Each match should be assigned one recommendation type.

* [ ] `EXISTING_PRODUCT`
* [ ] `ADJACENT_USE`
* [ ] `SMALL_EXTENSION`
* [ ] `WEAK_MATCH`

Definitions:

### Existing Product

The builder’s product can already solve the opportunity with little or no modification.

### Adjacent Use

The product was not originally made for this exact problem, but its existing capabilities can still provide a useful solution.

### Small Extension

The builder can serve the opportunity by adding one small, realistic feature to their existing product.

### Weak Match

The product would require major changes and should not be recommended.

---

## 9. Match Explanation

* [ ] Generate a one-paragraph opportunity summary
* [ ] Explain why the top product matches
* [ ] List matching capabilities
* [ ] List missing capabilities
* [ ] Explain whether the match is exact or adjacent
* [ ] Suggest one small feature extension when relevant
* [ ] Explain why the recommendation is practical
* [ ] Add a confidence score
* [ ] Keep the explanation concise enough for the demo

Example:

> StudySprint is a strong adjacent match because it already summarizes educational content and generates review questions. Although it was designed for students rather than employee onboarding, the builder could support this opportunity by adding document uploads and team-specific content collections.

---

# Day 1 — Azure AI Foundry Agent

## 10. Foundry Agent Setup

* [ ] Create an Azure AI Foundry project
* [ ] Create the ArgosHound agent
* [ ] Connect the Azure OpenAI model
* [ ] Write the agent instructions
* [ ] Define the expected output structure
* [ ] Test the agent using one example opportunity
* [ ] Connect the backend to the agent

---

## 11. Agent Workflow

The agent should coordinate the matching process rather than only produce a single generic response.

* [ ] Receive the opportunity
* [ ] Analyze the opportunity
* [ ] Review the available builder products
* [ ] compare candidate products
* [ ] rank the candidates
* [ ] select the recommendation type
* [ ] generate the final match report
* [ ] return structured results to the backend

For the MVP, this can still use one agent with several reasoning steps. A multi-agent system is not required.

---

## 12. End-to-End Backend Test

* [ ] Submit a sample opportunity through the API
* [ ] Confirm the backend calls the Foundry agent
* [ ] Confirm the agent receives the product dataset
* [ ] Confirm ranked matches are returned
* [ ] Confirm malformed outputs are handled
* [ ] Confirm the final response matches the frontend data structure

---

# Day 2 — Frontend and Demo

## 13. Basic Interface

* [ ] Create a simple ArgosHound landing page
* [ ] Add the product name
* [ ] Add a one-sentence explanation
* [ ] Add the opportunity form
* [ ] Add loading state
* [ ] Add error state
* [ ] Add results section

Do not spend significant time on animations or a complex design system.

---

## 14. Results Page

* [ ] Display the opportunity summary
* [ ] Display the top recommended product
* [ ] Display the builder name
* [ ] Display the product description
* [ ] Display the compatibility score
* [ ] Display the recommendation type
* [ ] Display matching capabilities
* [ ] Display missing capabilities
* [ ] Display the suggested product extension
* [ ] Display the AI explanation
* [ ] Display two alternative matches

---

## 15. Match Cards

Each match card should include:

* [ ] Builder name
* [ ] Product name
* [ ] Match score
* [ ] Recommendation type
* [ ] Short explanation
* [ ] Matching capabilities
* [ ] Suggested next step

---

## 16. Demo Examples

Prepare at least three opportunities that produce different recommendation types.

### Demo 1 — Existing Product

* [ ] Create an opportunity that closely matches an existing product
* [ ] Confirm the system returns `EXISTING_PRODUCT`

### Demo 2 — Adjacent Use

* [ ] Create an opportunity that uses an existing product in a different market
* [ ] Confirm the system returns `ADJACENT_USE`

### Demo 3 — Small Extension

* [ ] Create an opportunity that needs one additional feature
* [ ] Confirm the system returns `SMALL_EXTENSION`

---

## 17. Improve Output Reliability

* [ ] Test with vague opportunity descriptions
* [ ] Test with detailed opportunity descriptions
* [ ] Test with an opportunity that has no strong match
* [ ] Prevent the model from inventing product capabilities
* [ ] Require explanations to reference actual product data
* [ ] Ensure scores remain within the expected range
* [ ] Add fallback behavior for invalid model responses

---

## 18. UI Polish

Only complete these after the full demo works.

* [ ] Improve spacing
* [ ] Improve typography
* [ ] Add recommendation-type badges
* [ ] Add simple score visualization
* [ ] Make the page responsive
* [ ] Add example opportunity buttons
* [ ] Add a reset button

---

## 19. Documentation

* [ ] Update `README.md`
* [ ] Add a short problem statement
* [ ] Add the ArgosHound solution
* [ ] Describe the three recommendation types
* [ ] Explain how Azure OpenAI is used
* [ ] Explain how Azure AI Foundry is used
* [ ] Add local setup instructions
* [ ] Add required environment variables
* [ ] Add screenshots after the UI is finished
* [ ] Link `PROJECT.md`
* [ ] Link `ARCHITECTURE.md`

---

## 20. Demo Preparation

* [ ] Write a 2–3 minute demo script
* [ ] Choose the strongest example opportunity
* [ ] Keep backup screenshots
* [ ] Record a backup demo video if time permits
* [ ] Confirm the application works from a clean browser
* [ ] Confirm Azure credentials are not exposed
* [ ] Confirm the deployed version works
* [ ] Practice explaining the adjacent-product concept
* [ ] Practice explaining why the Foundry agent is necessary
* [ ] Practice explaining the value to builders

---

# Final MVP Checklist

The MVP is complete when all of the following work:

* [ ] A user can enter an opportunity
* [ ] The backend receives the request
* [ ] Azure AI analyzes the opportunity
* [ ] The Foundry agent evaluates builder products
* [ ] The system ranks the products
* [ ] The system recommends the builder’s own product
* [ ] The system distinguishes exact, adjacent, and small-extension matches
* [ ] The frontend displays a clear explanation
* [ ] At least three demo examples work reliably
* [ ] The application is ready to present

---

# Stretch Goals

Only begin these after the complete demo works.

## Product Submission

* [ ] Allow builders to add products through the UI
* [ ] Allow builders to edit products
* [ ] Automatically extract capabilities from product descriptions
* [ ] Accept product website links

## Search Improvements

* [ ] Create embeddings for opportunities
* [ ] Create embeddings for products
* [ ] Add vector similarity search
* [ ] Combine vector similarity with agent reasoning

## Additional Features

* [ ] Generate a suggested outreach message
* [ ] Let users save matches
* [ ] Add match history
* [ ] Add builder profile pages
* [ ] Add product screenshots
* [ ] Add feedback on recommendation quality
* [ ] Export the match report
* [ ] Add more builder products

## Production Work

Do not work on these during the hackathon unless required for deployment.

* [ ] Full authentication
* [ ] Password reset
* [ ] Docker
* [ ] CI/CD
* [ ] Advanced permissions
* [ ] Comprehensive automated testing
* [ ] Monitoring and alerting
* [ ] Payment processing
* [ ] Messaging between users
* [ ] Production-scale database design
