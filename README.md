# Food Ordering Helper Tool

Simple web application to manage food orders at festivals and concerts.

## Quickstart

Start from the repository root (requires Docker & Docker Compose):

```powershell
docker-compose up --build
```

After startup the backend API is available at http://localhost:5000 by default.

Note: Ticket IDs are returned as 6-digit strings (e.g. "000001").

## API Overview

Base URL: `http://localhost:5000`

### GET /tickets/ids

- Description: Returns all tickets including ID, first name, last name and balance.
- Response format: JSON array of objects with the following fields:
  - `ticket_id`: int (6-digit, leading zeros, e.g. "000001")
  - `first_name`: string
  - `last_name`: string
  - `balance`: number (e.g. 12.34)
- Example response:

```json
[
  { 
    "ticket_id": "000001", 
    "first_name": "Max", 
    "last_name": "Mustermann", 
    "balance": 10.5 
  },
  { 
    "ticket_id": "000002", 
    "first_name": "Erika", 
    "last_name": "Musterfrau", 
    "balance": 0.0 
  }
]
```

- Example (curl / PowerShell):

```powershell
curl http://localhost:5000/tickets/ids
```

### POST /tickets/book

- Description: Creates a new ticket (booking) with first and last name. The ticket gets an auto-generated password and an initial balance of 0.
- Request body: JSON with the following fields:
  - `firstName`: string (required)
  - `lastName`: string (required)
- Example request:

```json
{
  "firstName": "Anna",
  "lastName": "Schmidt"
}
```

- Example response (success):

```json
{
  "ticket_id": "000123",
  "password": "a8B2xYz1",
  "first_name": "Anna",
  "last_name": "Schmidt"
}
```

- Possible errors:
  - `400 Bad Request` if `firstName` or `lastName` is missing or empty.
  - `500 Internal Server Error` for server-side problems.

### Notes & further information

- The backend implementation is in the `Backend/` folder. See `Backend/README.md` for additional details and instructions.
- Database: MySQL (see `docker-compose.yml` and `mysql-init/init.sql`). The backend waits for the database to become available and logs startup errors to the console if needed.

---


