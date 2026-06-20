import { Component, inject, OnInit, signal } from '@angular/core';
import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { firstValueFrom } from 'rxjs';
import { DialogShellComponent } from '../dialog-shell/dialog-shell.component';
import { UserClaims } from '../../models/auth.model';
import {
  UserPermissionsDialogData,
  UserPermissionsDialogResult,
} from '../../models/dialog.model';
import { AuthService } from '../../services/auth.service';
import { UsersService } from '../../services/users.service';

type PermissionKey = keyof Pick<
  UserClaims,
  'readUsers' | 'updateUserPermissions' | 'lockOutUsers'
>;

interface PermissionSwitch {
  key: PermissionKey;
  label: string;
  description: string;
}

const PERMISSION_SWITCHES: PermissionSwitch[] = [
  {
    key: 'readUsers',
    label: 'View users dashboard',
    description: 'Allows viewing the list of system users.',
  },
  {
    key: 'updateUserPermissions',
    label: 'Manage users permissions',
    description: 'Allows changing access rights of other users.',
  },
  {
    key: 'lockOutUsers',
    label: 'Block users',
    description: 'Allows blocking and unblocking other users.',
  },
];

@Component({
  selector: 'app-user-permissions-dialog',
  standalone: true,
  imports: [DialogShellComponent],
  templateUrl: './user-permissions-dialog.component.html',
  styleUrl: './user-permissions-dialog.component.scss',
})
export class UserPermissionsDialogComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly usersService = inject(UsersService);

  readonly data = inject<UserPermissionsDialogData>(DIALOG_DATA);
  readonly dialogRef = inject(
    DialogRef<UserPermissionsDialogResult | undefined, UserPermissionsDialogComponent>,
  );

  protected readonly permissionSwitches = PERMISSION_SWITCHES;
  protected readonly readUsers = signal(false);
  protected readonly updateUserPermissions = signal(false);
  protected readonly lockOutUsers = signal(false);
  protected readonly isAdmin = signal(false);
  protected readonly isLoading = signal(true);
  protected readonly isSaving = signal(false);
  protected readonly loadError = signal<string | null>(null);
  protected readonly saveError = signal<string | null>(null);

  protected readonly dialogTitle = signal('User permissions');

  async ngOnInit(): Promise<void> {
    const userName = this.data.user.userName?.trim();
    this.dialogTitle.set(
      userName ? `Permissions — ${userName}` : 'User permissions',
    );

    await this.loadPermissions();
  }

  protected isPermissionEnabled(key: PermissionKey): boolean {
    switch (key) {
      case 'readUsers':
        return this.readUsers();
      case 'updateUserPermissions':
        return this.updateUserPermissions();
      case 'lockOutUsers':
        return this.lockOutUsers();
    }
  }

  protected onPermissionChange(key: PermissionKey, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;

    switch (key) {
      case 'readUsers':
        this.readUsers.set(checked);
        if (!checked) {
          this.updateUserPermissions.set(false);
          this.lockOutUsers.set(false);
        }
        break;
      case 'updateUserPermissions':
        this.updateUserPermissions.set(checked);
        if (checked) {
          this.readUsers.set(true);
        }
        break;
      case 'lockOutUsers':
        this.lockOutUsers.set(checked);
        if (checked) {
          this.readUsers.set(true);
        }
        break;
    }
  }

  protected onCancel(): void {
    this.dialogRef.close();
  }

  protected async onSave(): Promise<void> {
    this.saveError.set(null);
    this.isSaving.set(true);

    try {
      const claims = await firstValueFrom(
        this.usersService.updateUserPermissions(this.data.user.id, {
          readUsers: this.readUsers(),
          updateUserPermissions: this.updateUserPermissions(),
          lockOutUsers: this.lockOutUsers(),
        }),
      );

      this.dialogRef.close({
        userId: this.data.user.id,
        claims,
      });
    } catch {
      this.saveError.set('Failed to update permissions. Please try again.');
    } finally {
      this.isSaving.set(false);
    }
  }

  private async loadPermissions(): Promise<void> {
    this.loadError.set(null);
    this.isLoading.set(true);

    try {
      const claims =
        this.data.user.claims ??
        (await firstValueFrom(this.auth.getUserPermissions(this.data.user.id)));

      this.readUsers.set(claims.readUsers);
      this.updateUserPermissions.set(claims.updateUserPermissions);
      this.lockOutUsers.set(claims.lockOutUsers);
      this.isAdmin.set(claims.isAdmin);
    } catch {
      this.loadError.set('Failed to load user permissions. Please try again.');
    } finally {
      this.isLoading.set(false);
    }
  }
}
