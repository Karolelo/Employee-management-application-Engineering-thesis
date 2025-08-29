export interface Task {
  id: number;
  name: string;
  description: string;
  startDate: Date;
  estimatedTime: number;
  creator: string;
  deleted: boolean;
  priority: string;
  status: string;
}
