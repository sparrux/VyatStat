import { Routes } from '@angular/router';
import { authGuard } from '@vyatka-tracker/auth';

export const routes: Routes = [
  {
    path: 'callback',
    loadComponent: () =>
      import('./pages/callback-page/callback-page').then((m) => m.CallbackPage),
  },
  {
    path: 'calendar',
    loadComponent: () =>
      import('./pages/calendar-page/calendar-page').then((m) => m.CalendarPage),
    canActivate: [authGuard],
  },
  { path: '', pathMatch: 'full', redirectTo: 'calendar' },
  { path: '**', redirectTo: 'calendar' },
];
