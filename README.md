# vslhq26-argos-hound

# ArgosHound

One-sentence description of what your project does.

## Team

- **Solo**
- **Members:**
  - Nikhar Khamesra (@NIK710)

## Category

- **Primary:** AI agent/workflow automation
- Secondary (optional): Azure OpenAI/LLM app

## What it does

Two to four sentences describing the problem you're solving and how your project addresses it.

## Architecture

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

## Tech stack

- Languages:
- Frameworks/libraries:
- AI models/services:
- Hosting:

## Getting started

### Prerequisites

- List required SDKs, runtimes, and accounts
- Note which API keys or secrets the project needs — name them and describe their shape, never paste the values

### Setup

```bash
# Clone the repo
git clone https://github.com/<owner>/<repo>.git
cd <repo>

# Install dependencies
# Configure environment variables (see .env.example)

# Run
```

### Configuration

List the environment variables or config files needed. Do NOT commit secrets. Use `.env.example` to show the shape.

## Demo (required)

- Video file in this repo (preferred): `./demo/demo.mp4` (or similar path)
- Video link (YouTube, Loom, etc.) if not committed to repo:
- Deployed URL (if any):

## Known limitations

Be honest about what doesn't work yet. Judges appreciate this.

## License

MIT