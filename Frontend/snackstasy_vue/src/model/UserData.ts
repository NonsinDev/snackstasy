export interface User_Data {
    user_id: number,
    first_name: String,
    last_name: String,
    balance: number,
    ticket_id: String
  }
  
export interface Order {
  "user_id": number,
  "stand_id": number,
  "items": Array<{
    "item_id": number,
    "quantity": number
  }>
}

export interface Order_response {
  "order_id": number,
  "total_price": number
}

export interface GetOderById {
  "order": {
    "order_id": number,
    "user_id": number,
    "stand_id": number,
    "total_price": number,
    "status": String,
    "created_at": String
  },
  "items": Array<{
    "order_item_id": number,
    "order_id": number,
    "item_id": number,
    "is_collected": boolean,
    "quantity": number,
    "price": number
  }>
}

export interface EmployeeLogin {
  "username": String,
  "password": String
}

export interface EmployeeResponse {
  "employee_id": number,
  "username": String
}

export interface Employee {
  "username": String,
  "password": String,
  "first_name": String,
  "last_name": String,
  "role": EmployeeRole,
  "stand_id": number
}

export type EmployeeRole = 'admin' | 'staff' | 'manager' | 'cashier'