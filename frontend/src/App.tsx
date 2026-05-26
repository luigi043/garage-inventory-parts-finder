const workflowSteps = [
  "Authenticate demo user",
  "Open dashboard",
  "Create or update the core workflow",
  "Review metrics and status",
];

export function App() {
  return (
    <main className="app-shell">
      <section className="workspace">
        <header className="hero">
          <p className="eyebrow">Day 1 frontend scaffold</p>
          <h1>Garage Inventory And Parts Finder</h1>
          <p>Build a garage inventory platform for tracking parts, tools, suppliers, compatibility, VIN metadata, and stock location.</p>
        </header>

        <section className="panel">
          <h2>Main workflow</h2>
          <p>The user adds vehicles and parts, marks compatibility, tracks stock location and supplier, then searches for parts that fit a selected car.</p>
        </section>

        <section className="grid">
          {workflowSteps.map((step, index) => (
            <article className="step-card" key={step}>
              <span>{index + 1}</span>
              <p>{step}</p>
            </article>
          ))}
        </section>
      </section>
    </main>
  );
}
