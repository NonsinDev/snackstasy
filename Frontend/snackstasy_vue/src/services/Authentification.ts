
import type {  Login_response } from "@/model/AuthentificationInterface";
import { ref } from "vue";

import { checkSession } from '@/services/Login'

const user = ref<Login_response | null>(null);
const isLoading = ref(true);
const employeeData = ref<any>(null);

export async function initAuth() {
  try {
    user.value = await checkSession();
  } catch {
    user.value = null;
  } finally {
    isLoading.value = false;
  }
}

export function initEmployeeAuth(employeeId: number, username: string, standId: number) {
  const authData = {
    employee_id: employeeId,
    username: username,
    stand_id: standId,
    loggedInAt: new Date().toISOString()
  };
  
  localStorage.setItem('employeeAuth', JSON.stringify(authData));
  employeeData.value = authData;
}

export function useAuth() {
  return {
    user,
    isAuthenticated: () => user.value?.logged_in === true,
    isLoading,
  };
}

export function useEmployeeAuth() {
  // IMMER aus localStorage laden, nicht nur beim ersten Mal
  if (typeof localStorage !== 'undefined') {
    const stored = localStorage.getItem('employeeAuth');
    console.log('Loading employeeAuth from localStorage:', stored)
    
    if (stored) {
      try {
        employeeData.value = JSON.parse(stored);
        console.log('Loaded employeeData:', employeeData.value)
      } catch (e) {
        console.error('Failed to parse employeeAuth:', e)
        localStorage.removeItem('employeeAuth');
        employeeData.value = null;
      }
    } else {
      employeeData.value = null;
    }
  }

  return {
    employeeData,
    isEmployeeAuthenticated: () => {
      const isAuth = employeeData.value?.employee_id !== undefined;
      console.log('isEmployeeAuthenticated:', isAuth, 'employeeData:', employeeData.value)
      return isAuth;
    },
    clearEmployeeAuth: () => {
      employeeData.value = null;
      if (typeof localStorage !== 'undefined') {
        localStorage.removeItem('employeeAuth');
      }
    }
  };
}
