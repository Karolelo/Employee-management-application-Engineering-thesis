import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {ReactiveFormsModule, FormsModule} from '@angular/forms';
import { MatSidenavModule } from '@angular/material/sidenav';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './modules/login/login.component';
import { NavbarComponent } from './common_components/navbar/navbar.component';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import {MatInputModule, MatLabel} from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { MatDividerModule } from '@angular/material/divider';
import { MatTableModule } from '@angular/material/table';
import {MatIcon} from '@angular/material/icon';
import {CalendarModule} from './modules/calendar-module/calendar.module';
import {HTTP_INTERCEPTORS} from '@angular/common/http';
import {AuthInterceptorService} from './common_services/auth-interceptor-services.service';
import {TaskModule} from './modules/task/task.module';
import {JwtModule} from '@auth0/angular-jwt';
import {MatListItem, MatNavList} from '@angular/material/list';
import {CommonModule, NgClass,NgOptimizedImage,IMAGE_LOADER} from '@angular/common';
import {SidenavbarComponent} from './common_components/sidenavbar/sidenavbar.component';
import {UserDetailsNavComponent} from './common_components/user-details-nav/user-details-nav.component';
import {MatMenu, MatMenuItem, MatMenuTrigger} from '@angular/material/menu';
import {MatToolbar} from '@angular/material/toolbar';
import {MatStepperModule} from '@angular/material/stepper';
import { NotFoundPage404Component } from './common_components/not-found-page404/not-found-page404.component';
import { ExportImageComponent } from './common_components/export-image/export-image.component';
import { DebugComponent } from './debug_component/app-debug/app-debug.component';
export function tokenGetter() {
  return localStorage.getItem('auth_token');
}
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    NavbarComponent,
    AuthLayoutComponent,
    MainLayoutComponent,
    SidenavbarComponent,
    UserDetailsNavComponent,
  ],
  imports: [
    CommonModule,
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    MatSidenavModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    FormsModule,
    MatOptionModule,
    MatDividerModule,
    MatTableModule,
    MatIcon,
    CalendarModule,
    TaskModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: ["localhost:4200"],
        disallowedRoutes: []
      }
    }),
    MatNavList,
    MatListItem,
    MatIcon,
    MatSidenavModule,
    NgClass,
    MatMenu,
    MatToolbar,
    MatMenuTrigger,
    MatMenuItem,
    MatLabel,
    MatSelectModule,
    MatStepperModule,
    NgOptimizedImage,
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptorService,
      multi: true
    }
  ],
  exports: [
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
