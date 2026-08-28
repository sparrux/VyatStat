import type { Editor } from '@tiptap/core';

import {
  EMPTY_TOOLBAR_STATE,
  HEADING_LEVELS,
  type EditorToolbarState,
  type HeadingLevel,
} from '../models/editor-options';

export function readToolbarState(editor: Editor): EditorToolbarState {
  let heading: '' | HeadingLevel = '';
  for (const level of HEADING_LEVELS) {
    if (editor.isActive('heading', { level })) {
      heading = level;
      break;
    }
  }

  return {
    bold: editor.isActive('bold'),
    italic: editor.isActive('italic'),
    underline: editor.isActive('underline'),
    strike: editor.isActive('strike'),
    code: editor.isActive('code'),
    link: editor.isActive('link'),
    bulletList: editor.isActive('bulletList'),
    orderedList: editor.isActive('orderedList'),
    blockquote: editor.isActive('blockquote'),
    heading,
  };
}

export function applyHeading(editor: Editor, raw: string): void {
  const chain = editor.chain().focus();
  if (!raw) {
    chain.setParagraph().run();
    return;
  }

  const level = Number(raw) as HeadingLevel;
  if (!HEADING_LEVELS.includes(level)) {
    chain.setParagraph().run();
    return;
  }

  chain.toggleHeading({ level }).run();
}

export function toggleLinkPrompt(editor: Editor): void {
  if (editor.isActive('link')) {
    editor.chain().focus().unsetLink().run();
    return;
  }

  const previous = (editor.getAttributes('link')['href'] as string | undefined) ?? '';
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

export { EMPTY_TOOLBAR_STATE };
