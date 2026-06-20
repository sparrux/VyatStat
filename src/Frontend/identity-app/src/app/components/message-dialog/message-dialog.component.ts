import { Component, inject } from '@angular/core';
import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { DialogShellComponent } from '../dialog-shell/dialog-shell.component';
import { MessageDialogData } from '../../models/dialog.model';

@Component({
  selector: 'app-message-dialog',
  standalone: true,
  imports: [DialogShellComponent],
  templateUrl: './message-dialog.component.html',
  styleUrl: './message-dialog.component.scss',
})
export class MessageDialogComponent {
  readonly data = inject<MessageDialogData>(DIALOG_DATA);
  readonly dialogRef = inject(DialogRef<void, MessageDialogComponent>);

  protected onClose(): void {
    this.dialogRef.close();
  }
}
