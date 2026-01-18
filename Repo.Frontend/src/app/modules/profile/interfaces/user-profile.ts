export interface UserProfile {
  nickname: string;
  name: string;
  surname: string;
  login: string;
  email: string;
}

export interface ChangeEmailRequest {
  currentPassword: string;
  newEmail: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}
