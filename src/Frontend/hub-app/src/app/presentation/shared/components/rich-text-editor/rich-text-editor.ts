import {
  ChangeDetectionStrategy,
  Component,
  OnDestroy,
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
  FormsModule,
  NG_VALUE_ACCESSOR,
} from '@angular/forms';
import { Editor } from '@tiptap/core';
import Link from '@tiptap/extension-link';
import Placeholder from '@tiptap/extension-placeholder';
import Underline from '@tiptap/extension-underline';
import StarterKit from '@tiptap/starter-kit';
import { TiptapEditorDirective } from 'ngx-tiptap';

import { HtmlSanitizerService } from './services/html-sanitizer.service';
import { EditorToolbar } from './toolbar/editor-toolbar';

@Component({
  selector: 'app-rich-text-editor',
  standalone: true,
  imports: [FormsModule, TiptapEditorDirective, EditorToolbar],
  templateUrl: './rich-text-editor.html',
  styleUrl: './rich-text-editor.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
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
export class RichTextEditor implements ControlValueAccessor, OnDestroy {
  private readonly sanitizer = inject(HtmlSanitizerService);

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

  readonly editor = new Editor({
    extensions: [
      StarterKit.configure({
        heading: { levels: [1, 2, 3] },
      }),
      Underline,
      Link.configure({
        openOnClick: false,
        HTMLAttributes: {
          rel: 'noopener noreferrer',
          target: '_blank',
        },
      }),
      Placeholder.configure({
        placeholder: () => this.placeholder(),
      }),
    ],
    content: '',
    editorProps: {
      attributes: {
        class: 'rte__content',
        role: 'textbox',
        'aria-multiline': 'true',
      },
      handleDOMEvents: {
        focus: () => {
          this.focused.emit();
          return false;
        },
        blur: () => {
          this.onTouched();
          this.blurred.emit();
          return false;
        },
      },
    },
    onUpdate: ({ editor }) => {
      this.emitHtml(editor.getHTML());
    },
  });

  constructor() {
    effect(() => {
      const editable = !(this.readonly() || this.isDisabled());
      this.editor.setEditable(editable);
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
      this.applyingExternalValue = true;
      this.editor.commands.setContent(clean || '', { emitUpdate: false });
      this.lastEmittedHtml = clean;
      this.applyingExternalValue = false;
    });
  }

  ngOnDestroy(): void {
    this.editor.destroy();
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
}
