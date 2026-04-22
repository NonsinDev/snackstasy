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

export async function RemoveBalance(user_id: number, amount: number): Promise<void> {
  try {
    const request = await axios.put(
      `${BaseUrl}/balance/${user_id}/remove/${amount}`,
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
    
    console.log('RemoveBalance Response:', request.data)
    
    if (request.status === 200 || request.status === 204) {
      return request.data
    } else {
      throw new Error(`Unexpected status code: ${request.status}`)
    }
  } catch (error: any) {
    console.error('RemoveBalance Error:', error)
    console.error('Response:', error.response?.data)
    console.error('Status:', error.response?.status)
    
    if (error.response?.data?.message) {
      throw new Error(error.response.data.message)
    } else if (error.response?.status === 402) {
      throw new Error('Unzureichendes Guthaben')
    } else if (error.message) {
      throw new Error(error.message)
    } else {
      throw new Error('Fehler beim Abziehen des Guthabens')
    }
  }
}
