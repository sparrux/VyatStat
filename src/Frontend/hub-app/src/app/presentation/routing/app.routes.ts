import { Routes } from '@angular/router';

import { authGuard } from '../guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('../pages/main-page/main-page').then((m) => m.MainPage),
    canActivate: [authGuard],
  },
  { path: '**', redirectTo: '' },
];
