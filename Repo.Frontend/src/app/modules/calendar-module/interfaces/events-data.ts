import { DayPilot } from "@daypilot/daypilot-lite-angular";
export interface EventsData {
  id: number;
  text: string;
  start: DayPilot.Date;
  end: DayPilot.Date;
  backColor: string;
  task_id?: number;
  absenceDay_id?: number;
  course_id?: number;
}
