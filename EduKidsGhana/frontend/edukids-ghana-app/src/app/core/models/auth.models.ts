export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterParentRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  phoneNumber: string;
  region?: string;
  city?: string;
}

export interface AuthResult {
  success: boolean;
  token?: string;
  refreshToken?: string;
  tokenExpiry?: string;
  user?: UserInfo;
  error?: string;
}

export interface UserInfo {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  roles: string[];
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors: string[];
  correlationId?: string;
  timestamp: string;
}
