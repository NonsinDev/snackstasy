import type { CurrenUser_response, Login_data, Login_Request, Login_response } from "@/model/AuthentificationInterface";
import axios from "axios";

const BaseUrl = "http://localhost:5002/v1";

export async function checkSession(): Promise<Login_response> {
  const request = await axios.get(`${BaseUrl}/login/session`, { withCredentials: true });
  if (request.data) {
    return request.data;
  } else {
    throw new Error("Error fetching login");
  }
}

export async function Logout(): Promise<void> {
  const request = await axios.post(`${BaseUrl}/login/logout`, { withCredentials: true });
  if (request.data) {
    return request.data;
  } else {
    throw new Error("Error fetching logout");
  }
}

export async function Login(login: Login_Request): Promise<CurrenUser_response> {
  const request = await axios.post(`${BaseUrl}/login`, login, { withCredentials: true });
  if (request.data) {
    return request.data;
  } else {
    throw new Error("Error fetching logout");
  }
}