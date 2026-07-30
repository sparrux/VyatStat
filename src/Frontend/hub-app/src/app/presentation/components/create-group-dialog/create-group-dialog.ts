import { DialogRef } from '@angular/cdk/dialog';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';

import { GroupService } from '../../../application/services/group.service';
import { GroupSummary } from '../../../application/models/group.model';
import { DialogShell } from '../../shared/components/dialog-shell/dialog-shell';

const MAX_GROUP_NAME_LENGTH = 200;

@Component({
  selector: 'app-create-group-dialog',
  imports: [DialogShell, FormsModule],
  templateUrl: './create-group-dialog.html',
  styleUrl: './create-group-dialog.scss',
})
export class CreateGroupDialog {
  private readonly groupService = inject(GroupService);
  private readonly dialogRef = inject(DialogRef<GroupSummary | undefined, CreateGroupDialog>);

  protected readonly name = signal('');
  protected readonly isSaving = signal(false);
  protected readonly saveError = signal<string | null>(null);

  protected onCancel(): void {
    this.dialogRef.close(undefined);
  }

  protected async onCreate(): Promise<void> {
    this.saveError.set(null);

    const name = this.name().trim();

    if (!name) {
      this.saveError.set('Введите название группы.');
      return;
    }

    if (name.length > MAX_GROUP_NAME_LENGTH) {
      this.saveError.set(
        `Название не должно превышать ${MAX_GROUP_NAME_LENGTH} символов.`,
      );
      return;
    }

    this.isSaving.set(true);

    try {
      const group = await firstValueFrom(this.groupService.create({ name }));
      this.dialogRef.close(group);
    } catch {
      this.saveError.set('Не удалось создать группу. Попробуйте ещё раз.');
    } finally {
      this.isSaving.set(false);
    }
  }
}
