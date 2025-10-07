import {Injectable} from "@angular/core";
import {Observable} from "rxjs";
import { DayPilot } from "@daypilot/daypilot-lite-angular";
import {HttpClient} from "@angular/common/http";
import {Task} from '../../task/interfaces/task';
import {EventsData} from '../interfaces/events-data';

@Injectable({
  providedIn: 'root'
})
export class EventsService {
  private readonly ApiUrl = "api/calendar/events/user/"
  static colors = {
    green: "#6aa84f",
    yellow: "#f1c232",
    red: "#cc4125",
    gray: "#808080",
    blue: "#2e78d6",
  };
  //Possible ways to give dates
  //end: new DayPilot.Date("2024-03-20T12:00:00"),
  // DayPilot.Date.parse(this.exampleDate.toISOString().substring(0, 19))
  //DayPilot.Date.fromYearMonthDay
  //end: DayPilot.Date.today().addHours(2)
  /*exampleDate= new Date(Date.now());
  //DayPilot.Date.parse(this.exampleDate.toISOString().substring(0,19),"yyyy-MM-ddTHH:mm:ss")
  events = [
    {
      id: 1,
      text: "Event 1",
      start: DayPilot.Date.today().firstDayOfWeek().addHours(10),
      end: DayPilot.Date.today().firstDayOfWeek().addHours(13),
      participants: 2,
    },
    {
      id: 2,
      text: "Event 2",
      start: DayPilot.Date.today().firstDayOfWeek().addDays(1).addHours(12),
      end: DayPilot.Date.today().firstDayOfWeek().addDays(1).addHours(13),
      backColor: this.getRandomColor(),
      participants: 1,
    },
    {
      id: 3,
      text: "Event 3",
      start: DayPilot.Date.today().firstDayOfWeek().addDays(2).addHours(13),
      end: DayPilot.Date.parse(this.exampleDate.toISOString().substring(0,19),"yyyy-MM-ddTHH:mm:ss"),
      backColor: this.getRandomColor(),
      participants: 3,
    },
    {
      id: 4,
      text: "Event 4",
      start: DayPilot.Date.today().firstDayOfWeek().addDays(2).addHours(11),
      end: DayPilot.Date.parse(this.exampleDate.toISOString().substring(0,19),"yyyy-MM-ddTHH:mm:ss"),
      backColor: this.getRandomColor(),
      participants: 4,
    },
  ];*/

  constructor(private http : HttpClient){
  }

  getEvents(userId: number,from: DayPilot.Date, to: DayPilot.Date): Observable<any[]> {
    return this.http.get<EventsData[]>(`api/calendar/events/user/${userId}`,
      {
        params: {
          from: from.toString(),
          to: to.toString()
        }
      })
  }

  getColors(): any[] {
    const colors = [
      {name: "Green", id: EventsService.colors.green},
      {name: "Yellow", id: EventsService.colors.yellow},
      {name: "Red", id: EventsService.colors.red},
      {name: "Gray", id: EventsService.colors.gray},
      {name: "Blue", id: EventsService.colors.blue},
    ];
    return colors;
  }

  getRandomColor(): string {
    const colors = this.getColors();
    const index = Math.floor(Math.random() * colors.length);
    return colors[index].id;
  }
}
