import { DialogRef } from '@angular/cdk/dialog';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';

import { AuthenticationService } from '../../../application/services/authentication.service';
import { EventService } from '../../../application/services/event.service';
import { GroupService } from '../../../application/services/group.service';
import {
  CreateEventDraftResult,
  CreateEventLocationInput,
} from '../../../application/models/event.model';
import { GroupSummary } from '../../../application/models/group.model';
import { TextFormat } from '../../../application/models/rich-text.model';
import {
  addDays,
  fromDateTimeLocalValue,
  toDateTimeLocalValue,
} from '../../shared/utils/date-window.utils';
import { DialogShell } from '../../shared/components/dialog-shell/dialog-shell';

@Component({
  selector: 'app-create-event-dialog',
  imports: [DialogShell, FormsModule],
  templateUrl: './create-event-dialog.html',
  styleUrl: './create-event-dialog.scss',
})
export class CreateEventDialog implements OnInit {
  private readonly auth = inject(AuthenticationService);
  private readonly eventService = inject(EventService);
  private readonly groupService = inject(GroupService);
  private readonly dialogRef = inject(
    DialogRef<CreateEventDraftResult | undefined, CreateEventDialog>,
  );

  protected readonly groups = signal<GroupSummary[]>([]);
  protected readonly groupId = signal('');
  protected readonly title = signal('');
  protected readonly startDate = signal(toDateTimeLocalValue(new Date()));
  protected readonly endDate = signal(
    toDateTimeLocalValue(addDays(new Date(), 1)),
  );
  protected readonly description = signal('');
  protected readonly locationName = signal('');
  protected readonly latitude = signal('');
  protected readonly longitude = signal('');

  protected readonly isLoadingGroups = signal(true);
  protected readonly isSaving = signal(false);
  protected readonly saveError = signal<string | null>(null);

  async ngOnInit(): Promise<void> {
    await this.loadGroups();
  }

  protected onCancel(): void {
    this.dialogRef.close(undefined);
  }

  protected onCoordinateChange(
    field: 'latitude' | 'longitude',
    value: string | number | null,
  ): void {
    const next = value == null || value === '' ? '' : `${value}`;
    if (field === 'latitude') {
      this.latitude.set(next);
      return;
    }

    this.longitude.set(next);
  }

  protected async onCreate(): Promise<void> {
    this.saveError.set(null);

    const title = this.title().trim();
    const groupId = this.groupId();
    const start = fromDateTimeLocalValue(this.startDate());
    const end = fromDateTimeLocalValue(this.endDate());
    const description = this.description().trim();
    const location = this.parseLocation();

    if (!groupId) {
      this.saveError.set('Выберите группу.');
      return;
    }

    if (!title) {
      this.saveError.set('Введите название события.');
      return;
    }

    if (!start || !end) {
      this.saveError.set('Укажите даты начала и окончания.');
      return;
    }

    if (start >= end) {
      this.saveError.set('Дата начала должна быть раньше даты окончания.');
      return;
    }

    if (location === 'invalid') {
      this.saveError.set(
        'Для места укажите широту и долготу числом, либо очистите поля.',
      );
      return;
    }

    this.isSaving.set(true);

    try {
      const created = await firstValueFrom(
        this.eventService.create({
          title,
          startDate: start.toISOString(),
          endDate: end.toISOString(),
        }),
      );

      if (description) {
        await firstValueFrom(
          this.eventService.updateDescription(created.id, {
            text: description,
            format: TextFormat.PlainText,
          }),
        );
      }

      if (location) {
        await firstValueFrom(
          this.eventService.updateLocation(created.id, location),
        );
      }

      await firstValueFrom(this.groupService.attachEvent(groupId, created.id));

      this.dialogRef.close({ event: created, groupId });
    } catch {
      this.saveError.set(
        'Не удалось создать событие. Проверьте данные и попробуйте ещё раз.',
      );
    } finally {
      this.isSaving.set(false);
    }
  }

  private parseLocation(): CreateEventLocationInput | null | 'invalid' {
    const name = this.locationName().trim();
    const latRaw = this.latitude().trim();
    const lngRaw = this.longitude().trim();

    if (!name && !latRaw && !lngRaw) {
      return null;
    }

    const latitude = Number(latRaw);
    const longitude = Number(lngRaw);

    if (!Number.isFinite(latitude) || !Number.isFinite(longitude)) {
      return 'invalid';
    }

    return {
      name: name || null,
      latitude,
      longitude,
    };
  }

  private async loadGroups(): Promise<void> {
    const user = this.auth.user();
    if (!user) {
      this.saveError.set('Не удалось определить текущего пользователя.');
      this.isLoadingGroups.set(false);
      return;
    }

    this.isLoadingGroups.set(true);

    try {
      const response = await firstValueFrom(
        this.groupService.getList({
          memberUserId: user.id,
          take: 30,
          skip: 0,
        }),
      );

      this.groups.set(response.values);

      if (response.values.length > 0) {
        this.groupId.set(response.values[0].id);
      }
    } catch {
      this.saveError.set('Не удалось загрузить список групп.');
    } finally {
      this.isLoadingGroups.set(false);
    }
  }
}
