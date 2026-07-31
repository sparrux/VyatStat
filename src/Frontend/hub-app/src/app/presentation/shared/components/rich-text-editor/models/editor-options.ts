я/** Heading levels enabled in the TipTap StarterKit config. */
export type HeadingLevel = 1 | 2 | 3;

export const HEADING_LEVELS: readonly HeadingLevel[] = [1, 2, 3];

/** Tags allowed when sanitizing HTML for display / persistence. */
export const RICH_TEXT_ALLOWED_TAGS = [
  'p',
  'h1',
  'h2',
  'h3',
  'h4',
  'h5',
  'h6',
  'strong',
  'em',
  'u',
  's',
  'a',
  'blockquote',
  'ul',
  'ol',
  'li',
  'code',
  'pre',
  'hr',
  'br',
] as const;

export interface EditorToolbarState {
  bold: boolean;
  italic: boolean;
  underline: boolean;
  strike: boolean;
  code: boolean;
  link: boolean;
  bulletList: boolean;
  orderedList: boolean;
  blockquote: boolean;
  heading: '' | HeadingLevel;
}

export const EMPTY_TOOLBAR_STATE: EditorToolbarState = {
  bold: false,
  italic: false,
  underline: false,
  strike: false,
  code: false,
  link: false,
  bulletList: false,
  orderedList: false,
  blockquote: false,
  heading: '',
};
