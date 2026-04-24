# Employee Login System - Implementierungsanleitung

## Überblick
Ein separates Mitarbeiter-Login-System wurde implementiert, das es Mitarbeitern ermöglicht, sich mit Benutzername und Passwort einzuloggen und nur die Bestellungen ihres Standes zu verwalten.

---

## 🎯 Funktionalität

### 1. **Ticket-System (Kundenlogin)** - Existierend
- Zugang: `/login`
- Authentifizierung via Username + Ticket ID
- Zugriff auf alle Stände und deren Speiseplan

### 2. **Mitarbeiter-System** - Neu ✨
- **Zugang zum Mitarbeiter-Login:** Button unterhalb des Kundenlogins
- **Login-Seite:** `/employee-login`
- **Authentifizierung:** Username + Passwort (DB-Validierung via BCrypt)
- **Dashboard:** `/employee-dashboard`
- **Funktionen:**
  - Nur Bestellungen des eigenen Standes sichtbar
  - Liste aller offenen/zubereiteten Bestellungen
  - Markierung von Artikeln als "Abgeholt"
  - Bestelldetails anzeigen
  - Logout-Funktion

---

## 📁 Geänderte/Neue Dateien

### Frontend
1. **`src/views/Login.vue`** ✏️
   - Button "Zum Mitarbeiter-Zugang" hinzugefügt
   - Link zu `/employee-login`

2. **`src/views/EmployeeLogin.vue`** ✨ NEU
   - Mitarbeiter-Login-Formular
   - Username + Passwort Eingaben
   - Loading-State und Error-Handling
   - Redirect zu Dashboard nach erfolgreichem Login

3. **`src/views/EmployeeDashboard.vue`** ✨ NEU
   - DataTable mit allen Bestellungen des Standes
   - Bestelldetails in Dialog anzeigen
   - Items als "Abgeholt" markieren
   - Echtzeit-Status Anzeige
   - Logout-Funktion

4. **`src/services/Authentification.ts`** ✏️
   - `initEmployeeAuth()` - Speichert Employee-Daten in localStorage
   - `useEmployeeAuth()` - Lädt Employee-Auth-Daten
   - `clearEmployeeAuth()` - Löscht Employee-Session

5. **`src/services/Employee.ts`** ✏️
   - Angepasste Response-Struktur für Login

6. **`src/router/index.ts`** ✏️
   - Neue Route: `/employee-login` → EmployeeLogin.vue
   - Neue Route: `/employee-dashboard` → EmployeeDashboard.vue
   - Employee-Auth Guard für `/employee-dashboard`

7. **`src/model/UserData.ts`** ✏️
   - Erweiterte EmployeeResponse mit `stand_id` und `role`

### Backend
1. **`Backend/Models/Order.cs`** ✨ NEU
   - Order Model
   - OrderItem Model
   - OrderCreateRequest
   - OrderDetailResponse

2. **`Backend/Router/OrderRoutes.cs`** ✨ NEU (vollständig implementiert)
   - `GET /orders/stand/{stand_id}` - Alle Bestellungen eines Standes
   - `GET /orders/user/{user_id}` - Alle Bestellungen eines Kunden
   - `GET /orders/{order_id}` - Spezifische Bestellung mit Items
   - `POST /orders` - Neue Bestellung erstellen
   - `PATCH /orders/items/{order_item_id}` - Item als abgeholt markieren
   - `PATCH /orders/{order_id}/status` - Bestellstatus ändern

3. **`Backend/Router/EmployeeRoutes.cs`** ✏️
   - Geändert von `POST` zu `GET /employee/login`
   - Erweiterte Response: `employee_id`, `username`, `stand_id`, `role`

---

## 🔄 Login-Flow

### Kundenlogin (Existierend)
```
Login.vue → Login.ts → /v1/login → User Authentifizierung → FoodMenu.vue
```

### Mitarbeiter-Login (Neu)
```
Login.vue 
  ↓ (Button "Mitarbeiter-Zugang")
EmployeeLogin.vue 
  ↓ (Username + Passwort)
Employee.EmployeeLogin() 
  ↓ (GET /v1/employee/login)
Backend validiert via BCrypt 
  ↓ (Response mit employee_id, stand_id)
initEmployeeAuth() speichert in localStorage 
  ↓
EmployeeDashboard.vue 
  ↓ (Lädt Bestellungen via OrderPerStandId)
Zeigt Stand-Bestellungen
```

---

## 🛡️ Authentifizierung

### Kundenlogin
- Session-basiert via `checkSession()`
- Token-Verwaltung über Login Service

### Mitarbeiter-Login
- localStorage-basiert (wie angefordert, ohne Session)
- Struktur:
```json
{
  "employee_id": 1,
  "username": "staff01",
  "stand_id": 2,
  "loggedInAt": "2026-04-24T..."
}
```

---

## 📊 Datenbank-Struktur (Relevant)

### Employees
```sql
SELECT employee_id, username, password_hash, stand_id, is_active 
FROM employees 
WHERE username = ?
```

### Orders für Stand
```sql
SELECT * FROM orders WHERE stand_id = ?
ORDER BY created_at DESC
```

### Order-Items
```sql
SELECT * FROM order_items WHERE order_id = ?
```

---

## 🧪 Test-Anmeldedaten

Aus `mysql-init/init.sql`:

| Username | Password | Role | Stand |
|----------|----------|------|-------|
| staff01  | staff1234 | staff | 2     |
| admin    | admin1234 | admin | 1     |

---

## ✅ Implementierte Features

- ✅ Separater Mitarbeiter-Login Button auf Login-Seite
- ✅ Mitarbeiter-Login Formular (Username + Passwort)
- ✅ DB-Validierung mit BCrypt (kein Session-Storage)
- ✅ Employee Dashboard mit Stand-Bestellungen
- ✅ Bestelldetails anzeigen
- ✅ Items als "Abgeholt" markieren
- ✅ Status-Farben und Badges
- ✅ Responsive Design
- ✅ Logout-Funktion
- ✅ Error-Handling und Loading States
- ✅ Backend API für Bestellungsabruf

---

## 🚀 Verwendung

1. **Kundenlogin:** Hauptseite → Login mit Ticket-ID
2. **Mitarbeiter-Login:** Hauptseite → "Zum Mitarbeiter-Zugang" Button
3. **Im Dashboard:** Bestellungen verwalten, Items abhaken

---

## 📝 Zukünftige Verbesserungen (Optional)

- [ ] Bestellstatus direkt im Dashboard ändern
- [ ] Real-time Updates via WebSocket
- [ ] Audit-Log für Standbetreiber
- [ ] Druckfunktion für Bestellungen
- [ ] Push-Notifications bei neuen Bestellungen
