window.SnackstasyApiSpec = {
  "openapi": "3.0.3",
  "info": {
    "title": "Snackstasy API",
    "description": "API für das Snackstasy Essens-Bestellungs-Tool",
    "version": "1.0.0",
    "contact": {
      "name": "Snackstasy Team"
    }
  },
  "servers": [
    {
      "url": "http://localhost:5002/v1",
      "description": "Local Development Server"
    }
  ],
  "tags": [
    {
      "name": "Login",
      "description": "Benutzer-Authentifizierung und Session-Management"
    },
    {
      "name": "Tickets",
      "description": "Ticket-Verwaltung"
    },
    {
      "name": "Balance",
      "description": "Guthaben-Verwaltung"
    },
    {
      "name": "Stands",
      "description": "Stand-Verwaltung"
    },
    {
      "name": "Items",
      "description": "Artikel-Verwaltung"
    }
  ],
  "paths": {
    "/login-check": {
      "post": {
        "tags": ["Login"],
        "summary": "Prüft ob ein Account existiert",
        "description": "Überprüft ob ein Account mit den angegebenen Daten existiert",
        "operationId": "loginCheck",
        "requestBody": {
          "required": true,
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/LoginRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Account existiert",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "exists": { "type": "boolean" },
                    "user_id": { "type": "integer" },
                    "first_name": { "type": "string" },
                    "last_name": { "type": "string" },
                    "balance": { "type": "number", "format": "float" }
                  }
                }
              }
            }
          },
          "400": {
            "description": "Ungültige Anfrage",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ErrorResponse"
                }
              }
            }
          },
          "401": {
            "description": "Falscher Benutzername"
          },
          "404": {
            "description": "Benutzer nicht gefunden"
          }
        }
      }
    },
    "/login": {
      "post": {
        "tags": ["Login"],
        "summary": "Login und Session erstellen",
        "description": "Authentifiziert einen Benutzer und erstellt eine Session",
        "operationId": "login",
        "requestBody": {
          "required": true,
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/LoginRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Login erfolgreich",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "message": { "type": "string" },
                    "logged_in": { "type": "boolean" },
                    "user_id": { "type": "integer" },
                    "first_name": { "type": "string" },
                    "last_name": { "type": "string" },
                    "balance": { "type": "number", "format": "float" }
                  }
                }
              }
            }
          },
          "400": {
            "description": "Ungültige Anfrage"
          },
          "401": {
            "description": "Falscher Benutzername"
          },
          "404": {
            "description": "Benutzer nicht gefunden"
          }
        }
      }
    },
    "/logout": {
      "post": {
        "tags": ["Login"],
        "summary": "Logout und Session beenden",
        "description": "Beendet die aktuelle Session des Benutzers",
        "operationId": "logout",
        "responses": {
          "200": {
            "description": "Logout erfolgreich",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "message": { "type": "string" },
                    "logged_in": { "type": "boolean" }
                  }
                }
              }
            }
          },
          "400": {
            "description": "Keine aktive Session"
          }
        }
      }
    },
    "/session": {
      "get": {
        "tags": ["Login"],
        "summary": "Session-Status prüfen",
        "description": "Gibt den aktuellen Session-Status zurück",
        "operationId": "getSession",
        "responses": {
          "200": {
            "description": "Session-Status",
            "content": {
              "application/json": {
                "schema": {
                  "oneOf": [
                    {
                      "type": "object",
                      "properties": {
                        "logged_in": { "type": "boolean", "example": true },
                        "user_id": { "type": "integer" },
                        "username": { "type": "string" },
                        "first_name": { "type": "string" },
                        "last_name": { "type": "string" },
                        "logged_in_at": { "type": "string", "format": "date-time" }
                      }
                    },
                    {
                      "type": "object",
                      "properties": {
                        "logged_in": { "type": "boolean", "example": false }
                      }
                    }
                  ]
                }
              }
            }
          }
        }
      }
    },
    "/tickets/all": {
      "get": {
        "tags": ["Tickets"],
        "summary": "Alle Tickets abrufen",
        "description": "Gibt alle registrierten Tickets zurück (erfordert aktive Session)",
        "operationId": "getAllTickets",
        "security": [{ "SessionAuth": [] }],
        "responses": {
          "200": {
            "description": "Liste der Tickets",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "user_id": { "type": "integer" },
                    "first_name": { "type": "string" },
                    "last_name": { "type": "string" },
                    "balance": { "type": "number", "format": "float" }
                  }
                }
              }
            }
          },
          "401": {
            "description": "Nicht autorisiert - Session erforderlich"
          }
        }
      }
    },
    "/tickets/book": {
      "post": {
        "tags": ["Tickets"],
        "summary": "Neues Ticket buchen",
        "description": "Erstellt ein neues Ticket für einen Benutzer (erfordert aktive Session)",
        "operationId": "bookTicket",
        "security": [{ "SessionAuth": [] }],
        "requestBody": {
          "required": true,
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/BookRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Ticket erfolgreich erstellt",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "id": { "type": "integer" },
                    "first_name": { "type": "string" },
                    "last_name": { "type": "string" }
                  }
                }
              }
            }
          },
          "400": {
            "description": "Ungültige Anfrage"
          },
          "401": {
            "description": "Nicht autorisiert - Session erforderlich"
          }
        }
      }
    },
    "/balance/{user_id}": {
      "get": {
        "tags": ["Balance"],
        "summary": "Guthaben abrufen",
        "description": "Gibt das aktuelle Guthaben eines Benutzers zurück (erfordert aktive Session)",
        "operationId": "getBalance",
        "security": [{ "SessionAuth": [] }],
        "parameters": [
          {
            "name": "user_id",
            "in": "path",
            "required": true,
            "schema": { "type": "integer" },
            "description": "Benutzer-ID"
          }
        ],
        "responses": {
          "200": {
            "description": "Guthaben erfolgreich abgerufen",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "balance": { "type": "number", "format": "decimal" }
                  }
                }
              }
            }
          },
          "401": {
            "description": "Nicht autorisiert - Session erforderlich"
          },
          "404": {
            "description": "Benutzer nicht gefunden"
          }
        }
      }
    },
    "/balance/{user_id}/update/{new_balance}": {
      "put": {
        "tags": ["Balance"],
        "summary": "Guthaben setzen",
        "description": "Setzt das Guthaben auf einen bestimmten Wert (erfordert aktive Session)",
        "operationId": "updateBalance",
        "security": [{ "SessionAuth": [] }],
        "parameters": [
          {
            "name": "user_id",
            "in": "path",
            "required": true,
            "schema": { "type": "integer" },
            "description": "Benutzer-ID"
          },
          {
            "name": "new_balance",
            "in": "path",
            "required": true,
            "schema": { "type": "number", "format": "decimal" },
            "description": "Neuer Guthaben-Wert"
          }
        ],
        "responses": {
          "200": {
            "description": "Guthaben erfolgreich aktualisiert",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "message": { "type": "string" }
                  }
                }
              }
            }
          },
          "401": {
            "description": "Nicht autorisiert - Session erforderlich"
          },
          "404": {
            "description": "Ticket nicht gefunden"
          }
        }
      }
    },
    "/balance/{user_id}/remove/{amount}": {
      "put": {
        "tags": ["Balance"],
        "summary": "Guthaben abziehen",
        "description": "Zieht einen Betrag vom Guthaben ab (erfordert aktive Session)",
        "operationId": "removeBalance",
        "security": [{ "SessionAuth": [] }],
        "parameters": [
          {
            "name": "user_id",
            "in": "path",
            "required": true,
            "schema": { "type": "integer" },
            "description": "Benutzer-ID"
          },
          {
            "name": "amount",
            "in": "path",
            "required": true,
            "schema": { "type": "number", "format": "decimal" },
            "description": "Abzuziehender Betrag"
          }
        ],
        "responses": {
          "200": {
            "description": "Betrag erfolgreich abgezogen",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "message": { "type": "string" }
                  }
                }
              }
            }
          },
          "400": {
            "description": "Nicht genügend Guthaben"
          },
          "401": {
            "description": "Nicht autorisiert - Session erforderlich"
          },
          "404": {
            "description": "Ticket nicht gefunden"
          }
        }
      }
    },
    "/balance/{user_id}/add/{amount}": {
      "put": {
        "tags": ["Balance"],
        "summary": "Guthaben aufladen",
        "description": "Fügt einen Betrag zum Guthaben hinzu (erfordert aktive Session)",
        "operationId": "addBalance",
        "security": [{ "SessionAuth": [] }],
        "parameters": [
          {
            "name": "user_id",
            "in": "path",
            "required": true,
            "schema": { "type": "integer" },
            "description": "Benutzer-ID"
          },
          {
            "name": "amount",
            "in": "path",
            "required": true,
            "schema": { "type": "number", "format": "decimal" },
            "description": "Hinzuzufügender Betrag"
          }
        ],
        "responses": {
          "200": {
            "description": "Betrag erfolgreich hinzugefügt",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "message": { "type": "string" }
                  }
                }
              }
            }
          },
          "401": {
            "description": "Nicht autorisiert - Session erforderlich"
          },
          "404": {
            "description": "Ticket nicht gefunden"
          }
        }
      }
    },
    "/stands/all": {
      "get": {
        "tags": ["Stands"],
        "summary": "Alle Stände abrufen",
        "description": "Gibt eine Liste aller Stände zurück",
        "operationId": "getAllStands",
        "responses": {
          "200": {
            "description": "Liste der Stände",
            "content": {
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/Stand"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/stands/{stand_id}": {
      "get": {
        "tags": ["Stands"],
        "summary": "Stand nach ID abrufen",
        "description": "Gibt einen bestimmten Stand anhand seiner ID zurück",
        "operationId": "getStandById",
        "parameters": [
          {
            "name": "stand_id",
            "in": "path",
            "required": true,
            "schema": { "type": "integer" },
            "description": "Stand-ID"
          }
        ],
        "responses": {
          "200": {
            "description": "Stand erfolgreich abgerufen",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/Stand"
                }
              }
            }
          },
          "404": {
            "description": "Stand nicht gefunden"
          }
        }
      }
    },
    "/items/all": {
      "get": {
        "tags": ["Items"],
        "summary": "Alle Artikel abrufen",
        "description": "Gibt eine Liste aller Artikel über alle Stände hinweg zurück",
        "operationId": "getAllItems",
        "responses": {
          "200": {
            "description": "Liste der Artikel",
            "content": {
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/Item"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/stands/{stand_id}/items": {
      "get": {
        "tags": ["Items"],
        "summary": "Artikel nach Stand abrufen",
        "description": "Gibt alle Artikel für einen bestimmten Stand zurück",
        "operationId": "getItemsByStand",
        "parameters": [
          {
            "name": "stand_id",
            "in": "path",
            "required": true,
            "schema": { "type": "integer" },
            "description": "Stand-ID"
          }
        ],
        "responses": {
          "200": {
            "description": "Liste der Artikel für den Stand",
            "content": {
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/Item"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/items/{item_id}": {
      "get": {
        "tags": ["Items"],
        "summary": "Artikel nach ID abrufen",
        "description": "Gibt einen bestimmten Artikel anhand seiner ID zurück",
        "operationId": "getItemById",
        "parameters": [
          {
            "name": "item_id",
            "in": "path",
            "required": true,
            "schema": { "type": "integer" },
            "description": "Artikel-ID"
          }
        ],
        "responses": {
          "200": {
            "description": "Artikel erfolgreich abgerufen",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/Item"
                }
              }
            }
          },
          "404": {
            "description": "Artikel nicht gefunden"
          }
        }
      }
    }
  },
  "components": {
    "securitySchemes": {
      "SessionAuth": {
        "type": "apiKey",
        "in": "cookie",
        "name": "session",
        "description": "Session-basierte Authentifizierung"
      }
    },
    "schemas": {
      "LoginRequest": {
        "type": "object",
        "required": ["user_id", "username"],
        "properties": {
          "user_id": {
            "type": "integer",
            "description": "Benutzer-/Ticket-ID"
          },
          "username": {
            "type": "string",
            "description": "Benutzername"
          }
        }
      },
      "BookRequest": {
        "type": "object",
        "required": ["first_name", "last_name"],
        "properties": {
          "first_name": {
            "type": "string",
            "description": "Vorname"
          },
          "last_name": {
            "type": "string",
            "description": "Nachname"
          },
          "birth_date": {
            "type": "string",
            "format": "date-time",
            "description": "Geburtsdatum"
          }
        }
      },
      "Stand": {
        "type": "object",
        "properties": {
          "stand_id": {
            "type": "integer",
            "description": "Stand-ID"
          },
          "name": {
            "type": "string",
            "description": "Stand-Name"
          },
          "pickup_id": {
            "type": "string",
            "description": "Abholstation-ID"
          },
          "tablet_id": {
            "type": "string",
            "description": "Tablet-ID"
          }
        }
      },
      "Item": {
        "type": "object",
        "properties": {
          "item_id": {
            "type": "integer",
            "description": "Artikel-ID"
          },
          "stand_id": {
            "type": "integer",
            "description": "Stand-ID"
          },
          "name": {
            "type": "string",
            "description": "Artikel-Name"
          },
          "price": {
            "type": "number",
            "format": "decimal",
            "description": "Preis"
          },
          "stock": {
            "type": "integer",
            "description": "Bestand"
          }
        }
      },
      "ErrorResponse": {
        "type": "object",
        "properties": {
          "error": {
            "type": "string",
            "description": "Fehlermeldung"
          }
        }
      }
    }
  }
};
