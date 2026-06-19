import { inject, Injectable, Type } from '@angular/core';
import { Dialog, DialogConfig, DialogRef } from '@angular/cdk/dialog';
import {
  MessageDialogComponent,
  MessageDialogData,
} from '../components/message-dialog/message-dialog.component';

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

  openMessage(
    message: string,
    title?: string,
  ): DialogRef<void, MessageDialogComponent> {
    return this.open(MessageDialogComponent, {
      data: { message, title } satisfies MessageDialogData,
    });
  }
}
