
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
  // IMMER aus localStorage laden
  if (typeof localStorage !== 'undefined') {
    const stored = localStorage.getItem('employeeAuth');
    
    if (stored) {
      try {
        employeeData.value = JSON.parse(stored);
      } catch (e) {
        localStorage.removeItem('employeeAuth');
        employeeData.value = null;
      }
    } else {
      employeeData.value = null;
    }
  }

  return {
    employeeData,
    isEmployeeAuthenticated: () => employeeData.value?.employee_id !== undefined,
    clearEmployeeAuth: () => {
      employeeData.value = null;
      if (typeof localStorage !== 'undefined') {
        localStorage.removeItem('employeeAuth');
      }
    }
  };
}
