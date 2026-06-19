import { Component, computed, input, output } from '@angular/core';
import { DashboardUser } from '../../services/users';

@Component({
  selector: 'app-users-table',
  standalone: true,
  templateUrl: './users-table.component.html',
  styleUrl: './users-table.component.scss',
})
export class UsersTableComponent {
  readonly users = input.required<DashboardUser[]>();
  readonly total = input.required<number>();
  readonly skip = input.required<number>();
  readonly pageSize = input.required<number>();
  readonly canUpdatePermissions = input(false);
  readonly canLockOutUsers = input(false);

  readonly pageChange = output<number>();
  readonly changeAccess = output<DashboardUser>();
  readonly blockUser = output<DashboardUser>();

  protected readonly hasPreviousPage = computed(() => this.skip() > 0);

  protected readonly hasNextPage = computed(
    () => this.skip() + this.users().length < this.total(),
  );

  protected readonly rangeLabel = computed(() => {
    const total = this.total();
    if (total === 0) {
      return 'No users';
    }

    const from = this.skip() + 1;
    const to = this.skip() + this.users().length;
    return `Showing ${from}–${to} of ${total}`;
  });

  protected displayValue(value: string | null | undefined): string {
    const trimmed = value?.trim();
    return trimmed ? trimmed : '—';
  }

  protected roleLabel(user: DashboardUser): string {
    return user.claims?.isAdmin ? 'Administrator' : 'User';
  }

  protected statusLabel(user: DashboardUser): string {
    return user.isLockedOut ? 'Blocked' : 'Active';
  }

  protected blockActionLabel(user: DashboardUser): string {
    return user.isLockedOut ? 'Unblock user' : 'Block user';
  }

  protected blockActionIcon(user: DashboardUser): string {
    return user.isLockedOut ? '/icons/unlock.png' : '/icons/lock.png';
  }

  protected blockActionButtonClass(user: DashboardUser): string {
    return user.isLockedOut ? 'vt-btn--table-unblock' : 'vt-btn--table-danger';
  }

  protected onPreviousPage(): void {
    if (!this.hasPreviousPage()) {
      return;
    }
    this.pageChange.emit(Math.max(0, this.skip() - this.pageSize()));
  }

  protected onNextPage(): void {
    if (!this.hasNextPage()) {
      return;
    }
    this.pageChange.emit(this.skip() + this.pageSize());
  }

  protected onMoreInfoClick(_user: DashboardUser): void {
    // Placeholder until user details page exists
  }

  protected onChangeAccessClick(user: DashboardUser): void {
    this.changeAccess.emit(user);
  }

  protected onBlockClick(user: DashboardUser): void {
    this.blockUser.emit(user);
  }
}
