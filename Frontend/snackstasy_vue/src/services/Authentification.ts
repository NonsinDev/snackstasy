/* 
import type { CurrenUser_response } from "@/model/AuthentificationInterface";
import { ref } from "vue";
import Login from "@/views/Login.vue";


const user = ref<CurrenUser_response | null>(null);
const isLoading = ref(true);

export async function initAuth() {
  try {
    user.value = await Login();
  } catch {
    user.value = null;
  } finally {
    isLoading.value = false;
  }
}

export function useAuth() {
  return {
    user,
    isAuthenticated: () => !!user.value,
    isLoading,
  };
} */