import { Component, computed, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';

import { GroupSummary } from '../../../application/models/group.model';
import { UserFilterSelect } from '../user-filter-select/user-filter-select';
import { UserSummary } from '../../../application/models/user-summary.model';

@Component({
  selector: 'app-groups-table',
  imports: [RouterLink, UserFilterSelect],
  templateUrl: './groups-table.html',
  styleUrl: './groups-table.scss',
})
export class GroupsTable {
  readonly groups = input.required<GroupSummary[]>();
  readonly total = input.required<number>();
  readonly skip = input.required<number>();
  readonly pageSize = input.required<number>();
  readonly selectedUserId = input.required<string>();
  readonly selectedUserNickname = input.required<string>();
  readonly currentUserId = input.required<string>();
  readonly currentUserNickname = input.required<string>();

  readonly pageChange = output<number>();
  readonly memberFilterChange = output<UserSummary>();

  protected readonly hasPreviousPage = computed(() => this.skip() > 0);

  protected readonly hasNextPage = computed(
    () => this.skip() + this.groups().length < this.total(),
  );

  protected readonly rangeLabel = computed(() => {
    const total = this.total();
    if (total === 0) {
      return 'Нет групп';
    }

    const from = this.skip() + 1;
    const to = this.skip() + this.groups().length;
    return `Показано ${from}–${to} из ${total}`;
  });

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
}
