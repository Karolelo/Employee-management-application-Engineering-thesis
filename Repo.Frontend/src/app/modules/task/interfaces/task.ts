//Możliwe będą zmiany tego potem
export interface Task {
  id: number;
  Name: string;
  Description: string;
  StartDate: Date;
  EstimatedTime: number;
  Creator: string;
  Deleted: boolean;
  Priority: string;
  Status: string;
}
