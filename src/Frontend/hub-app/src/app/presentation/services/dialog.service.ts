import { Dialog, DialogConfig, DialogRef } from '@angular/cdk/dialog';
import { Injectable, Type, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { CreateEventDraftResult } from '../../application/models/event.model';
import { GroupSummary } from '../../application/models/group.model';
import { CreateEventDialog } from '../components/create-event-dialog/create-event-dialog';
import { CreateGroupDialog } from '../components/create-group-dialog/create-group-dialog';

const DEFAULT_DIALOG_CONFIG = {
  panelClass: 'vt-dialog-panel',
  backdropClass: 'vt-dialog-backdrop',
  hasBackdrop: true,
} satisfies DialogConfig;

@Injectable({ providedIn: 'root' })
export class DialogService {
  private readonly dialog = inject(Dialog);

  open<C, D = unknown, R = unknown>(
    component: Type<C>,
    config?: DialogConfig<D, DialogRef<R, C>>,
  ): DialogRef<R, C> {
    return this.dialog.open<R, D, C>(component, {
      ...DEFAULT_DIALOG_CONFIG,
      ...config,
    });
  }

  openCreateGroup(): Promise<GroupSummary | undefined> {
    const dialogRef = this.open<
      CreateGroupDialog,
      unknown,
      GroupSummary | undefined
    >(CreateGroupDialog, {
      minWidth: '25rem',
      maxWidth: '28rem',
    });

    return firstValueFrom(dialogRef.closed);
  }

  openCreateEvent(): Promise<CreateEventDraftResult | undefined> {
    const dialogRef = this.open<
      CreateEventDialog,
      unknown,
      CreateEventDraftResult | undefined
    >(CreateEventDialog, {
      minWidth: '28rem',
      maxWidth: '36rem',
    });

    return firstValueFrom(dialogRef.closed);
  }
}
