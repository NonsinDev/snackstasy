import type { Employee, EmployeeLogin, EmployeeResponse } from "@/model/UserData";
import axios from "axios";

const BaseUrl = "http://localhost:5002/v1";

export async function EmployeeLogin(login : EmployeeLogin): Promise<any> {
    console.log('API URL:', `${BaseUrl}/employee/login`)
    console.log('Request Body:', login)
    
    // Versuche POST statt GET
    const request = await axios.post(`${BaseUrl}/employee/login`, login, {
      headers: {
        'Content-Type': 'application/json',
      },
    })
  if (request.data) {
    console.log('API Response:', request.data)
    return request.data;
  } else {
    throw new Error("Error fetching login");
  }
}




export async function EmployeeCreate(employee : Employee): Promise<EmployeeLogin> {
  const request = await axios.post(`${BaseUrl}/employee/create`, {
      params: employee,
      headers: {
        'Content-Type': 'application/json',
      },
    })
  if (request.data) {
    return request.data;
  } else {
    throw new Error("Error fetching login");
  }
}