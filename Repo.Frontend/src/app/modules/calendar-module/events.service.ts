import {Injectable} from "@angular/core";
import {Observable} from "rxjs";
import { DayPilot } from "@daypilot/daypilot-lite-angular";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class EventsService {
  events: any[] = [
    {
      id: 1,
      start: DayPilot.Date.today().addHours(12),
      end: DayPilot.Date.today().addHours(14),
      text: "Event 1",
      resource: "GA"
    },
    {
      id: 2,
      start: DayPilot.Date.today().addHours(9),
      end: DayPilot.Date.today().addHours(10),
      text: "Event 2",
      resource: "R1"
    }
  ];

  resources: any[] = [
    { name: "Group A", id: "GA", children: [
        { name: "Resource 1", id: "R1"},
        { name: "Resource 2", id: "R2"},
        { name: "Resource 3", id: "R3"},
        { name: "Resource 4", id: "R4"}
      ]},
    { name: "Group B", id: "GB", children: [
        { name: "Resource 5", id: "R5"},
        { name: "Resource 6", id: "R6"},
        { name: "Resource 7", id: "R7"},
        { name: "Resource 8", id: "R8"}
      ]}
  ];

  constructor(private http : HttpClient){
  }

  getEvents(from: DayPilot.Date, to: DayPilot.Date): Observable<any[]> {

    // simulating an HTTP request
    return new Observable(observer => {
      setTimeout(() => {
        observer.next(this.events);
      }, 200);
    });

    // return this.http.get("/api/events?from=" + from.toString() + "&to=" + to.toString());
  }

  getResources(): Observable<any[]> {

    // simulating an HTTP request
    return new Observable(observer => {
      setTimeout(() => {
        observer.next(this.resources);
      }, 200);
    });

    // return this.http.get("/api/resources");
  }
}
