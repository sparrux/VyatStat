import {
  Component,
  computed,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthService, UserClaims, UserProfile } from '../../services/auth';

@Component({
  selector: 'app-account-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './account-page.component.html',
  styleUrl: './account-page.component.scss',
})
export class AccountPageComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly profile = signal<UserProfile | null>(null);

  protected readonly claims = signal<UserClaims | null>(null);
  protected readonly loadError = signal<string | null>(null);
  protected readonly isLoading = signal(true);

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
    if (c.isAdmin) {
      return [
        'View users dashboard',
        'Manage users',
        'Configure user access rights',
        'Configure system settings',
      ];
    }
    const items: string[] = [];
    if (c.readUsers) {
      items.push('View users dashboard', 'Manage users');
    }
    if (c.updateUserPermissions) {
      items.push('Configure user access rights');
    }
    if (items.length === 0) {
      items.push('Use your personal account features');
    }
    return items;
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

  protected displayOrNull(value: string | null | undefined): string {
    const trimmed = value?.trim();
    return trimmed ? trimmed : 'null';
  }

  protected displayInitials(userName: string | null | undefined): string {
    const name = userName?.trim();
    if (!name) {
      return '?';
    }
    const parts = name.split(/\s+/).filter(Boolean);
    if (parts.length >= 2) {
      return `${parts[0]![0]!}${parts[1]![0]!}`.toUpperCase();
    }
    return name.slice(0, 2).toUpperCase();
  }

  protected onLogout(): void {
    this.auth.logout();
    void this.router.navigate(['/login']);
  }

  protected onChangeEmailClick(): void {
    // Placeholder until email change flow exists
  }

  protected onUpdatePhotoClick(): void {
    // Placeholder until photo upload exists
  }
}
