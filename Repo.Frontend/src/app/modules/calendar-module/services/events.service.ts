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
  private readonly ApiUrl = "api/Calendar/events/"
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
  */
  constructor(private http : HttpClient){
  }

  getEvents(userId: number,from: DayPilot.Date, to: DayPilot.Date): Observable<any[]> {
    return this.http.get<EventsData[]>(this.ApiUrl+`/user/${userId}`,
      {
        params: {
          from: from.toString(),
          to: to.toString()
        }
      })
  }
  changeEventColor(eventId: number, color: string): Observable<any> {
    return this.http.put(this.ApiUrl+`${eventId}/color`,{color: color},)
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
