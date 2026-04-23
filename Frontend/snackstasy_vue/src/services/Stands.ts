import type { AllStands, ItemsByStand, Stand } from "@/model/Items";
import axios from "axios";

const BaseUrl = "http://localhost:5002/v1";

export async function GetAllStands(): Promise<AllStands[]> {
  const request = await axios.get(`${BaseUrl}/stands/all`, { withCredentials: true });
  if (request.data) {
    return request.data;
  } else {
    throw new Error("Error fetching login");
  }
}

export async function StandById(stand_id: number): Promise<Stand> {
  const request = await axios.get(`${BaseUrl}/stands/${stand_id}`, { withCredentials: true });
  if (request.data) {
    return request.data;
  } else {
    throw new Error("Error fetching login");
  }
}

export async function ItemsByStandId(stand_id: number): Promise<ItemsByStand[]> {
  const request = await axios.get(`${BaseUrl}/stands/${stand_id}/items`,{ withCredentials: true });
  if (request.data) {
    return request.data;
  } else {
    throw new Error("Error fetching login");
  }
}