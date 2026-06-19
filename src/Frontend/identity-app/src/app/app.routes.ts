import { Routes } from '@angular/router';
import { CallbackComponent } from './components/callback/callback';
import { authGuard } from './guards/auth.guard';
import { readUsersGuard } from './guards/read-users.guard';
import { AccountPageComponent } from './pages/account-page/account-page.component';
import { DashboardPageComponent } from './pages/dashboard-page/dashboard-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { RegisterPageComponent } from './pages/register-page/register-page.component';

export const routes: Routes = [
  { path: 'callback', component: CallbackComponent },
  { path: 'login', component: LoginPageComponent },
  { path: 'register', component: RegisterPageComponent },
  {
    path: 'account',
    component: AccountPageComponent,
    canActivate: [authGuard],
  },
  {
    path: 'dashboard',
    component: DashboardPageComponent,
    canActivate: [authGuard, readUsersGuard],
  },
  { path: '', pathMatch: 'full', redirectTo: 'account' },
  { path: '**', redirectTo: 'account' },
];
