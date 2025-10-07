import {AfterViewInit, Component, OnInit, ViewChild} from "@angular/core";
import {
  DayPilot,
  DayPilotCalendarComponent,
  DayPilotMonthComponent,
  DayPilotNavigatorComponent
} from "@daypilot/daypilot-lite-angular";
import {EventsService} from '../../services/events.service'
import {UserStoreService} from '../../../login/services/user_data/user-store.service';
import {Router} from '@angular/router';
@Component({
  selector: 'calendar-component',
  standalone: false,
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})
export class CalendarComponent implements OnInit,AfterViewInit {

  @ViewChild("day") day!: DayPilotCalendarComponent;
  @ViewChild("week") week!: DayPilotCalendarComponent;
  @ViewChild("month") month!: DayPilotMonthComponent;
  @ViewChild("navigator") nav!: DayPilotNavigatorComponent;

  events: DayPilot.EventData[] = [];

  date = DayPilot.Date.today();

  contextMenu = new DayPilot.Menu({
    items: [
      {
        text: "Edit...",
        onClick: async args => {
          const event = args.source;
          const dp = event.calendar;

          const path = this.determinePath(event.data.text);
          this.router.navigate([path], {
            queryParams: {
              taskId: event.data.resource
            }
          })
          /*const modal = await DayPilot.Modal.prompt("Edit event text:", event.data.text);
          dp.clearSelection();
          if (!modal.result) {
            return;
          }
          event.data.text = modal.result;
          dp.events.update(event);*/
        }
      },
      {
        text: "-"
      },
      {
        text: "Red",
        onClick: args => {
          const event = args.source;
          const dp = event.calendar;
          event.data.backColor = EventsService.colors.red;
          this.eventsService.changeEventColor(
            event.data.id,
            "Red"
          ).subscribe({
            next: (response) => {
              console.log(response);
              console.log(`Change event to color: Red, eventId: ${event.data.id}`);
            },
            error: (error) => {
              console.log(error);
            }
          });
          dp.events.update(event);
        }
      },
      {
        text: "Green",
        onClick: args => {
          const event = args.source;
          const dp = event.calendar;
          event.data.backColor = EventsService.colors.green;
          this.eventsService.changeEventColor(
            event.data.id,
            "Green"
          ).subscribe({
            next: (response) => {
              console.log(response);
              console.log(`Change event to color: Green, eventId: ${event.data.id}`);
            },
            error: (error) => {
              console.log(error);
            }
          });
          dp.events.update(event);
        }
      },
      {
        text: "Blue",
        onClick: args => {
          const event = args.source;
          const dp = event.calendar;
          event.data.backColor = EventsService.colors.blue;
          this.eventsService.changeEventColor(
            event.data.id,
            "Blue"
          ).subscribe({
            next: (response) => {
              console.log(response);
              console.log(`Change event to color: Blue, eventId: ${event.data.id}`);
            },
            error: (error) => {
              console.log(error);
            }
          });
          dp.events.update(event);
        }
      },
      {
        text: "Yellow",
        onClick: args => {
          const event = args.source;
          const dp = event.calendar;
          event.data.backColor = EventsService.colors.yellow;
          this.eventsService.changeEventColor(
            event.data.id,
            "Yellow"
          ).subscribe({
            next: (response) => {
              console.log(response);
              console.log(`Change event to color: Yellow, eventId: ${event.data.id}`);
            },
            error: (error) => {
              console.log(error);
            }
          });
          dp.events.update(event);
        }
      },
      {
        text: "Gray",
        onClick: args => {
          const event = args.source;
          const dp = event.calendar;
          event.data.backColor = EventsService.colors.gray;
          this.eventsService.changeEventColor(
            event.data.id,
            "Gray"
          ).subscribe({
            next: (response) => {
              console.log(response);
              console.log(`Change event to color: Gray, eventId: ${event.data.id}`);
            },
            error: (error) => {
              console.log(error);
            }
          });
          dp.events.update(event);
        }
      }
    ]
  });

  configNavigator: DayPilot.NavigatorConfig = {
    showMonths: 3,
    cellWidth: 25,
    cellHeight: 25,
    onVisibleRangeChanged: args => {
      this.loadEvents();
    }
  };
  constructor(private ds: EventsService, private eventsService: EventsService,private userDataStore: UserStoreService,
  private router: Router) {
    this.viewWeek();
  }
  //TODO zrobić obsługę tych rzeczy
  determinePath(name: string): string{
    if(name.startsWith('Course'))
      return '/courses/course-details' ;
    if(name.startsWith('Task'))
      return '/tasks'
    if(name.startsWith('AbsenceDay'))
      return 'dashboard/absence-day-details'

    return '/notFound'
  }
  ngAfterViewInit(): void {
    this.loadEvents();
  }
  ngOnInit(): void {
    this.userDataStore.getUserData().subscribe({
    })
  }

  selectTomorrow() {
    this.date = DayPilot.Date.today().addDays(1);
  }

  changeDate(date: DayPilot.Date): void {
    this.configDay.startDate = date;
    this.configWeek.startDate = date;
    this.configMonth.startDate = date;
  }

  configDay: DayPilot.CalendarConfig = {
    durationBarVisible: false,
    contextMenu: this.contextMenu,
    /*onTimeRangeSelected: this.onTimeRangeSelected.bind(this),*/
    onBeforeEventRender: this.onBeforeEventRender.bind(this),
    onEventClick: this.onEventClick.bind(this),
    eventMoveHandling: "Disabled",
    eventResizeHandling: "Disabled",
  };

  configWeek: DayPilot.CalendarConfig = {
    viewType: "Week",
    durationBarVisible: false,
    contextMenu: this.contextMenu,
    /*onTimeRangeSelected: this.onTimeRangeSelected.bind(this),*/
    onBeforeEventRender: this.onBeforeEventRender.bind(this),
    onEventClick: this.onEventClick.bind(this),
    eventMoveHandling: "Disabled",
    eventResizeHandling: "Disabled"
  };

  configMonth: DayPilot.MonthConfig = {
    contextMenu: this.contextMenu,
    eventBarVisible: false,
    /*onTimeRangeSelected: this.onTimeRangeSelected.bind(this),*/
    onEventClick: this.onEventClick.bind(this),
    eventMoveHandling: "Disabled",
    eventResizeHandling: "Disabled"
  };
  loadEvents(): void {
    const from = this.nav.control.visibleStart();
    const to = this.nav.control.visibleEnd();
    const userId = this.userDataStore.getUserId();

    if (userId) {
      this.eventsService.getEvents(userId, from, to)
        .subscribe({
          next: (events) => {
            this.events = events.map(event => ({
              id: event.id,
              text: event.text,
              start: new DayPilot.Date(event.start),
              end: new DayPilot.Date(event.end),
              backColor: event.backColor || this.eventsService.getRandomColor(),
              resource: event.task_ID ?? event.course_ID ?? event.absenceDay_ID
            }));

            if (this.day && this.day.control) {
              this.day.control.update();
            }
            if (this.week && this.week.control) {
              this.week.control.update();
            }
            if (this.month && this.month.control) {
              this.month.control.update();
            }
            console.log(this.events);
          },
          error: (error) => {
            console.error('Error during loading of events:', error);
          }
        });
    }
  }
  viewDay(): void {
    this.configNavigator.selectMode = "Day";
    this.configDay.visible = true;
    this.configWeek.visible = false;
    this.configMonth.visible = false;
  }

  viewWeek(): void {
    this.configNavigator.selectMode = "Week";
    this.configDay.visible = false;
    this.configWeek.visible = true;
    this.configMonth.visible = false;
  }

  viewMonth(): void {
    this.configNavigator.selectMode = "Month";
    this.configDay.visible = false;
    this.configWeek.visible = false;
    this.configMonth.visible = true;
  }

  onBeforeEventRender(args: any) {
    const dp = args.control;
    args.data.areas = [
      {
        top: 3,
        right: 3,
        width: 20,
        height: 20,
        symbol: "/icons/daypilot.svg#minichevron-down-2",
        fontColor: "#fff",
        toolTip: "Show context menu",
        action: "ContextMenu",
      },
      {
        top: 3,
        right: 25,
        width: 20,
        height: 20,
        symbol: "/icons/daypilot.svg#x-circle",
        fontColor: "#fff",
        action: "None",
        toolTip: "Delete event",
        onClick: async (args: any) => {
          dp.events.remove(args.source);
        }
      }
    ];

    args.data.areas.push({
      bottom: 5,
      left: 5,
      width: 36,
      height: 36,
      action: "None",
      image: `https://picsum.photos/36/36?random=${args.data.id}`,
      style: "border-radius: 50%; border: 2px solid #fff; overflow: hidden;",
    });
  }
  //We disable this function to not allow creating evetns outside certain module
  /* async onTimeRangeSelected(args: any) {
    const modal = await DayPilot.Modal.prompt("Create a new event:", "Event 1");
    const dp = args.control;
    dp.clearSelection();
    if (!modal.result) {
      return;
    }
    dp.events.add(new DayPilot.Event({
      start: args.start,
      end: args.end,
      id: DayPilot.guid(),
      text: modal.result
    }));
  }*/

  //We disable direct editing in calendar, because we think is not suitable options
  //but left code if future developemnt thought is usefull
  async onEventClick(args: any) {
    /*const form = [
      {name: "Text", id: "text",readonly: true, editable: false},
      {name: "Start", id: "start", dateFormat: "MM/dd/yyyy", type: "datetime",readonly: true, editable: false},
      {name: "End", id: "end", dateFormat: "MM/dd/yyyy", type: "datetime",readonly: true,editable: false },
      {name: "Color", id: "backColor",readonly: true,editable: false},
    ];*/

    const data = args.e.data;

    /*const modal = await DayPilot.Modal.form(form, data);*/
    //const message = `Event name: ${data.text}<br>Start: ${data.start.toString()}<br>End: ${data.end.toString()}`;
    const message = `
      <div style="text-align: center">
        <div style="font-size: 24px">📅 ${data.text}</div>
        <div style="margin-top: 10px">
          <span>🕐 Start: ${data.start.toString()}</span><br>
          <span>🔚 End: ${data.end.toString()}</span>
        </div>
      </div>
    `;
    const modal = await DayPilot.Modal.alert(message);
    if (modal.canceled) {
      return;
    }

    const dp = args.control;

    //dp.events.update(modal.result);
  }
}
