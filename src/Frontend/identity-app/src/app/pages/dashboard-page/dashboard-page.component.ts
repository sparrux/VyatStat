import {
  Component,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { UsersTableComponent } from '../../components/users-table/users-table.component';
import { UserClaims } from '../../models/auth.model';
import { DashboardUser } from '../../models/user.model';
import { AuthService } from '../../services/auth.service';
import { DialogService } from '../../services/dialog.service';
import { UsersService } from '../../services/users.service';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [UsersTableComponent],
  templateUrl: './dashboard-page.component.html',
  styleUrl: './dashboard-page.component.scss',
})
export class DashboardPageComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly usersService = inject(UsersService);
  private readonly dialog = inject(DialogService);

  protected readonly claims = signal<UserClaims | null>(null);
  protected readonly users = signal<DashboardUser[]>([]);
  protected readonly total = signal(0);
  protected readonly skip = signal(0);
  protected readonly loadError = signal<string | null>(null);
  protected readonly usersLoadError = signal<string | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly isUsersLoading = signal(false);

  protected readonly pageSize = this.usersService.pageSize;

  async ngOnInit(): Promise<void> {
    await this.loadPageData();
  }

  protected reload(): void {
    void this.loadPageData();
  }

  protected onPageChange(nextSkip: number): void {
    this.skip.set(nextSkip);
    void this.loadUsers(true);
  }

  protected onChangeAccess(user: DashboardUser): void {
    const dialogRef = this.dialog.openUserPermissions(user);

    dialogRef.closed.subscribe((result) => {
      if (!result) {
        return;
      }

      this.users.update((users) =>
        users.map((item) =>
          item.id === result.userId
            ? { ...item, claims: result.claims }
            : item,
        ),
      );
    });
  }

  protected async onBlockUser(user: DashboardUser): Promise<void> {
    const nextLocked = !user.isLockedOut;
    const action = nextLocked ? 'block' : 'unblock';
    const displayName = user.userName?.trim() || 'this user';

    const confirmed = await this.dialog.openConfirm(
      `Are you sure you want to ${action} ${displayName}?`,
      'Confirm action',
    );

    if (!confirmed) {
      return;
    }

    try {
      await firstValueFrom(this.usersService.setUserLockOut(user.id, nextLocked));
      this.users.update((users) =>
        users.map((item) =>
          item.id === user.id ? { ...item, isLockedOut: nextLocked } : item,
        ),
      );
    } catch {
      this.dialog.openMessage(
        `Failed to ${action} user. Please try again.`,
        'Error',
      );
    }
  }

  private async loadPageData(): Promise<void> {
    this.loadError.set(null);
    this.isLoading.set(true);

    try {
      const profile = await firstValueFrom(this.auth.getProfile());
      const perm = await firstValueFrom(this.auth.getUserPermissions(profile.id));
      this.claims.set(perm);
      await this.loadUsers();
    } catch {
      this.loadError.set('Failed to load the users dashboard. Please try again later.');
    } finally {
      this.isLoading.set(false);
    }
  }

  private async loadUsers(showLoading = false): Promise<void> {
    this.usersLoadError.set(null);
    if (showLoading) {
      this.isUsersLoading.set(true);
    }

    try {
      const response = await firstValueFrom(
        this.usersService.getUsers(this.skip(), this.pageSize),
      );
      this.users.set(response.users);
      this.total.set(response.total);
    } catch {
      this.usersLoadError.set('Failed to load users. Please try again later.');
    } finally {
      if (showLoading) {
        this.isUsersLoading.set(false);
      }
    }
  }
}
