# Backend API

This folder contains a .NET 8.0 web API that manages customer tickets, balance, and orders for the food ordering system. The API provides endpoints for ticket management, user authentication, and balance operations.
More functions in funture.

After startup the API will be available at http://localhost:5002/v1/

## Authentication & Session Management

The API uses **HTTP Sessions** for authentication. Here's the login workflow:

1. **Verification (Optional):** Call `POST /login-check` to verify if an account with given credentials exists (returns user data without creating a session)
2. **Login:** Call `POST /login` with user ID (ticket ID) and username to authenticate and create a session
3. **Session Creation:** On successful login, a session is created server-side with user information
4. **Protected Routes:** Endpoints that modify data (updates, purchases) use the `RequireSession()` filter to ensure a valid session exists
5. **Username Format:** Usernames are constructed from `first_name` + `last_name` (concatenated)

**Example /login-check Request (verification only):**
```json
POST /login-check
{
  "user_id": "000001",
  "username": "JohnDoe"
}
```

**Response:**
```json
{
  "exists": true,
  "user_id": "000001",
  "first_name": "John",
  "last_name": "Doe",
  "balance": 5000
}
```

**Example /login Request (creates session):**
```json
POST /login
{
  "user_id": "000001",
  "username": "JohnDoe"
}
```

**Response:**
```json
{
  "message": "Login successful",
  "logged_in": true,
  "user_id": "000001",
  "first_name": "John",
  "last_name": "Doe",
  "balance": 5000
}
```

## API Routes

For detailed API documentation, visit the Swagger UI under `localhost:5002/swagger` when the backend is running.