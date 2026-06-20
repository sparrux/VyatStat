import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppFooterComponent } from './components/app-footer/app-footer.component';
import { AppShellHeaderComponent } from './components/app-shell-header/app-shell-header.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, AppShellHeaderComponent, AppFooterComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {}
