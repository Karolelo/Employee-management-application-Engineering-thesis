import {Component, ViewChild, AfterViewInit} from "@angular/core";
import {DayPilot, DayPilotModule, DayPilotCalendarComponent, DayPilotMonthComponent} from "@daypilot/daypilot-lite-angular";
import {EventsService} from '../events.service';
//TODO zastanowić się czy wynieśc do klasy css style
@Component({
  selector: 'calendar-component',
  standalone: false,
  template: `
    <div class="buttons">
        <button class="btn-primary" (click)="viewDay()">Day</button>
        <button class="btn-primary" (click)="viewWeek()">Week</button>
        <button class="btn-primary" (click)="viewMonth()">Month</button>
    </div>

    <daypilot-calendar #calendar [config]="config" [events]="events" *ngIf="!isMonthView">
    </daypilot-calendar>
    <daypilot-month #month [config]="configMonth" [events]="events" *ngIf="isMonthView">
    </daypilot-month>
  `,
  styles: [`
    .buttons { margin-bottom: 10px; display: flex;,flex-direction: row;,justify-content: center;,color: #6c757d; padding: 10px; }
    .buttons button { margin-right: 5px; }
  `]
})
export class CalendarComponent implements AfterViewInit {
  @ViewChild("calendar")
  calendar!: DayPilotCalendarComponent;

  @ViewChild("month")
  month!: DayPilotMonthComponent;

  events: any[] = [];
  isMonthView: boolean = false;

  config: DayPilot.CalendarConfig = {
    viewType: "Week",
    timeRangeSelectedHandling: "Enabled",
    onTimeRangeSelected: async (args) => {
      const modal = await DayPilot.Modal.prompt("Create a new event:", "Event 1");
      const calendar = args.control;
      calendar.clearSelection();
      if (modal.canceled) { return; }
      calendar.events.add({
        start: args.start,
        end: args.end,
        id: DayPilot.guid(),
        text: modal.result
      });
    },
    eventMoveHandling: "Update",
    onEventMoved: (args) => {
      console.log("Event moved: " + args.e.text());
    },
    eventResizeHandling: "Update",
    onEventResized: (args) => {
      console.log("Event resized: " + args.e.text());
    },
    eventClickHandling: "Disabled",
  };

  configMonth: DayPilot.MonthConfig = {
    timeRangeSelectedHandling: "Enabled",
    onTimeRangeSelected: async (args) => {
      const modal = await DayPilot.Modal.prompt("Create a new event:", "Event 1");
      const month = args.control;
      month.clearSelection();
      if (modal.canceled) { return; }
      month.events.add({
        start: args.start,
        end: args.end,
        id: DayPilot.guid(),
        text: modal.result
      });
    },
    eventMoveHandling: "Update",
    onEventMoved: (args) => {
      console.log("Event moved: " + args.e.text());
    },
    eventClickHandling: "Disabled",
  };

  constructor(private ds: EventsService) {
  }

  viewDay(): void {
    this.isMonthView = false;
    this.config.viewType = "Day";
    this.calendar.control.update();
  }

  viewWeek(): void {
    this.isMonthView = false;
    this.config.viewType = "Week";
    this.calendar.control.update();
  }

  viewMonth(): void {
    this.isMonthView = true;
  }

  ngAfterViewInit(): void {
    this.ds.getResources().subscribe(result => this.config.columns = result);
    this.loadEvents();
  }

  private loadEvents(): void {
    const from = this.isMonthView ?
      this.month.control.visibleStart() :
      this.calendar.control.visibleStart();
    const to = this.isMonthView ?
      this.month.control.visibleEnd() :
      this.calendar.control.visibleEnd();

    this.ds.getEvents(from, to).subscribe(result => {
      this.events = result;
    });
  }
}
