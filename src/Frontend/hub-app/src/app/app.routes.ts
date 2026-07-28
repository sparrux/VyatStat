import { Routes } from '@angular/router';
import { bffAuthGuard } from '@vyatka-tracker/auth';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/calendar-page/calendar-page').then((m) => m.CalendarPage),
    canActivate: [bffAuthGuard],
  },
  { path: '**', redirectTo: '' },
];
