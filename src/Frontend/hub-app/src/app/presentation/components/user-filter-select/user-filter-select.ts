import {
  Component,
  ElementRef,
  HostListener,
  OnInit,
  computed,
  inject,
  input,
  output,
  signal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';

import { UserService } from '../../../application/services/user.service';
import { UserSummary } from '../../../application/models/user-summary.model';

@Component({
  selector: 'app-user-filter-select',
  imports: [FormsModule],
  templateUrl: './user-filter-select.html',
  styleUrl: './user-filter-select.scss',
})
export class UserFilterSelect implements OnInit {
  private readonly userService = inject(UserService);
  private readonly host = inject(ElementRef<HTMLElement>);

  readonly selectedUserId = input.required<string>();
  readonly selectedUserNickname = input.required<string>();
  readonly currentUserId = input.required<string>();
  readonly currentUserNickname = input.required<string>();

  readonly selectionChange = output<UserSummary>();

  protected readonly isOpen = signal(false);
  protected readonly isLoading = signal(false);
  protected readonly loadError = signal<string | null>(null);
  protected readonly users = signal<UserSummary[]>([]);
  protected readonly total = signal(0);
  protected readonly skip = signal(0);
  protected readonly searchStub = signal('');

  protected readonly pageSize = this.userService.pageSize;

  protected readonly selectedLabel = computed(() => {
    if (this.selectedUserId() === this.currentUserId()) {
      return 'Вы';
    }

    return this.selectedUserNickname() || 'Пользователь';
  });

  protected readonly hasPreviousPage = computed(() => this.skip() > 0);

  protected readonly hasNextPage = computed(
    () => this.skip() + this.users().length < this.total(),
  );

  protected readonly rangeLabel = computed(() => {
    const total = this.total();
    if (total === 0) {
      return 'Нет пользователей';
    }

    const from = this.skip() + 1;
    const to = this.skip() + this.users().length;
    return `${from}–${to} из ${total}`;
  });

  async ngOnInit(): Promise<void> {
    await this.loadUsers();
  }

  @HostListener('document:click', ['$event'])
  protected onDocumentClick(event: MouseEvent): void {
    if (!this.isOpen()) {
      return;
    }

    const target = event.target as Node | null;
    if (target && !this.host.nativeElement.contains(target)) {
      this.isOpen.set(false);
    }
  }

  protected async toggleOpen(): Promise<void> {
    const next = !this.isOpen();
    this.isOpen.set(next);

    if (next && this.users().length === 0) {
      await this.loadUsers();
    }
  }

  protected selectUser(user: UserSummary): void {
    this.selectionChange.emit(user);
    this.isOpen.set(false);
  }

  protected selectCurrentUser(): void {
    this.selectUser({
      id: this.currentUserId(),
      nickname: this.currentUserNickname(),
    });
  }

  protected async onPreviousPage(): Promise<void> {
    if (!this.hasPreviousPage()) {
      return;
    }

    this.skip.set(Math.max(0, this.skip() - this.pageSize));
    await this.loadUsers();
  }

  protected async onNextPage(): Promise<void> {
    if (!this.hasNextPage()) {
      return;
    }

    this.skip.set(this.skip() + this.pageSize);
    await this.loadUsers();
  }

  protected displayNickname(user: UserSummary): string {
    if (user.id === this.currentUserId()) {
      return `${user.nickname} (вы)`;
    }

    return user.nickname;
  }

  private async loadUsers(): Promise<void> {
    this.isLoading.set(true);
    this.loadError.set(null);

    try {
      const response = await firstValueFrom(
        this.userService.getList({
          take: this.pageSize,
          skip: this.skip(),
        }),
      );
      this.users.set(response.values);
      this.total.set(response.total);
    } catch {
      this.loadError.set('Не удалось загрузить пользователей.');
    } finally {
      this.isLoading.set(false);
    }
  }
}
