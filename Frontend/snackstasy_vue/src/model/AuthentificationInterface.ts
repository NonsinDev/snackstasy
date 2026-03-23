export interface Login_data {
  user_id: string;
  username: string
}

export interface Login_response {
  logged_in: boolean;
  ticket_id: string
}

export interface CurrenUser_response {
  logged_in: boolean,
  ticket_id: number,
  first_name: string,
  last_name: string,
}
export interface Login_Request {
  ticket_id: string;
  username: string
}
