export interface GanttTask {
  id: number;
  name: string;
  start_Time: Date;
  end_Time: Date;
  priority: string;
  status: string;
  ownerUserId: number;
  dependencies: number[];
}
