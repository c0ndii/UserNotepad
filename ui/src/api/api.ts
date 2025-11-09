import axios from "axios";
import { backendUrlBase } from "../constants/const";

export const api = axios.create({
  baseURL: backendUrlBase,
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers = config.headers ?? {};
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      localStorage.removeItem("token");
      localStorage.removeItem("tokenExpires");
      window.location.href = "/login";
    }

    return Promise.reject(error);
  }
);
