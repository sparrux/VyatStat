import { Component, inject } from '@angular/core';
import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { DialogShellComponent } from '../dialog-shell/dialog-shell.component';
import { ConfirmDialogData } from '../../models/dialog.model';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [DialogShellComponent],
  templateUrl: './confirm-dialog.component.html',
  styleUrl: './confirm-dialog.component.scss',
})
export class ConfirmDialogComponent {
  readonly data = inject<ConfirmDialogData>(DIALOG_DATA);
  readonly dialogRef = inject(DialogRef<boolean | undefined, ConfirmDialogComponent>);

  protected onConfirm(): void {
    this.dialogRef.close(true);
  }

  protected onCancel(): void {
    this.dialogRef.close(false);
  }
}
