import { Dialog, DialogConfig, DialogRef } from '@angular/cdk/dialog';
import { inject, Injectable, Type } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ChangePasswordDialogComponent } from '../components/change-password-dialog/change-password-dialog.component';
import { ConfirmDialogComponent } from '../components/confirm-dialog/confirm-dialog.component';
import { MessageDialogComponent } from '../components/message-dialog/message-dialog.component';
import { UserPermissionsDialogComponent } from '../components/user-permissions-dialog/user-permissions-dialog.component';
import {
  ConfirmDialogData,
  MessageDialogData,
  UserPermissionsDialogData,
  UserPermissionsDialogResult,
} from '../models/dialog.model';
import { DashboardUser } from '../models/user.model';

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

  openConfirm(
    message: string,
    title = 'Confirm',
  ): Promise<boolean> {
    const dialogRef = this.open(ConfirmDialogComponent, {
      data: { message, title } satisfies ConfirmDialogData,
      maxWidth: '28rem',
    });

    return firstValueFrom(dialogRef.closed).then((result) => result === true);
  }

  openUserPermissions(
    user: DashboardUser,
  ): DialogRef<UserPermissionsDialogResult | undefined, UserPermissionsDialogComponent> {
    return this.open(UserPermissionsDialogComponent, {
      data: { user } satisfies UserPermissionsDialogData,
      maxWidth: '32rem',
    });
  }

  openChangePassword(): DialogRef<boolean | undefined, ChangePasswordDialogComponent> {
    return this.open(ChangePasswordDialogComponent, {
      maxWidth: '28rem',
    });
  }
}
