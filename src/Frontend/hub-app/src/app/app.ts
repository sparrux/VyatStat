import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { HubHeader } from './components/hub-header/hub-header';

@Component({
  selector: 'app-root',
  imports: [HubHeader, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {}
