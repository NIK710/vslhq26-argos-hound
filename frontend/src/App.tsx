import { useEffect, useState } from 'react'
import './App.css'

type ProposedBuilderProfile = {
  name: string
  currentSkills: string[]
  learningGoals: string[]
  interests: string[]
  preferredOpportunityTypes: string[]
  location: string | null
  effortPreferences: string[]
}

type ProfileFieldChange = {
  field: string
  currentValues: string[]
  proposedValues: string[]
}

type ProfileImport = {
  id: string
  provider: 'chatGpt' | 'claude' | 'other'
  status: 'extracted' | 'approved' | 'rejected'
  proposedProfile: ProposedBuilderProfile
  changes: ProfileFieldChange[]
  rawContentDeletedAt: string
}

type SourceComment = {
  id: string
  externalId: string
  parentExternalId: string | null
  body: string
  url: string
  authorHandle: string | null
  publishedAt: string
}

type SourceDiscussion = {
  id: string
  platform: string
  externalId: string
  community: string
  title: string
  body: string
  url: string
  authorHandle: string | null
  publishedAt: string
  retrievedAt: string
  comments: SourceComment[]
}

type OpportunityAnalysis = {
  problem: {
    summary: string
    inferred: boolean
  }
  topic: string
  sentiment: 'negative' | 'mixed' | 'neutral' | 'positive'
  evidenceReferences: string[]
  opportunityType: 'product' | 'builder' | 'none'
  productMatch: {
    productId: string
    productName: string
    matchType: 'direct' | 'adjacent' | 'smallExtension'
    matchedCapabilities: string[]
  } | null
  limitations: string[]
  explanation: string
  suggestedAction: string
  confidence: number
}

type AnalyzeDiscussionResponse = {
  discussionId: string
  analysis: OpportunityAnalysis
  analyzedAt: string
}

type OpportunityScoreFactor = {
  key: string
  label: string
  points: number
  explanation: string
}

type Opportunity = {
  id: string
  discussionId: string
  type: 'product' | 'builder' | 'none'
  productMatchType: 'direct' | 'adjacent' | 'smallExtension' | null
  problem: string
  problemInferred: boolean
  topic: string
  sentiment: 'negative' | 'mixed' | 'neutral' | 'positive'
  matchedProductId: string | null
  matchedProductName: string | null
  matchedCapabilities: string[]
  limitations: string[]
  evidenceReferences: string[]
  explanation: string
  suggestedAction: string
  confidence: number
  score: number
  scoreFactors: OpportunityScoreFactor[]
  createdAt: string
}

type OpportunitySummary = Pick<
  Opportunity,
  | 'id'
  | 'discussionId'
  | 'type'
  | 'problem'
  | 'problemInferred'
  | 'topic'
  | 'score'
  | 'confidence'
  | 'suggestedAction'
  | 'createdAt'
>

type OpportunityDetail = {
  opportunity: Opportunity
  source: SourceDiscussion
  relevantComments: SourceComment[]
}

type BackendStatus =
  | { state: 'checking'; message: string }
  | { state: 'connected'; message: string }
  | { state: 'error'; message: string }

const apiBaseUrl =
  import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5080'

const listFields: Array<{
  key: Exclude<keyof ProposedBuilderProfile, 'name' | 'location'>
  label: string
}> = [
  { key: 'currentSkills', label: 'Current skills' },
  { key: 'learningGoals', label: 'Learning goals' },
  { key: 'interests', label: 'Interests' },
  { key: 'preferredOpportunityTypes', label: 'Preferred opportunity types' },
  { key: 'effortPreferences', label: 'Effort preferences' },
]

async function readApiError(response: Response) {
  try {
    const problem = (await response.json()) as { detail?: string; title?: string }
    return problem.detail ?? problem.title ?? `Request failed (${response.status})`
  } catch {
    return `Request failed (${response.status})`
  }
}

function App() {
  const [backendStatus, setBackendStatus] = useState<BackendStatus>({
    state: 'checking',
    message: 'Checking backend connection…',
  })
  const [provider, setProvider] =
    useState<ProfileImport['provider']>('chatGpt')
  const [content, setContent] = useState('')
  const [profileImport, setProfileImport] = useState<ProfileImport | null>(null)
  const [draft, setDraft] = useState<ProposedBuilderProfile | null>(null)
  const [importError, setImportError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [discussions, setDiscussions] = useState<SourceDiscussion[]>([])
  const [selectedDiscussionId, setSelectedDiscussionId] = useState<
    string | null
  >(null)
  const [sourceError, setSourceError] = useState<string | null>(null)
  const [sourcesLoading, setSourcesLoading] = useState(true)
  const [analysisResult, setAnalysisResult] =
    useState<AnalyzeDiscussionResponse | null>(null)
  const [analysisError, setAnalysisError] = useState<string | null>(null)
  const [isAnalyzing, setIsAnalyzing] = useState(false)
  const [opportunities, setOpportunities] = useState<OpportunitySummary[]>([])
  const [selectedOpportunity, setSelectedOpportunity] =
    useState<OpportunityDetail | null>(null)
  const [opportunityError, setOpportunityError] = useState<string | null>(null)
  const [opportunitiesLoading, setOpportunitiesLoading] = useState(true)
  const [isDiscovering, setIsDiscovering] = useState(false)

  useEffect(() => {
    const controller = new AbortController()

    async function checkBackend() {
      try {
        const response = await fetch(`${apiBaseUrl}/api/health`, {
          signal: controller.signal,
        })

        if (!response.ok) {
          throw new Error(`Health check returned ${response.status}`)
        }

        const message = await response.text()
        setBackendStatus({ state: 'connected', message })
      } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') {
          return
        }

        setBackendStatus({
          state: 'error',
          message:
            error instanceof Error
              ? error.message
              : 'Unable to reach the backend',
        })
      }
    }

    void checkBackend()

    return () => controller.abort()
  }, [])

  useEffect(() => {
    const controller = new AbortController()

    async function loadOpportunities() {
      try {
        const response = await fetch(`${apiBaseUrl}/api/opportunities`, {
          signal: controller.signal,
        })

        if (!response.ok) {
          throw new Error(await readApiError(response))
        }

        setOpportunities((await response.json()) as OpportunitySummary[])
      } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') {
          return
        }

        setOpportunityError(
          error instanceof Error
            ? error.message
            : 'Unable to load opportunities',
        )
      } finally {
        setOpportunitiesLoading(false)
      }
    }

    void loadOpportunities()

    return () => controller.abort()
  }, [])

  useEffect(() => {
    const controller = new AbortController()

    async function loadDiscussions() {
      try {
        const response = await fetch(
          `${apiBaseUrl}/api/sources/discussions`,
          { signal: controller.signal },
        )

        if (!response.ok) {
          throw new Error(await readApiError(response))
        }

        const sources = (await response.json()) as SourceDiscussion[]
        setDiscussions(sources)
        setSelectedDiscussionId(sources[0]?.id ?? null)
      } catch (error) {
        if (error instanceof DOMException && error.name === 'AbortError') {
          return
        }

        setSourceError(
          error instanceof Error ? error.message : 'Unable to load discussions',
        )
      } finally {
        setSourcesLoading(false)
      }
    }

    void loadDiscussions()

    return () => controller.abort()
  }, [])

  async function createImport(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setImportError(null)
    setIsSubmitting(true)

    try {
      const response = await fetch(
        `${apiBaseUrl}/api/builder/profile-imports`,
        {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ provider, content }),
        },
      )

      if (!response.ok) {
        throw new Error(await readApiError(response))
      }

      const created = (await response.json()) as ProfileImport
      setProfileImport(created)
      setDraft(created.proposedProfile)
      setContent('')
    } catch (error) {
      setImportError(
        error instanceof Error ? error.message : 'Unable to import profile',
      )
    } finally {
      setIsSubmitting(false)
    }
  }

  async function saveDraft(): Promise<boolean> {
    if (!profileImport || !draft) return false

    setImportError(null)
    setIsSubmitting(true)

    try {
      const response = await fetch(
        `${apiBaseUrl}/api/builder/profile-imports/${profileImport.id}`,
        {
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ proposedProfile: draft }),
        },
      )

      if (!response.ok) {
        throw new Error(await readApiError(response))
      }

      const updated = (await response.json()) as ProfileImport
      setProfileImport(updated)
      setDraft(updated.proposedProfile)
      return true
    } catch (error) {
      setImportError(
        error instanceof Error ? error.message : 'Unable to save changes',
      )
      return false
    } finally {
      setIsSubmitting(false)
    }
  }

  async function transitionImport(action: 'approve' | 'reject') {
    if (!profileImport) return

    try {
      if (action === 'approve') {
        const saved = await saveDraft()
        if (!saved) return
      }

      setImportError(null)
      setIsSubmitting(true)

      const response = await fetch(
        `${apiBaseUrl}/api/builder/profile-imports/${profileImport.id}/${action}`,
        { method: 'POST' },
      )

      if (!response.ok) {
        throw new Error(await readApiError(response))
      }

      setProfileImport({
        ...profileImport,
        status: action === 'approve' ? 'approved' : 'rejected',
      })
    } catch (error) {
      setImportError(
        error instanceof Error ? error.message : `Unable to ${action} import`,
      )
    } finally {
      setIsSubmitting(false)
    }
  }

  async function deleteImport() {
    if (!profileImport) return

    setImportError(null)
    setIsSubmitting(true)

    try {
      const response = await fetch(
        `${apiBaseUrl}/api/builder/profile-imports/${profileImport.id}`,
        { method: 'DELETE' },
      )

      if (!response.ok) {
        throw new Error(await readApiError(response))
      }

      setProfileImport(null)
      setDraft(null)
    } catch (error) {
      setImportError(
        error instanceof Error ? error.message : 'Unable to delete import',
      )
    } finally {
      setIsSubmitting(false)
    }
  }

  function updateListField(
    key: Exclude<keyof ProposedBuilderProfile, 'name' | 'location'>,
    value: string,
  ) {
    if (!draft) return

    setDraft({
      ...draft,
      [key]: value
        .split('\n')
        .map((item) => item.trim())
        .filter(Boolean),
    })
  }

  const selectedDiscussion =
    discussions.find(({ id }) => id === selectedDiscussionId) ?? null

  async function analyzeSelectedDiscussion() {
    if (!selectedDiscussion) return

    setAnalysisError(null)
    setAnalysisResult(null)
    setIsAnalyzing(true)

    try {
      const response = await fetch(
        `${apiBaseUrl}/api/analysis/discussions/${selectedDiscussion.id}`,
        { method: 'POST' },
      )

      if (!response.ok) {
        throw new Error(await readApiError(response))
      }

      setAnalysisResult(
        (await response.json()) as AnalyzeDiscussionResponse,
      )
    } catch (error) {
      setAnalysisError(
        error instanceof Error
          ? error.message
          : 'Unable to analyze this discussion',
      )
    } finally {
      setIsAnalyzing(false)
    }
  }

  async function discoverSelectedDiscussion() {
    if (!selectedDiscussion) return

    setOpportunityError(null)
    setIsDiscovering(true)

    try {
      const response = await fetch(`${apiBaseUrl}/api/discovery`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ discussionId: selectedDiscussion.id }),
      })

      if (!response.ok) {
        throw new Error(await readApiError(response))
      }

      const report = (await response.json()) as OpportunityDetail
      setSelectedOpportunity(report)
      setOpportunities((current) => {
        const summary: OpportunitySummary = {
          id: report.opportunity.id,
          discussionId: report.opportunity.discussionId,
          type: report.opportunity.type,
          problem: report.opportunity.problem,
          problemInferred: report.opportunity.problemInferred,
          topic: report.opportunity.topic,
          score: report.opportunity.score,
          confidence: report.opportunity.confidence,
          suggestedAction: report.opportunity.suggestedAction,
          createdAt: report.opportunity.createdAt,
        }

        return [
          summary,
          ...current.filter((item) => item.id !== summary.id),
        ]
      })
    } catch (error) {
      setOpportunityError(
        error instanceof Error
          ? error.message
          : 'Unable to discover an opportunity',
      )
    } finally {
      setIsDiscovering(false)
    }
  }

  async function selectOpportunity(id: string) {
    setOpportunityError(null)

    try {
      const response = await fetch(`${apiBaseUrl}/api/opportunities/${id}`)
      if (!response.ok) {
        throw new Error(await readApiError(response))
      }

      setSelectedOpportunity((await response.json()) as OpportunityDetail)
    } catch (error) {
      setOpportunityError(
        error instanceof Error
          ? error.message
          : 'Unable to load the opportunity report',
      )
    }
  }

  return (
    <main className="app-shell">
      <p className="eyebrow">ArgosHound</p>
      <h1>Opportunity intelligence for builders</h1>
      <p className="intro">
        Scout public conversations for product and builder opportunities.
      </p>

      <section className="connection-card" aria-live="polite">
        <span
          className={`status-dot status-dot--${backendStatus.state}`}
          aria-hidden="true"
        />
        <div>
          <h2>Backend connection</h2>
          <p>
            {backendStatus.state === 'connected'
              ? `Connected: ${backendStatus.message}`
              : backendStatus.message}
          </p>
          <code>{apiBaseUrl}/api/health</code>
        </div>
      </section>

      <section className="profile-import">
        <div className="section-heading">
          <p className="eyebrow">Builder context</p>
          <h2>Import an assistant profile</h2>
          <p>
            Ask ChatGPT, Claude, or another assistant to run the profile export
            prompt, then paste its JSON result. Nothing changes until you
            approve the preview.
          </p>
        </div>

        {!profileImport && (
          <form className="import-form" onSubmit={createImport}>
            <label>
              Assistant
              <select
                value={provider}
                onChange={(event) =>
                  setProvider(event.target.value as ProfileImport['provider'])
                }
              >
                <option value="chatGpt">ChatGPT</option>
                <option value="claude">Claude</option>
                <option value="other">Other</option>
              </select>
            </label>

            <label>
              Exported profile JSON
              <textarea
                value={content}
                onChange={(event) => setContent(event.target.value)}
                placeholder='{"name":"Jordan","currentSkills":["React"],...}'
                rows={12}
                required
              />
            </label>

            <button type="submit" disabled={isSubmitting}>
              {isSubmitting ? 'Extracting…' : 'Create review'}
            </button>
          </form>
        )}

        {profileImport && draft && (
          <div className="review-panel">
            <div className="review-meta">
              <span className={`status-badge status-badge--${profileImport.status}`}>
                {profileImport.status}
              </span>
              <span>
                Raw pasted content deleted after extraction
              </span>
            </div>

            {profileImport.status === 'extracted' && (
              <>
                <div className="change-preview">
                  <h3>Field-level preview</h3>
                  {profileImport.changes.map((change) => (
                    <div className="change-row" key={change.field}>
                      <strong>{change.field}</strong>
                      <span>{change.currentValues.join(', ') || 'Empty'}</span>
                      <span>{change.proposedValues.join(', ') || 'Empty'}</span>
                    </div>
                  ))}
                </div>

                <div className="profile-fields">
                  <label>
                    Name
                    <input
                      value={draft.name}
                      onChange={(event) =>
                        setDraft({ ...draft, name: event.target.value })
                      }
                    />
                  </label>

                  <label>
                    Location
                    <input
                      value={draft.location ?? ''}
                      onChange={(event) =>
                        setDraft({
                          ...draft,
                          location: event.target.value || null,
                        })
                      }
                    />
                  </label>

                  {listFields.map(({ key, label }) => (
                    <label key={key}>
                      {label} — one per line
                      <textarea
                        value={draft[key].join('\n')}
                        onChange={(event) =>
                          updateListField(key, event.target.value)
                        }
                        rows={4}
                      />
                    </label>
                  ))}
                </div>

                <div className="actions">
                  <button
                    type="button"
                    className="secondary"
                    onClick={saveDraft}
                    disabled={isSubmitting}
                  >
                    Save edits
                  </button>
                  <button
                    type="button"
                    onClick={() => transitionImport('approve')}
                    disabled={isSubmitting}
                  >
                    Approve profile
                  </button>
                  <button
                    type="button"
                    className="secondary"
                    onClick={() => transitionImport('reject')}
                    disabled={isSubmitting}
                  >
                    Reject
                  </button>
                </div>
              </>
            )}

            <button
              type="button"
              className="danger"
              onClick={deleteImport}
              disabled={isSubmitting}
            >
              Delete import
            </button>
          </div>
        )}

        {importError && (
          <p className="error-message" role="alert">
            {importError}
          </p>
        )}
      </section>

      <section className="source-evidence">
        <div className="section-heading">
          <p className="eyebrow">Source evidence</p>
          <h2>Seeded public discussions</h2>
          <p>
            Inspect the exact thread and comments that later opportunity
            analysis will cite. Handles are source attribution only and are not
            stored as lead profiles.
          </p>
        </div>

        {sourcesLoading && <p className="source-state">Loading discussions…</p>}

        {sourceError && (
          <p className="error-message" role="alert">
            {sourceError}
          </p>
        )}

        {!sourcesLoading && !sourceError && (
          <div className="source-browser">
            <nav className="source-list" aria-label="Source discussions">
              {discussions.map((discussion) => (
                <button
                  type="button"
                  className={
                    discussion.id === selectedDiscussionId
                      ? 'source-list-item source-list-item--selected'
                      : 'source-list-item'
                  }
                  onClick={() => {
                    setSelectedDiscussionId(discussion.id)
                    setAnalysisResult(null)
                    setAnalysisError(null)
                  }}
                  key={discussion.id}
                >
                  <span>{discussion.community}</span>
                  <strong>{discussion.title}</strong>
                  <small>{discussion.comments.length} comments</small>
                </button>
              ))}
            </nav>

            {selectedDiscussion && (
              <article className="discussion-detail">
                <div className="source-meta">
                  <span>{selectedDiscussion.platform}</span>
                  <span>{selectedDiscussion.community}</span>
                  <time dateTime={selectedDiscussion.publishedAt}>
                    {new Date(
                      selectedDiscussion.publishedAt,
                    ).toLocaleDateString()}
                  </time>
                </div>

                <h3>{selectedDiscussion.title}</h3>
                <p>{selectedDiscussion.body}</p>
                <p className="source-attribution">
                  Source author:{' '}
                  {selectedDiscussion.authorHandle ?? 'not retained'}
                </p>
                <a
                  className="source-link"
                  href={selectedDiscussion.url}
                  target="_blank"
                  rel="noreferrer"
                >
                  Open original thread
                </a>

                <div className="analysis-actions">
                  <button
                    type="button"
                    onClick={analyzeSelectedDiscussion}
                    disabled={isAnalyzing}
                  >
                    {isAnalyzing ? 'Analyzing with Foundry…' : 'Analyze opportunity'}
                  </button>
                  <span>
                    Review only — this does not save an opportunity or contact anyone.
                  </span>
                  <button
                    type="button"
                    className="secondary"
                    onClick={discoverSelectedDiscussion}
                    disabled={isDiscovering}
                  >
                    {isDiscovering
                      ? 'Discovering and saving…'
                      : 'Discover and save'}
                  </button>
                </div>

                {analysisError && (
                  <p className="error-message" role="alert">
                    {analysisError}
                  </p>
                )}

                {analysisResult?.discussionId === selectedDiscussion.id && (
                  <section className="analysis-result" aria-live="polite">
                    <div className="analysis-result__header">
                      <span
                        className={`opportunity-badge opportunity-badge--${analysisResult.analysis.opportunityType}`}
                      >
                        {analysisResult.analysis.opportunityType}
                      </span>
                      <span>
                        {Math.round(analysisResult.analysis.confidence * 100)}%
                        {' '}confidence
                      </span>
                      <span>{analysisResult.analysis.sentiment} sentiment</span>
                    </div>

                    <h4>{analysisResult.analysis.topic}</h4>
                    <p>
                      <strong>Problem:</strong>{' '}
                      {analysisResult.analysis.problem.summary}
                    </p>
                    {analysisResult.analysis.problem.inferred && (
                      <p className="inference-label">
                        Inferred need — not explicitly requested by the source.
                      </p>
                    )}

                    <p>{analysisResult.analysis.explanation}</p>

                    {analysisResult.analysis.productMatch && (
                      <div className="product-match">
                        <strong>
                          {analysisResult.analysis.productMatch.productName}
                        </strong>
                        <span>
                          {analysisResult.analysis.productMatch.matchType
                            .replace(/([A-Z])/g, ' $1')
                            .toLowerCase()}{' '}
                          match
                        </span>
                        <ul>
                          {analysisResult.analysis.productMatch
                            .matchedCapabilities.map((capability) => (
                              <li key={capability}>{capability}</li>
                            ))}
                        </ul>
                      </div>
                    )}

                    <div>
                      <strong>Evidence references</strong>
                      <div className="evidence-references">
                        {analysisResult.analysis.evidenceReferences.length > 0
                          ? analysisResult.analysis.evidenceReferences.map(
                              (reference) => (
                                <code key={reference}>{reference}</code>
                              ),
                            )
                          : <span>No problem evidence identified</span>}
                      </div>
                    </div>

                    <div>
                      <strong>Limitations</strong>
                      <ul>
                        {analysisResult.analysis.limitations.map((limitation) => (
                          <li key={limitation}>{limitation}</li>
                        ))}
                      </ul>
                    </div>

                    <p>
                      <strong>Suggested action:</strong>{' '}
                      {analysisResult.analysis.suggestedAction}
                    </p>
                  </section>
                )}

                <div className="comment-list">
                  <h4>Relevant comments</h4>
                  {selectedDiscussion.comments.map((comment) => (
                    <article className="source-comment" key={comment.id}>
                      <div className="comment-meta">
                        <span>{comment.authorHandle ?? 'Author not retained'}</span>
                        <time dateTime={comment.publishedAt}>
                          {new Date(comment.publishedAt).toLocaleString()}
                        </time>
                      </div>
                      <p>{comment.body}</p>
                      <a
                        href={comment.url}
                        target="_blank"
                        rel="noreferrer"
                      >
                        Open exact comment
                      </a>
                    </article>
                  ))}
                </div>
              </article>
            )}
          </div>
        )}
      </section>

      <section className="opportunity-reports">
        <div className="section-heading">
          <p className="eyebrow">Opportunity reports</p>
          <h2>Validated, persisted discoveries</h2>
          <p>
            Scores are calculated by deterministic backend factors. Model
            confidence is displayed separately and is not used as the final score.
          </p>
        </div>

        {opportunitiesLoading && (
          <p className="source-state">Loading opportunities…</p>
        )}

        {opportunityError && (
          <p className="error-message" role="alert">
            {opportunityError}
          </p>
        )}

        {!opportunitiesLoading && opportunities.length === 0 && (
          <p className="source-state">
            No opportunities saved yet. Select a source and choose
            {' '}<strong>Discover and save</strong>.
          </p>
        )}

        {opportunities.length > 0 && (
          <div className="opportunity-browser">
            <nav className="opportunity-list" aria-label="Saved opportunities">
              {opportunities.map((opportunity) => (
                <button
                  type="button"
                  className={
                    opportunity.id === selectedOpportunity?.opportunity.id
                      ? 'opportunity-list-item opportunity-list-item--selected'
                      : 'opportunity-list-item'
                  }
                  onClick={() => selectOpportunity(opportunity.id)}
                  key={opportunity.id}
                >
                  <span>{opportunity.type}</span>
                  <strong>{opportunity.topic}</strong>
                  <small>Score {opportunity.score}/100</small>
                </button>
              ))}
            </nav>

            {selectedOpportunity && (
              <article className="opportunity-detail">
                <div className="opportunity-report-header">
                  <span
                    className={`opportunity-badge opportunity-badge--${selectedOpportunity.opportunity.type}`}
                  >
                    {selectedOpportunity.opportunity.type}
                  </span>
                  <strong>
                    Score {selectedOpportunity.opportunity.score}/100
                  </strong>
                  <span>
                    Model confidence{' '}
                    {Math.round(
                      selectedOpportunity.opportunity.confidence * 100,
                    )}%
                  </span>
                </div>

                <h3>{selectedOpportunity.opportunity.topic}</h3>
                <p>{selectedOpportunity.opportunity.problem}</p>
                {selectedOpportunity.opportunity.problemInferred && (
                  <p className="inference-label">
                    Inferred problem — the source did not explicitly request
                    this solution.
                  </p>
                )}

                <div className="report-source">
                  <strong>Source</strong>
                  <span>
                    {selectedOpportunity.source.community} ·{' '}
                    {selectedOpportunity.source.title}
                  </span>
                  <a
                    href={selectedOpportunity.source.url}
                    target="_blank"
                    rel="noreferrer"
                  >
                    Open original thread
                  </a>
                </div>

                {selectedOpportunity.opportunity.matchedProductName && (
                  <div className="product-match">
                    <strong>
                      {selectedOpportunity.opportunity.matchedProductName}
                    </strong>
                    <span>
                      {selectedOpportunity.opportunity.productMatchType
                        ?.replace(/([A-Z])/g, ' $1')
                        .toLowerCase()}{' '}
                      match
                    </span>
                    <ul>
                      {selectedOpportunity.opportunity.matchedCapabilities.map(
                        (capability) => (
                          <li key={capability}>{capability}</li>
                        ),
                      )}
                    </ul>
                  </div>
                )}

                <div>
                  <strong>Relevant source comments</strong>
                  <div className="report-comments">
                    {selectedOpportunity.relevantComments.length > 0
                      ? selectedOpportunity.relevantComments.map((comment) => (
                          <article key={comment.id}>
                            <p>{comment.body}</p>
                            <a
                              href={comment.url}
                              target="_blank"
                              rel="noreferrer"
                            >
                              Open {comment.externalId}
                            </a>
                          </article>
                        ))
                      : <p>No comment-level evidence was selected.</p>}
                  </div>
                </div>

                <p>{selectedOpportunity.opportunity.explanation}</p>

                <div>
                  <strong>Limitations</strong>
                  <ul>
                    {selectedOpportunity.opportunity.limitations.map(
                      (limitation) => <li key={limitation}>{limitation}</li>,
                    )}
                  </ul>
                </div>

                <div>
                  <strong>Score factors</strong>
                  <div className="score-factors">
                    {selectedOpportunity.opportunity.scoreFactors.map(
                      (factor) => (
                        <div className="score-factor" key={factor.key}>
                          <span>{factor.label}</span>
                          <strong>
                            {factor.points > 0 ? '+' : ''}
                            {factor.points}
                          </strong>
                          <small>{factor.explanation}</small>
                        </div>
                      ),
                    )}
                  </div>
                </div>

                <p>
                  <strong>Suggested action:</strong>{' '}
                  {selectedOpportunity.opportunity.suggestedAction}
                </p>
              </article>
            )}
          </div>
        )}
      </section>
    </main>
  )
}

export default App
