import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-dialog-shell',
  standalone: true,
  templateUrl: './dialog-shell.component.html',
  styleUrl: './dialog-shell.component.scss',
})
export class DialogShellComponent {
  readonly title = input<string | null>(null);
  readonly showCloseButton = input(true);

  readonly closed = output<void>();

  protected onCloseClick(): void {
    this.closed.emit();
  }
}
