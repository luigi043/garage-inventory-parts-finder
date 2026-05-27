import { useEffect, useMemo, useState } from "react";
import type { FormEvent } from "react";
import {
  apiBaseUrl,
  clearStoredSession,
  createWorkflowItem,
  getCurrentUser,
  getAdvancedFeature,
  getStoredSession,
  listWorkflowItems,
  login,
  logout,
  refreshSession,
  register,
  saveSession,
  updateWorkflowItem,
  type AdvancedFeatureSnapshot,
  type Session,
  type WorkflowItem,
  type WorkflowItemDraft,
} from "./lib/api";

const project = {
  title: "Garage Inventory And Parts Finder",
  objective: "Build a garage inventory platform for tracking parts, tools, suppliers, compatibility, VIN metadata, and stock location.",
  targetUser: "Garage owners and car enthusiasts tracking parts, tools, compatibility, and suppliers.",
  workflow: "The user adds vehicles and parts, marks compatibility, tracks stock location and supplier, then searches for parts that fit a selected car.",
  advancedFeature: "Compatibility checker for vehicles, parts, and supplier options.",
  entityName: "Part",
  entityRoute: "part",
  coreEntities: ["User", "Vehicle", "Part", "PartCompatibility", "Supplier", "StockLocation", "PurchaseRecord", "Tool"],
  demoEmail: "demo@garageinventorypartsfinder.local",
  demoPassword: "Passw0rd!",
};

const emptyDraft: WorkflowItemDraft = {
  name: "",
  notes: "",
};

type AuthMode = "login" | "register";
type View = "dashboard" | "records" | "operations";
type LoadState = "idle" | "loading" | "error";

function getErrorMessage(error: unknown) {
  return error instanceof Error ? error.message : "Something went wrong.";
}

function formatDate(value?: string | null) {
  if (!value) {
    return "Not updated";
  }

  return new Intl.DateTimeFormat(undefined, {
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  }).format(new Date(value));
}

function getInitials(value: string) {
  return value
    .split(" ")
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0]?.toUpperCase())
    .join("");
}

export function App() {
  const [session, setSession] = useState<Session | null>(null);
  const [authMode, setAuthMode] = useState<AuthMode>("login");
  const [authForm, setAuthForm] = useState({
    email: project.demoEmail,
    password: project.demoPassword,
    displayName: "Demo User",
  });
  const [authError, setAuthError] = useState("");
  const [authLoading, setAuthLoading] = useState(true);
  const [authSubmitting, setAuthSubmitting] = useState(false);
  const [view, setView] = useState<View>("dashboard");
  const [items, setItems] = useState<WorkflowItem[]>([]);
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [recordsState, setRecordsState] = useState<LoadState>("idle");
  const [recordsError, setRecordsError] = useState("");
  const [createDraft, setCreateDraft] = useState<WorkflowItemDraft>(emptyDraft);
  const [editDraft, setEditDraft] = useState<WorkflowItemDraft>(emptyDraft);
  const [savingCreate, setSavingCreate] = useState(false);
  const [savingEdit, setSavingEdit] = useState(false);
  const [advancedFeature, setAdvancedFeature] = useState<AdvancedFeatureSnapshot | null>(null);
  const [advancedState, setAdvancedState] = useState<LoadState>("idle");
  const [advancedError, setAdvancedError] = useState("");

  const selectedItem = useMemo(
    () => items.find((item) => item.id === selectedId) ?? null,
    [items, selectedId],
  );

  const completionScore = useMemo(() => {
    const total = Math.max(project.coreEntities.length, 1);
    return Math.min(100, Math.round((items.length / total) * 100));
  }, [items.length]);

  useEffect(() => {
    const storedSession = getStoredSession();
    if (!storedSession) {
      setAuthLoading(false);
      return;
    }

    setSession(storedSession);
    void bootstrap(storedSession);
  }, []);

  useEffect(() => {
    if (!selectedItem) {
      setEditDraft(emptyDraft);
      return;
    }

    setEditDraft({
      name: selectedItem.name,
      notes: selectedItem.notes ?? "",
    });
  }, [selectedItem]);

  useEffect(() => {
    if (view === "operations" && session) {
      void loadAdvancedFeature(session);
    }
  }, [view, session]);
  async function bootstrap(activeSession: Session) {
    setAuthLoading(true);
    try {
      await getCurrentUser(activeSession);
      await loadItems(activeSession);
    } catch {
      try {
        const refreshedSession = await refreshSession(activeSession);
        saveSession(refreshedSession);
        setSession(refreshedSession);
        await loadItems(refreshedSession);
      } catch {
        clearStoredSession();
        setSession(null);
      }
    } finally {
      setAuthLoading(false);
    }
  }

  async function loadItems(activeSession = session) {
    if (!activeSession) {
      return;
    }

    setRecordsState("loading");
    setRecordsError("");
    try {
      const workflowItems = await listWorkflowItems(project.entityRoute, activeSession);
      setItems(workflowItems);
      setSelectedId((currentId) =>
        currentId && workflowItems.some((item) => item.id === currentId)
          ? currentId
          : workflowItems[0]?.id ?? null,
      );
      setRecordsState("idle");
    } catch (error) {
      setRecordsState("error");
      setRecordsError(getErrorMessage(error));
    }
  }


  async function loadAdvancedFeature(activeSession = session) {
    if (!activeSession) {
      return;
    }

    setAdvancedState("loading");
    setAdvancedError("");
    try {
      const snapshot = await getAdvancedFeature(activeSession);
      setAdvancedFeature(snapshot);
      setAdvancedState("idle");
    } catch (error) {
      setAdvancedState("error");
      setAdvancedError(getErrorMessage(error));
    }
  }
  async function handleAuthSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setAuthSubmitting(true);
    setAuthError("");

    try {
      const nextSession =
        authMode === "login"
          ? await login(authForm.email, authForm.password)
          : await register(authForm.email, authForm.password, authForm.displayName);

      saveSession(nextSession);
      setSession(nextSession);
      await loadItems(nextSession);
    } catch (error) {
      setAuthError(getErrorMessage(error));
    } finally {
      setAuthSubmitting(false);
    }
  }

  async function handleLogout() {
    if (session) {
      try {
        await logout(session);
      } catch {
        // Local sign-out should still clear a stale session.
      }
    }

    clearStoredSession();
    setSession(null);
    setItems([]);
    setSelectedId(null);
    setView("dashboard");
  }

  async function handleCreate(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!session || !createDraft.name.trim()) {
      return;
    }

    setSavingCreate(true);
    setRecordsError("");
    try {
      const createdItem = await createWorkflowItem(project.entityRoute, createDraft, session);
      setItems((currentItems) => [createdItem, ...currentItems]);
      setSelectedId(createdItem.id);
      setCreateDraft(emptyDraft);
      setView("records");
    } catch (error) {
      setRecordsError(getErrorMessage(error));
      setRecordsState("error");
    } finally {
      setSavingCreate(false);
    }
  }

  async function handleUpdate(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!session || !selectedItem || !editDraft.name.trim()) {
      return;
    }

    setSavingEdit(true);
    setRecordsError("");
    try {
      const updatedItem = await updateWorkflowItem(
        project.entityRoute,
        selectedItem.id,
        editDraft,
        session,
      );
      setItems((currentItems) =>
        currentItems.map((item) => (item.id === updatedItem.id ? updatedItem : item)),
      );
      setSelectedId(updatedItem.id);
    } catch (error) {
      setRecordsError(getErrorMessage(error));
      setRecordsState("error");
    } finally {
      setSavingEdit(false);
    }
  }

  if (authLoading && !session) {
    return (
      <main className="auth-page">
        <section className="auth-intro">
          <p className="eyebrow">Portfolio MVP</p>
          <h1>{project.title}</h1>
          <p>{project.objective}</p>
        </section>
        <section className="auth-panel loading-panel">Checking saved session...</section>
      </main>
    );
  }

  if (!session) {
    return (
      <main className="auth-page">
        <section className="auth-intro">
          <p className="eyebrow">Portfolio MVP workflow</p>
          <h1>{project.title}</h1>
          <p>{project.objective}</p>
          <div className="proof-strip">
            <span>{project.entityName}</span>
            <span>{project.coreEntities.length} modules</span>
            <span>JWT + API ready</span>
          </div>
        </section>

        <section className="auth-panel">
          <div className="segmented-control" aria-label="Authentication mode">
            <button
              className={authMode === "login" ? "active" : ""}
              type="button"
              onClick={() => setAuthMode("login")}
            >
              Sign in
            </button>
            <button
              className={authMode === "register" ? "active" : ""}
              type="button"
              onClick={() => setAuthMode("register")}
            >
              Create account
            </button>
          </div>

          <form className="stacked-form" onSubmit={handleAuthSubmit}>
            {authMode === "register" && (
              <label>
                Display name
                <input
                  value={authForm.displayName}
                  onChange={(event) =>
                    setAuthForm((current) => ({ ...current, displayName: event.target.value }))
                  }
                  required
                />
              </label>
            )}

            <label>
              Email
              <input
                autoComplete="email"
                type="email"
                value={authForm.email}
                onChange={(event) =>
                  setAuthForm((current) => ({ ...current, email: event.target.value }))
                }
                required
              />
            </label>

            <label>
              Password
              <input
                autoComplete={authMode === "login" ? "current-password" : "new-password"}
                type="password"
                value={authForm.password}
                onChange={(event) =>
                  setAuthForm((current) => ({ ...current, password: event.target.value }))
                }
                required
              />
            </label>

            {authError && <p className="error-banner">{authError}</p>}

            <div className="form-actions">
              <button className="primary-button" disabled={authSubmitting} type="submit">
                {authSubmitting ? "Working..." : authMode === "login" ? "Sign in" : "Create account"}
              </button>
              <button
                className="ghost-button"
                type="button"
                onClick={() =>
                  setAuthForm({
                    email: project.demoEmail,
                    password: project.demoPassword,
                    displayName: "Demo User",
                  })
                }
              >
                Use demo
              </button>
            </div>
          </form>
        </section>
      </main>
    );
  }

  return (
    <main className="app-frame">
      <aside className="sidebar">
        <div>
          <div className="brand-mark">{getInitials(project.title)}</div>
          <p className="eyebrow">Portfolio workspace</p>
          <h1>{project.title}</h1>
        </div>

        <nav className="nav-list" aria-label="Main navigation">
          <button className={view === "dashboard" ? "active" : ""} onClick={() => setView("dashboard")}>
            Dashboard
          </button>
          <button className={view === "records" ? "active" : ""} onClick={() => setView("records")}>
            {project.entityName}
          </button>
          <button className={view === "operations" ? "active" : ""} onClick={() => setView("operations")}>
            Operations
          </button>
        </nav>

        <div className="user-card">
          <span>{getInitials(session.displayName)}</span>
          <div>
            <strong>{session.displayName}</strong>
            <small>{session.email}</small>
          </div>
          <button className="icon-button" type="button" onClick={handleLogout} aria-label="Sign out">
            Out
          </button>
        </div>
      </aside>

      <section className="workspace">
        <header className="topbar">
          <div>
            <p className="eyebrow">Portfolio-ready full stack</p>
            <h2>{view === "records" ? `${project.entityName} workspace` : project.title}</h2>
            <p>{view === "dashboard" ? project.workflow : project.targetUser}</p>
          </div>
          <div className="api-pill">
            <span>API</span>
            <strong>{apiBaseUrl}</strong>
          </div>
        </header>

        {recordsError && <p className="error-banner">{recordsError}</p>}

        {view === "dashboard" && (
          <section className="dashboard-grid">
            <article className="metric-card strong">
              <span>Total {project.entityName}</span>
              <strong>{items.length}</strong>
              <small>{recordsState === "loading" ? "Syncing records" : "Synced from protected API"}</small>
            </article>
            <article className="metric-card">
              <span>Workflow coverage</span>
              <strong>{completionScore}%</strong>
              <small>{project.coreEntities.length} planned modules</small>
            </article>
            <article className="metric-card">
              <span>Signed in as</span>
              <strong>{session.displayName}</strong>
              <small>{session.email}</small>
            </article>

            <section className="workflow-panel">
              <div>
                <p className="eyebrow">Main workflow</p>
                <h3>{project.workflow}</h3>
              </div>
              <div className="entity-rail">
                {project.coreEntities.map((entity, index) => (
                  <span key={entity}>
                    <strong>{String(index + 1).padStart(2, "0")}</strong>
                    {entity}
                  </span>
                ))}
              </div>
            </section>

            <section className="quick-create">
              <div>
                <p className="eyebrow">Fast capture</p>
                <h3>Create {project.entityName}</h3>
              </div>
              <WorkflowForm
                draft={createDraft}
                onChange={setCreateDraft}
                onSubmit={handleCreate}
                submitLabel={savingCreate ? "Creating..." : "Create record"}
                disabled={savingCreate}
              />
            </section>
          </section>
        )}

        {view === "records" && (
          <section className="records-layout">
            <section className="list-panel">
              <div className="panel-heading">
                <div>
                  <p className="eyebrow">Protected endpoint</p>
                  <h3>{project.entityName} records</h3>
                </div>
                <button className="ghost-button compact" type="button" onClick={() => void loadItems()}>
                  Refresh
                </button>
              </div>

              {recordsState === "loading" && <p className="status-line">Loading records...</p>}

              {recordsState !== "loading" && items.length === 0 && (
                <div className="empty-state">
                  <strong>No {project.entityName.toLowerCase()} records yet</strong>
                  <span>Create the first record from the form.</span>
                </div>
              )}

              {items.length > 0 && (
                <div className="record-list">
                  {items.map((item) => (
                    <button
                      className={item.id === selectedId ? "record-row active" : "record-row"}
                      key={item.id}
                      type="button"
                      onClick={() => setSelectedId(item.id)}
                    >
                      <span>
                        <strong>{item.name}</strong>
                        <small>{item.notes || "No notes"}</small>
                      </span>
                      <em>{formatDate(item.updatedAtUtc ?? item.createdAtUtc)}</em>
                    </button>
                  ))}
                </div>
              )}
            </section>

            <section className="editor-panel">
              <div className="panel-block">
                <p className="eyebrow">Create</p>
                <h3>New {project.entityName}</h3>
                <WorkflowForm
                  draft={createDraft}
                  onChange={setCreateDraft}
                  onSubmit={handleCreate}
                  submitLabel={savingCreate ? "Creating..." : "Create record"}
                  disabled={savingCreate}
                />
              </div>

              <div className="panel-block">
                <p className="eyebrow">Detail and edit</p>
                <h3>{selectedItem ? selectedItem.name : "Select a record"}</h3>
                {selectedItem ? (
                  <>
                    <dl className="detail-list">
                      <div>
                        <dt>Created</dt>
                        <dd>{formatDate(selectedItem.createdAtUtc)}</dd>
                      </div>
                      <div>
                        <dt>Updated</dt>
                        <dd>{formatDate(selectedItem.updatedAtUtc)}</dd>
                      </div>
                    </dl>
                    <WorkflowForm
                      draft={editDraft}
                      onChange={setEditDraft}
                      onSubmit={handleUpdate}
                      submitLabel={savingEdit ? "Saving..." : "Save changes"}
                      disabled={savingEdit}
                    />
                  </>
                ) : (
                  <div className="empty-state">
                    <strong>Nothing selected</strong>
                    <span>Choose a row to inspect and edit it.</span>
                  </div>
                )}
              </div>
            </section>
          </section>
        )}

        {view === "operations" && (
          <section className="operations-grid">
            <article>
              <p className="eyebrow">Audience</p>
              <h3>{project.targetUser}</h3>
            </article>
            <article>
              <p className="eyebrow">Advanced API</p>
              <h3>/api/advanced-feature</h3>
              <span>Protected readiness snapshot powered by live {project.entityName} records</span>
            </article>
            <article>
              <p className="eyebrow">Advanced feature</p>
              <h3>{project.advancedFeature || "Ready for portfolio demo"}</h3>
            </article>

            <section className="advanced-feature-panel">
              <div className="panel-heading">
                <div>
                  <p className="eyebrow">Advanced feature</p>
                  <h3>Advanced feature readiness</h3>
                </div>
                <button
                  className="ghost-button compact"
                  type="button"
                  onClick={() => void loadAdvancedFeature()}
                >
                  Refresh
                </button>
              </div>

              {advancedState === "loading" && <p className="status-line">Calculating feature readiness...</p>}
              {advancedError && <p className="error-banner">{advancedError}</p>}

              {advancedFeature ? (
                <div className="advanced-feature-grid">
                  <div className="readiness-card">
                    <span>Readiness</span>
                    <strong>{advancedFeature.readinessScore}%</strong>
                    <em>{advancedFeature.maturity}</em>
                  </div>

                  <div className="insight-list">
                    <p className="eyebrow">Signals</p>
                    {advancedFeature.highlights.map((highlight) => (
                      <span key={highlight}>{highlight}</span>
                    ))}
                  </div>

                  <div className="insight-list">
                    <p className="eyebrow">Next actions</p>
                    {advancedFeature.nextActions.map((action) => (
                      <span key={action}>{action}</span>
                    ))}
                  </div>

                  <div className="recent-records">
                    <p className="eyebrow">Recent records</p>
                    {advancedFeature.recentRecords.length > 0 ? (
                      advancedFeature.recentRecords.map((record) => <span key={record}>{record}</span>)
                    ) : (
                      <span>Create a workflow record to feed this panel.</span>
                    )}
                  </div>
                </div>
              ) : (
                <div className="empty-state">
                  <strong>Advanced feature snapshot not loaded</strong>
                  <span>Refresh this panel after signing in and creating workflow data.</span>
                </div>
              )}
            </section>
          </section>
        )}
      </section>
    </main>
  );
}

type WorkflowFormProps = {
  draft: WorkflowItemDraft;
  onChange: (draft: WorkflowItemDraft) => void;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
  submitLabel: string;
  disabled?: boolean;
};

function WorkflowForm({ draft, onChange, onSubmit, submitLabel, disabled }: WorkflowFormProps) {
  return (
    <form className="stacked-form" onSubmit={onSubmit}>
      <label>
        Name
        <input
          value={draft.name}
          onChange={(event) => onChange({ ...draft, name: event.target.value })}
          required
        />
      </label>
      <label>
        Notes
        <textarea
          rows={4}
          value={draft.notes}
          onChange={(event) => onChange({ ...draft, notes: event.target.value })}
        />
      </label>
      <button className="primary-button" disabled={disabled} type="submit">
        {submitLabel}
      </button>
    </form>
  );
}