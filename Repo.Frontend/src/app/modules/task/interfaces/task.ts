export interface Task {
  id: number;
  name: string;
  description: string;
  Start_Time: Date;
  estimated_Time: number;
  creator: string;
  deleted: boolean;
  priority: string;
  status: string;
}
