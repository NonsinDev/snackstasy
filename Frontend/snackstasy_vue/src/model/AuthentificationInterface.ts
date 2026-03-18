export interface Login_data {
  user_id: string;
  username: string
}

export interface Login_response {
  exists: boolean,
  user_id: number,
  first_name: string,
  last_name: string,
  balance: number
}

export interface CurrenUser_response {
  message: string,
  logged_in: boolean,
  user_id: number,
  first_name: string,
  last_name: string,
  balance: number
}