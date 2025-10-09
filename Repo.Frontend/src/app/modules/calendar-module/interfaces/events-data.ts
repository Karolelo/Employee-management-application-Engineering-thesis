import { DayPilot } from "@daypilot/daypilot-lite-angular";
export interface EventsData {
  id: number;
  text: string;
  start: DayPilot.Date;
  end: DayPilot.Date;
  backColor: string;
  task_ID?: number;
  absenceDay_ID?: number;
  course_ID?: number;
}
