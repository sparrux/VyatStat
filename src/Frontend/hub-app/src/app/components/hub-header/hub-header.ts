import { Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BffAuthService } from '@vyatka-tracker/auth';

import { displayInitials } from '../../utils/display.utils';

@Component({
  selector: 'app-hub-header',
  imports: [RouterLink],
  templateUrl: './hub-header.html',
  styleUrl: './hub-header.scss',
})
export class HubHeader {
  private readonly auth = inject(BffAuthService);

  protected readonly displayInitials = displayInitials;
  protected readonly user = this.auth.user;
  protected readonly userInitials = computed(() =>
    displayInitials(this.user()?.nickname),
  );
}
