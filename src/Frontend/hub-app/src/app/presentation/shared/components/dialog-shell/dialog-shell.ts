import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-dialog-shell',
  templateUrl: './dialog-shell.html',
  styleUrl: './dialog-shell.scss',
})
export class DialogShell {
  readonly title = input<string | null>(null);
  readonly showCloseButton = input(true);

  readonly closed = output<void>();

  protected onCloseClick(): void {
    this.closed.emit();
  }
}
