import { Component, OnInit, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { AuthenticationService } from '../../../../application/services/authentication.service';
import { GroupService } from '../../../../application/services/group.service';
import { GroupSummary } from '../../../../application/models/group.model';
import { UserSummary } from '../../../../application/models/user-summary.model';
import { GroupsTable } from '../../../components/groups-table/groups-table';

@Component({
  selector: 'app-my-groups-page',
  imports: [GroupsTable],
  templateUrl: './my-groups-page.html',
  styleUrl: './my-groups-page.scss',
})
export class MyGroupsPage implements OnInit {
  private readonly auth = inject(AuthenticationService);
  private readonly groupService = inject(GroupService);

  protected readonly groups = signal<GroupSummary[]>([]);
  protected readonly total = signal(0);
  protected readonly skip = signal(0);
  protected readonly selectedMember = signal<UserSummary | null>(null);
  protected readonly loadError = signal<string | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly isGroupsLoading = signal(false);

  protected readonly pageSize = this.groupService.pageSize;

  protected get currentUser(): UserSummary | null {
    const user = this.auth.user();
    return user ? { id: user.id, nickname: user.nickname } : null;
  }

  async ngOnInit(): Promise<void> {
    const current = this.currentUser;
    if (!current) {
      this.loadError.set('Не удалось определить текущего пользователя.');
      this.isLoading.set(false);
      return;
    }

    this.selectedMember.set(current);
    await this.loadGroups();
    this.isLoading.set(false);
  }

  protected reload(): void {
    void this.loadGroups(true);
  }

  protected onPageChange(nextSkip: number): void {
    this.skip.set(nextSkip);
    void this.loadGroups(true);
  }

  protected onMemberFilterChange(user: UserSummary): void {
    this.selectedMember.set(user);
    this.skip.set(0);
    void this.loadGroups(true);
  }

  private async loadGroups(showLoading = false): Promise<void> {
    const member = this.selectedMember();
    if (!member) {
      return;
    }

    this.loadError.set(null);
    if (showLoading) {
      this.isGroupsLoading.set(true);
    }

    try {
      const response = await firstValueFrom(
        this.groupService.getList({
          memberUserId: member.id,
          take: this.pageSize,
          skip: this.skip(),
        }),
      );
      this.groups.set(response.values);
      this.total.set(response.total);
    } catch {
      this.loadError.set('Не удалось загрузить группы. Попробуйте ещё раз.');
    } finally {
      if (showLoading) {
        this.isGroupsLoading.set(false);
      }
    }
  }
}
