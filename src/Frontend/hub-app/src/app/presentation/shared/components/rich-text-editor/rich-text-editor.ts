import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  ViewEncapsulation,
  effect,
  forwardRef,
  inject,
  input,
  model,
  output,
  signal,
} from '@angular/core';
import {
  ControlValueAccessor,
  NG_VALUE_ACCESSOR,
} from '@angular/forms';
import type { Editor } from '@tiptap/core';
import { TiptapEditorDirective } from 'ngx-tiptap';

import { createRichTextEditor } from './services/create-rich-text-editor';
import { HtmlSanitizerService } from './services/html-sanitizer.service';
import { EditorToolbar } from './toolbar/editor-toolbar';

@Component({
  selector: 'app-rich-text-editor',
  standalone: true,
  imports: [TiptapEditorDirective, EditorToolbar],
  templateUrl: './rich-text-editor.html',
  styleUrl: './rich-text-editor.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  // ProseMirror mounts outside Angular's template tree; scope via `.rte`.
  encapsulation: ViewEncapsulation.None,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RichTextEditor),
      multi: true,
    },
  ],
  host: {
    class: 'rte',
    '[class.rte--readonly]': 'readonly()',
    '[class.rte--disabled]': 'isDisabled()',
  },
})
export class RichTextEditor implements ControlValueAccessor {
  private readonly sanitizer = inject(HtmlSanitizerService);
  private readonly destroyRef = inject(DestroyRef);

  /** Two-way HTML value: `[(value)]="html"`. */
  readonly value = model<string>('');

  readonly readonly = input(false);
  readonly placeholder = input('Введите текст…');

  readonly focused = output<void>({ alias: 'focus' });
  readonly blurred = output<void>({ alias: 'blur' });

  protected readonly isDisabled = signal(false);

  private lastEmittedHtml = '';
  private applyingExternalValue = false;

  private onChange: (value: string) => void = () => undefined;
  private onTouched: () => void = () => undefined;

  readonly editor: Editor = createRichTextEditor({
    placeholder: () => this.placeholder(),
    onUpdate: (html) => this.emitHtml(html),
    onFocus: () => this.focused.emit(),
    onBlur: () => {
      this.onTouched();
      this.blurred.emit();
    },
  });

  constructor() {
    this.destroyRef.onDestroy(() => {
      this.editor.destroy();
    });

    effect(() => {
      const editable = !(this.readonly() || this.isDisabled());
      this.editor.setEditable(editable);
      this.syncAriaReadonly(!editable);
    });

    effect(() => {
      const html = this.value();
      if (this.applyingExternalValue) {
        return;
      }

      const clean = this.sanitizer.sanitize(html);
      if (clean === this.lastEmittedHtml) {
        return;
      }

      this.applyExternalHtml(clean);
    });
  }

  writeValue(value: string | null): void {
    const clean = this.sanitizer.sanitize(value ?? '');
    this.applyingExternalValue = true;
    this.value.set(clean);
    this.lastEmittedHtml = clean;
    this.editor.commands.setContent(clean || '', { emitUpdate: false });
    this.applyingExternalValue = false;
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.isDisabled.set(isDisabled);
  }

  private applyExternalHtml(clean: string): void {
    this.applyingExternalValue = true;
    this.editor.commands.setContent(clean || '', { emitUpdate: false });
    this.lastEmittedHtml = clean;
    this.applyingExternalValue = false;
  }

  private emitHtml(html: string): void {
    if (this.applyingExternalValue) {
      return;
    }

    const clean = this.sanitizer.sanitize(html);
    if (clean === this.lastEmittedHtml) {
      return;
    }

    this.lastEmittedHtml = clean;
    this.value.set(clean);
    this.onChange(clean);
  }

  private syncAriaReadonly(isReadonly: boolean): void {
    if (this.editor.isDestroyed) {
      return;
    }

    try {
      const root = this.editor.view.dom;
      if (isReadonly) {
        root.setAttribute('aria-readonly', 'true');
      } else {
        root.removeAttribute('aria-readonly');
      }
    } catch {
      // View is not mounted yet (ngx-tiptap attaches root later).
    }
  }
}
