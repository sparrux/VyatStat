import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { CallbackComponent } from './components/callback/callback';
import { RegisterComponent } from './components/register/register'; // 🟢 Импортируем

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent }, // 🟢 Добавляем маршрут
  { path: 'callback', component: CallbackComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];
