export interface Course {
  id: number;
  name: string;
  description: string;
  start_Date: string;
  finish_Date: string;
}

export type CourseMini = {
  name: string;
  description: string;
  start_Date: string;
  finish_Date: string;
}
