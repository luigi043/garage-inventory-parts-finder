# Garage Inventory And Parts Finder Frontend

Day 3 React + TypeScript interface for the protected MVP workflow.

## Run

~~~bash
npm install
npm run dev
~~~

Set the API base URL when the backend is not running on the default port:

~~~bash
VITE_API_BASE_URL=http://localhost:5000
~~~

## Implemented

- JWT sign-in and registration screens.
- Persistent local session storage with refresh-token recovery.
- Dashboard, operations overview, and Part workspace.
- Connected list, detail, create, and edit flows for /api/part.
- Loading, empty, and error states.

Demo login after backend migrations and seed:

~~~text
email: demo@garageinventorypartsfinder.local
password: Passw0rd!
~~~