import {
  ChangeDetectionStrategy,
  Component,
  effect,
  input,
  signal,
} from '@angular/core';
import type { Editor } from '@tiptap/core';

import type { HeadingLevel } from '../models/editor-options';

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

  protected readonly tick = signal(0);
  protected readonly codeLabel = '</>';

  protected readonly headingOptions: { value: '' | HeadingLevel; label: string }[] =
    [
      { value: '', label: 'Параграф' },
      { value: 1, label: 'Заголовок 1' },
      { value: 2, label: 'Заголовок 2' },
      { value: 3, label: 'Заголовок 3' },
    ];

  constructor() {
    effect((onCleanup) => {
      const editor = this.editor();
      const refresh = (): void => this.tick.update((n) => n + 1);
      editor.on('selectionUpdate', refresh);
      editor.on('transaction', refresh);
      refresh();
      onCleanup(() => {
        editor.off('selectionUpdate', refresh);
        editor.off('transaction', refresh);
      });
    });
  }

  protected isActive(name: string, attrs?: Record<string, unknown>): boolean {
    this.tick();
    return this.editor().isActive(name, attrs);
  }

  protected currentHeading(): '' | HeadingLevel {
    this.tick();
    const editor = this.editor();
    for (const level of [1, 2, 3] as HeadingLevel[]) {
      if (editor.isActive('heading', { level })) {
        return level;
      }
    }
    return '';
  }

  protected canUndo(): boolean {
    this.tick();
    return this.editor().can().undo();
  }

  protected canRedo(): boolean {
    this.tick();
    return this.editor().can().redo();
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

  protected setHeading(raw: string): void {
    const editor = this.editor().chain().focus();
    if (!raw) {
      editor.setParagraph().run();
      return;
    }
    editor.toggleHeading({ level: Number(raw) as HeadingLevel }).run();
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
    const editor = this.editor();
    if (editor.isActive('link')) {
      editor.chain().focus().unsetLink().run();
      return;
    }

    const previous = editor.getAttributes('link')['href'] as string | undefined;
    const url = window.prompt('URL ссылки', previous || 'https://');
    if (url === null) {
      return;
    }
    const trimmed = url.trim();
    if (!trimmed) {
      editor.chain().focus().unsetLink().run();
      return;
    }
    editor.chain().focus().extendMarkRange('link').setLink({ href: trimmed }).run();
  }

  protected undo(): void {
    this.editor().chain().focus().undo().run();
  }

  protected redo(): void {
    this.editor().chain().focus().redo().run();
  }
}
