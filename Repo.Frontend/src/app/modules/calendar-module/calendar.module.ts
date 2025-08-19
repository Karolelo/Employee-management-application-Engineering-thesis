import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DayPilotModule } from '@daypilot/daypilot-lite-angular';
import { EventsService } from './events.service';
import {CalendarComponent} from './calendar/calendar.component';
import {CalendarRoutingModule} from './calendar-routing.module';
import {HttpClient, HttpClientModule} from '@angular/common/http';

@NgModule({
  declarations: [
    CalendarComponent
  ],
  imports: [
    CommonModule,
    DayPilotModule,
    CalendarRoutingModule,
    HttpClientModule,
  ],
  providers: [
    EventsService
  ],
  exports: [
    CalendarComponent
  ]
})
export class CalendarModule { }
