# Backend API

This folder contains a minimal .NET 7 API that exposes multiple endpoints such as balance check, adding and subtracting, login, and ticket retrieval and booking. More functions will be added in the future.

Run with Docker Compose from the repository root:

```bash
docker-compose up --build
```

After startup the API will be available at http://localhost:5000/tickets/ids

Hint: The API returns the id as a 6-digit string ("000001", "000002").

# API Routes
All routes in this api will get listed here
## Ticket Route

**GET /tickets** returns `id`, `first_name`, `last_name` and `balance` for each ticket in the db.

**POST /tickets/book** creates a new ticket in the db.

## Login Route

**GET /login** returns the `id`, `first_name`, `last_name` and `balance` if correct `id` and `password` is in body.

## Balance Route

**GET /balance/{id}** returns the balance of the user.

**PUT /balance/{id}/update/{newBalance}** updates the balance to new value and returns new value if successful.

**PUT /balance/{id}/remove/{amount}** removes the amount from balance but only if result is greater then 0. (Needs to be checkt if works correnctly)

**PUT /balance/{id}/add/{amount}** adds amount to balance.
