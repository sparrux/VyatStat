import { Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { AuthenticationService } from '../../../application/services/authentication.service';
import { displayInitials } from '../../shared/utils/display.utils';

@Component({
  selector: 'app-hub-header',
  imports: [RouterLink],
  templateUrl: './hub-header.html',
  styleUrl: './hub-header.scss',
})
export class HubHeader {
  private readonly auth = inject(AuthenticationService);

  protected readonly displayInitials = displayInitials;
  protected readonly user = this.auth.user;
  protected readonly userInitials = computed(() =>
    displayInitials(this.user()?.nickname),
  );
}
