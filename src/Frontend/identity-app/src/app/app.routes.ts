import { Routes } from '@angular/router';
import { CallbackComponent } from './components/callback/callback';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { RegisterPlaceholderComponent } from './pages/register-placeholder/register-placeholder.component';

export const routes: Routes = [
  { path: 'callback', component: CallbackComponent },
  { path: 'login', component: LoginPageComponent },
  { path: 'register', component: RegisterPlaceholderComponent },
  { path: '', pathMatch: 'full', redirectTo: 'callback' },
  { path: '**', redirectTo: 'callback' },
];
