import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DayPilotModule } from '@daypilot/daypilot-lite-angular';
import { CalendarRoutingModule } from './calendar-routing.module';
import { HttpClientModule } from '@angular/common/http';
import { CalendarComponent } from './components/calendar/calendar.component';
import { EventsService } from './services/events.service';

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
