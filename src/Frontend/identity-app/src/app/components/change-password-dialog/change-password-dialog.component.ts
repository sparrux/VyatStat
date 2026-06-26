import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '@angular/cdk/dialog';
import { firstValueFrom } from 'rxjs';
import { DialogShellComponent } from '../dialog-shell/dialog-shell.component';
import { AuthService } from '@vyatka-tracker/auth';

const MIN_PASSWORD_LENGTH = 6;

@Component({
  selector: 'app-change-password-dialog',
  standalone: true,
  imports: [DialogShellComponent, FormsModule],
  templateUrl: './change-password-dialog.component.html',
  styleUrl: './change-password-dialog.component.scss',
})
export class ChangePasswordDialogComponent {
  private readonly auth = inject(AuthService);

  readonly dialogRef = inject(DialogRef<boolean, ChangePasswordDialogComponent>);

  protected readonly currentPassword = signal('');
  protected readonly newPassword = signal('');
  protected readonly isSaving = signal(false);
  protected readonly saveError = signal<string | null>(null);

  protected onCancel(): void {
    this.dialogRef.close(false);
  }

  protected async onSave(): Promise<void> {
    this.saveError.set(null);

    const current = this.currentPassword().trim();
    const next = this.newPassword().trim();

    if (!current || !next) {
      this.saveError.set('Enter your current password and a new password.');
      return;
    }

    if (next.length < MIN_PASSWORD_LENGTH) {
      this.saveError.set(`New password must be at least ${MIN_PASSWORD_LENGTH} characters.`);
      return;
    }

    if (current === next) {
      this.saveError.set('New password must be different from the current password.');
      return;
    }

    this.isSaving.set(true);

    try {
      await firstValueFrom(
        this.auth.updatePassword({
          currentPassword: current,
          newPassword: next,
        }),
      );
      this.dialogRef.close(true);
    } catch {
      this.saveError.set('Failed to change password. Check your current password and try again.');
    } finally {
      this.isSaving.set(false);
    }
  }
}
