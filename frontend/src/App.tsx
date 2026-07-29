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
                  onClick={() => setSelectedDiscussionId(discussion.id)}
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
    </main>
  )
}

export default App
