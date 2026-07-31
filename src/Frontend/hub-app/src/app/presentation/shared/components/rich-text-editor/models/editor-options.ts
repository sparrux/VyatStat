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

export type HeadingLevel = 1 | 2 | 3 | 4 | 5 | 6;
