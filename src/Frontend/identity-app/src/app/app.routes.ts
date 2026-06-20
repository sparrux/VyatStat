import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { readUsersGuard } from './guards/read-users.guard';

export const routes: Routes = [
  {
    path: 'callback',
    loadComponent: () =>
      import('./pages/callback-page/callback-page.component').then(
        (m) => m.CallbackPageComponent,
      ),
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login-page/login-page.component').then((m) => m.LoginPageComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./pages/register-page/register-page.component').then(
        (m) => m.RegisterPageComponent,
      ),
  },
  {
    path: 'account',
    loadComponent: () =>
      import('./pages/account-page/account-page.component').then(
        (m) => m.AccountPageComponent,
      ),
    canActivate: [authGuard],
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./pages/dashboard-page/dashboard-page.component').then(
        (m) => m.DashboardPageComponent,
      ),
    canActivate: [authGuard, readUsersGuard],
  },
  { path: '', pathMatch: 'full', redirectTo: 'account' },
  { path: '**', redirectTo: 'account' },
];
