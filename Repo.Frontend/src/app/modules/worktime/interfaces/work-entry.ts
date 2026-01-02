export interface WorkEntry {
  id: number;
  workTable_ID: number;
  task_ID?: number | null;
  work_Date: string;        // 'YYYY-MM-DD'
  hours_Worked: number;
  comment?: string | null;
  taskName?: string | null;
  userNickname?: string | null;
  userID?: number;
}
