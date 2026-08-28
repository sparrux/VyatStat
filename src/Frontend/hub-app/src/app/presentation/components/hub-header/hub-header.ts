import { Component, computed, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { AuthenticationService } from '../../../application/services/authentication.service';
import { DialogService } from '../../services/dialog.service';
import { displayInitials } from '../../shared/utils/display.utils';

@Component({
  selector: 'app-hub-header',
  imports: [RouterLink],
  templateUrl: './hub-header.html',
  styleUrl: './hub-header.scss',
})
export class HubHeader {
  private readonly auth = inject(AuthenticationService);
  private readonly dialogs = inject(DialogService);
  private readonly router = inject(Router);

  protected readonly displayInitials = displayInitials;
  protected readonly user = this.auth.user;
  protected readonly userInitials = computed(() =>
    displayInitials(this.user()?.nickname),
  );

  protected async openCreateGroup(): Promise<void> {
    const group = await this.dialogs.openCreateGroup();
    if (!group) {
      return;
    }

    await this.router.navigate(['/groups', group.id]);
  }

  protected async openCreateEvent(): Promise<void> {
    const result = await this.dialogs.openCreateEvent();
    if (!result) {
      return;
    }

    await this.router.navigate(['/account']);
  }
}
