import type { GetOderById, Order, Order_response } from "@/model/UserData";
import axios from "axios";

const BaseUrl = "http://localhost:5002/v1";

export async function createOrder(order: Order): Promise<Order_response> {
  const response = await axios.post(
    `${BaseUrl}/orders`, order,
    {
      headers: {
        'Content-Type': 'application/json',
      },
    }
  )

  return response.data
}

export async function getOrdersByUserId(user_id: number): Promise<any> {
  const request = await axios.get(`${BaseUrl}/orders/user/${user_id}`, { withCredentials: true });
  if (request.data) {
    return request.data;
  } else {
    throw new Error("Error fetching orders");
  }
}

export async function OrderById(order_id: number): Promise<GetOderById> {
  const request = await axios.get(`${BaseUrl}/orders/${order_id}`, { withCredentials: true });
  if (request.data) {
    return request.data;
  } else {
    throw new Error("Error fetching order");
  }
}

export async function UpdateOrderItems(order_item_id: number): Promise<boolean> {
  const request = await axios.patch(`${BaseUrl}/orders/items/${order_item_id}`, order_item_id, { withCredentials: true });
  if (request.data) {
    return request.data;
  } else {
    throw new Error("Error fetching logout");
  }
}

export async function OrderPerStandId(stand_id: number): Promise<GetOderById> {
  const request = await axios.get(`${BaseUrl}/orders/stand/${stand_id}`, { withCredentials: true });
  if (request.data) {
    return request.data;
  } else {
    throw new Error("Error fetching order");
  }
}