import {
  Component,
  computed,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { UserClaims, UserProfile } from '../../models/auth.model';
import { AuthService } from '../../services/auth.service';
import { DialogService } from '../../services/dialog.service';
import { displayInitials, displayOrNull } from '../../utils/display.utils';

@Component({
  selector: 'app-account-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './account-page.component.html',
  styleUrl: './account-page.component.scss',
})
export class AccountPageComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly dialog = inject(DialogService);

  protected readonly profile = signal<UserProfile | null>(null);

  protected readonly claims = signal<UserClaims | null>(null);
  protected readonly loadError = signal<string | null>(null);
  protected readonly isLoading = signal(true);

  protected readonly displayInitials = displayInitials;
  protected readonly displayOrNull = displayOrNull;

  protected readonly roleLabel = computed(() =>
    this.claims()?.isAdmin ? 'Administrator' : 'User',
  );

  protected readonly roleDescription = computed(() => {
    const c = this.claims();
    if (!c) {
      return '';
    }
    if (c.isAdmin) {
      return 'You have the maximum permissions in your system. You can manage users and access rights.';
    }
    return 'You are signed in with a standard account. Available actions depend on the permissions assigned to you.';
  });

  protected readonly opportunities = computed(() => {
    const c = this.claims();

    if (!c) {
      return [];
    }

    const opportunities: string[] = [];

    if (c.readUsers) {
      opportunities.push('View users dashboard');
    }
    if (c.updateUserPermissions) {
      opportunities.push('Manage users permissions');
    }
    if (c.lockOutUsers) {
      opportunities.push('Block and unblock users');
    }
    if (opportunities.length === 0) {
      opportunities.push('Use your personal account features');
    }
    return opportunities;
  });

  async ngOnInit(): Promise<void> {
    await this.loadAccountData();
  }

  protected reload(): void {
    void this.loadAccountData();
  }

  private async loadAccountData(): Promise<void> {
    this.loadError.set(null);
    this.isLoading.set(true);
    try {
      const p = await firstValueFrom(this.auth.getProfile());
      this.profile.set(p);
      const perm = await firstValueFrom(this.auth.getUserPermissions(p.id));
      this.claims.set(perm);
    } catch {
      this.loadError.set('Failed to load your profile. Please try again later.');
    } finally {
      this.isLoading.set(false);
    }
  }

  protected onChangeEmailClick(): void {
    // Placeholder until email change flow exists
  }

  protected onUpdatePhotoClick(): void {
    // Placeholder until photo upload exists
  }

  protected onChangePasswordClick(): void {
    const dialogRef = this.dialog.openChangePassword();

    dialogRef.closed.subscribe((changed) => {
      if (!changed) {
        return;
      }

      this.dialog.openMessage('Your password has been updated.', 'Password changed');
    });
  }
}
