import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppShellHeaderComponent } from './components/app-shell-header/app-shell-header.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, AppShellHeaderComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly title = signal('Identity.Web');
}
