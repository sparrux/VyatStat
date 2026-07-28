import { Routes } from '@angular/router';
import { bffAuthGuard } from '@vyatka-tracker/auth';

export const routes: Routes = [
  {
    path: 'calendar',
    loadComponent: () =>
      import('./pages/calendar-page/calendar-page').then((m) => m.CalendarPage),
    canActivate: [bffAuthGuard],
  },
  { path: '', pathMatch: 'full', redirectTo: 'calendar' },
  { path: '**', redirectTo: 'calendar' },
];
