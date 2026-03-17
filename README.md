# Food Ordering Tool for Concerts and Festivals

Simple web application to order food at festivals and concerts.

## Quickstart

Start from the repository root (requires Docker & Docker Compose):

```bash
docker-compose down -v && docker-compose up --build
```

After startup the backend API is available at http://localhost:5002/v1/ by default.

## API Endpoints

Base URL: `http://localhost:5002`

Sufix: `v1`

Key routes:
- **`/tickets`** – Manage customer tickets (list, create, book)
- **`/login`** – User authentication
- **`/balance`** – Query and update user balance
- **`/stands`** – Manage stands (list, create, edit, remove)
- **`/items`** – Manage items (list, create, edit, remove, stock)

For comprehensive API documentation, see [Backend/README.md](Backend/README.md) or the [Swagger UI](swagger.html).

## Authentication

The API uses **session-based authentication**:
- Users call `/login-check` to verify credentials without creating a session
- Users then call `/login` with their ticket ID and username to authenticate and create a session
- A session token is created and stored server-side
- Subsequent requests require a valid session to perform protected operations (balance updates, bookings, etc.)

See [Backend/README.md](Backend/README.md#authentication--session-management) for detailed authentication information and examples.

## Architecture

- **Backend:** .NET 8.0 API in the `Backend/` folder with Dapper for data access
- **Frontend:** Vue 3 + TypeScript application in `Frontend/snackstasy_vue/`
- **Database:** MySQL (defined in `docker-compose.yml` and `mysql-init/init.sql`)
- **Containerization:** Docker & Docker Compose for local development

## Customization

To add more endpoints (e.g., balance top-up, purchase recording, or ticket deletion), modify `Backend/Program.cs` and update the corresponding route files in `Backend/Router/`.

---
