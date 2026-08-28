import { Editor } from '@tiptap/core';
import Link from '@tiptap/extension-link';
import Placeholder from '@tiptap/extension-placeholder';
import Underline from '@tiptap/extension-underline';
import StarterKit from '@tiptap/starter-kit';

import { HEADING_LEVELS } from '../models/editor-options';

export interface CreateRichTextEditorOptions {
  placeholder: () => string;
  onUpdate: (html: string) => void;
  onFocus: () => void;
  onBlur: () => void;
}

/**
 * Builds a TipTap editor configured for Hub rich text (HTML in/out).
 * Extensions stay centralized so toolbar customization can grow later.
 */
export function createRichTextEditor(
  options: CreateRichTextEditorOptions,
): Editor {
  return new Editor({
    extensions: [
      StarterKit.configure({
        heading: { levels: [...HEADING_LEVELS] },
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
        placeholder: () => options.placeholder(),
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
          options.onFocus();
          return false;
        },
        blur: () => {
          options.onBlur();
          return false;
        },
      },
    },
    onUpdate: ({ editor }) => {
      options.onUpdate(editor.getHTML());
    },
  });
}
