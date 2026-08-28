import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { HubHeader } from '../../components/hub-header/hub-header';

@Component({
  selector: 'app-root',
  imports: [HubHeader, RouterOutlet],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.scss',
})
export class AppShell {}
