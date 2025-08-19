export interface ApiResponse<T> {
  Success: boolean;
  Data: T;
  Error: string;
}
