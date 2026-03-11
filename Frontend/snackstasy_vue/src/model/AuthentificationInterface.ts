export interface CurrenUser_response {
  id: number
  username: String;
  role: String;
}

export interface Login_Register {
  username: string;
  password: string
}

export interface Login_response {
  success: boolean
}