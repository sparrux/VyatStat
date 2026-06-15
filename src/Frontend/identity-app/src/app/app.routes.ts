import { Routes } from '@angular/router';
import { CallbackComponent } from './components/callback/callback';

export const routes: Routes = [
  { path: 'callback', component: CallbackComponent },
  { path: '', pathMatch: 'full', redirectTo: 'callback' },
  { path: '**', redirectTo: 'callback' },
];
