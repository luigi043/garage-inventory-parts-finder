export const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5000";

const sessionStorageKey = "portfolio-day3-session";

export type Session = {
  userId: string;
  email: string;
  displayName: string;
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
  refreshTokenExpiresAtUtc: string;
};

export type CurrentUser = {
  userId: string;
  email: string;
  displayName: string;
};

export type WorkflowItem = {
  id: string;
  name: string;
  notes?: string | null;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
};

export type WorkflowItemDraft = {
  name: string;
  notes: string;
};

export class ApiError extends Error {
  constructor(
    message: string,
    public readonly status: number,
    public readonly details: unknown,
  ) {
    super(message);
    this.name = "ApiError";
  }
}

function getApiMessage(payload: unknown, fallback: string) {
  if (payload && typeof payload === "object" && "message" in payload) {
    const message = (payload as { message?: unknown }).message;
    if (typeof message === "string") {
      return message;
    }
  }

  if (payload && typeof payload === "object" && "title" in payload) {
    const title = (payload as { title?: unknown }).title;
    if (typeof title === "string") {
      return title;
    }
  }

  return fallback;
}

function normalizeRoute(route: string) {
  return route.replace(/^\/+|\/+$/g, "");
}

async function apiRequest<T>(
  path: string,
  options: RequestInit = {},
  session?: Session | null,
): Promise<T> {
  const headers = new Headers(options.headers);

  if (options.body && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }

  if (session?.accessToken) {
    headers.set("Authorization", `Bearer ${session.accessToken}`);
  }

  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...options,
    headers,
  });

  const responseText = await response.text();
  const payload = responseText ? JSON.parse(responseText) : undefined;

  if (!response.ok) {
    throw new ApiError(
      getApiMessage(payload, `API request failed with status ${response.status}`),
      response.status,
      payload,
    );
  }

  return payload as T;
}

export function getStoredSession() {
  const rawSession = window.localStorage.getItem(sessionStorageKey);
  if (!rawSession) {
    return null;
  }

  try {
    return JSON.parse(rawSession) as Session;
  } catch {
    window.localStorage.removeItem(sessionStorageKey);
    return null;
  }
}

export function saveSession(session: Session) {
  window.localStorage.setItem(sessionStorageKey, JSON.stringify(session));
}

export function clearStoredSession() {
  window.localStorage.removeItem(sessionStorageKey);
}

export function login(email: string, password: string) {
  return apiRequest<Session>("/api/auth/login", {
    method: "POST",
    body: JSON.stringify({ email, password }),
  });
}

export function register(email: string, password: string, displayName: string) {
  return apiRequest<Session>("/api/auth/register", {
    method: "POST",
    body: JSON.stringify({ email, password, displayName }),
  });
}

export function refreshSession(session: Session) {
  return apiRequest<Session>("/api/auth/refresh", {
    method: "POST",
    body: JSON.stringify({ refreshToken: session.refreshToken }),
  });
}

export function logout(session: Session) {
  return apiRequest<void>(
    "/api/auth/logout",
    {
      method: "POST",
      body: JSON.stringify({ refreshToken: session.refreshToken }),
    },
    session,
  );
}

export function getCurrentUser(session: Session) {
  return apiRequest<CurrentUser>("/api/auth/me", {}, session);
}

export function listWorkflowItems(route: string, session: Session) {
  return apiRequest<WorkflowItem[]>(`/api/${normalizeRoute(route)}`, {}, session);
}

export function createWorkflowItem(route: string, draft: WorkflowItemDraft, session: Session) {
  return apiRequest<WorkflowItem>(
    `/api/${normalizeRoute(route)}`,
    {
      method: "POST",
      body: JSON.stringify(draft),
    },
    session,
  );
}

export function updateWorkflowItem(
  route: string,
  id: string,
  draft: WorkflowItemDraft,
  session: Session,
) {
  return apiRequest<WorkflowItem>(
    `/api/${normalizeRoute(route)}/${id}`,
    {
      method: "PUT",
      body: JSON.stringify(draft),
    },
    session,
  );
}
// Day 4 advanced feature API
export type AdvancedFeatureSnapshot = {
  projectName: string;
  featureName: string;
  entityName: string;
  connectedRecords: number;
  moduleCount: number;
  readinessScore: number;
  maturity: string;
  highlights: string[];
  nextActions: string[];
  recentRecords: string[];
  generatedAtUtc: string;
};

export function getAdvancedFeature(session: Session) {
  return apiRequest<AdvancedFeatureSnapshot>("/api/advanced-feature", {}, session);
}