export interface TokenResponse {
  message: string;
  token: {
    accessToken: string;
    refreshToken: string;
  }
}
