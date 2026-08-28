import {
  ChangeDetectionStrategy,
  Component,
  effect,
  input,
  signal,
} from '@angular/core';
import type { Editor } from '@tiptap/core';

import {
  EMPTY_TOOLBAR_STATE,
  type HeadingLevel,
} from '../models/editor-options';
import {
  applyHeading,
  readToolbarState,
  toggleLinkPrompt,
} from '../services/editor-toolbar.commands';

@Component({
  selector: 'app-editor-toolbar',
  standalone: true,
  templateUrl: './editor-toolbar.html',
  styleUrl: './editor-toolbar.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EditorToolbar {
  readonly editor = input.required<Editor>();
  readonly disabled = input(false);

  protected readonly state = signal(EMPTY_TOOLBAR_STATE);
  protected readonly codeLabel = '</>';

  protected readonly headingOptions: {
    value: '' | HeadingLevel;
    label: string;
  }[] = [
    { value: '', label: 'Параграф' },
    { value: 1, label: 'Заголовок 1' },
    { value: 2, label: 'Заголовок 2' },
    { value: 3, label: 'Заголовок 3' },
  ];

  constructor() {
    effect((onCleanup) => {
      const editor = this.editor();
      const refresh = (): void => {
        this.state.set(readToolbarState(editor));
      };

      editor.on('selectionUpdate', refresh);
      editor.on('transaction', refresh);
      refresh();

      onCleanup(() => {
        editor.off('selectionUpdate', refresh);
        editor.off('transaction', refresh);
      });
    });
  }

  protected toggleBold(): void {
    this.editor().chain().focus().toggleBold().run();
  }

  protected toggleItalic(): void {
    this.editor().chain().focus().toggleItalic().run();
  }

  protected toggleUnderline(): void {
    this.editor().chain().focus().toggleUnderline().run();
  }

  protected toggleStrike(): void {
    this.editor().chain().focus().toggleStrike().run();
  }

  protected toggleCode(): void {
    this.editor().chain().focus().toggleCode().run();
  }

  protected onHeadingChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    applyHeading(this.editor(), select.value);
  }

  protected toggleBulletList(): void {
    this.editor().chain().focus().toggleBulletList().run();
  }

  protected toggleOrderedList(): void {
    this.editor().chain().focus().toggleOrderedList().run();
  }

  protected toggleBlockquote(): void {
    this.editor().chain().focus().toggleBlockquote().run();
  }

  protected toggleLink(): void {
    toggleLinkPrompt(this.editor());
  }
}
