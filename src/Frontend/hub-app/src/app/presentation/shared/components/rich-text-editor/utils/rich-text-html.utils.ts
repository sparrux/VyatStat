import type { RichText } from '../../../../../application/models/rich-text.model';
import { TextFormat } from '../../../../../application/models/rich-text.model';

/**
 * Converts a domain RichText value to a safe HTML string for display.
 * Caller must still pass the result through DomSanitizer.bypassSecurityTrustHtml.
 */
export function richTextToSafeHtmlString(
  description: RichText,
  sanitizeHtml: (html: string) => string,
): string {
  if (description.format === TextFormat.Html) {
    return sanitizeHtml(description.text);
  }

  const escaped = description.text
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#39;');

  return escaped ? `<p>${escaped}</p>` : '';
}
