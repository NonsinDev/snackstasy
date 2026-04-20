import type {
  CurrenUser_response,
  Login_data,
  Login_Request,
  Login_response,
} from '@/model/AuthentificationInterface'
import type { User_Data } from '@/model/UserData'
import axios from 'axios'

const BaseUrl = 'http://localhost:5002/v1'

export async function UserData(ticket_id: string): Promise<User_Data> {
  const request = await axios.get(`${BaseUrl}/tickets/${ticket_id}`)
  if (request.data) {
    return request.data
  } else {
    throw new Error('Error fetching login')
  }
}

export async function AddBalance(user_id: number, amount: number): Promise<void> {
  const request = await axios.put(
    `${BaseUrl}/balance/${user_id}/add/${amount}`,
    {
      user_id: user_id,
      amount: amount,
    },
    {
      headers: {
        'Content-Type': 'application/json',
      },
    },
  )
  if (request.data) {
    return request.data
  } else {
    throw new Error('Error fetching login')
  }
}
