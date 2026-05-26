const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5000";

export async function apiGet<T>(path: string): Promise<T> {
  const response = await fetch(${apiBaseUrl});

  if (!response.ok) {
    throw new Error(API request failed with status );
  }

  return response.json() as Promise<T>;
}
