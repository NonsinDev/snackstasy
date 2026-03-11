import type { Login_Register, Login_response } from "@/model/AuthentificationInterface";
import axios from "axios";


// const BaseUrl = "https://cooking-recipe-web-production.up.railway.app/api";
const BaseUrl = "http://localhost:5000/";

export async function checkLogin(login: Login_Register): Promise<Login_response> {
  const request = await axios.post(`${BaseUrl}/login`, login, { withCredentials: true });
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
