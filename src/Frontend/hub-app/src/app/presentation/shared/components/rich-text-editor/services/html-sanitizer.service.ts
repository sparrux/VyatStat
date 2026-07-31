import { Injectable } from '@angular/core';

import { RICH_TEXT_ALLOWED_TAGS } from '../models/editor-options';

const ALLOWED = new Set<string>(RICH_TEXT_ALLOWED_TAGS);
const TAG_ALIASES: Record<string, string> = {
  b: 'strong',
  i: 'em',
  strike: 's',
  del: 's',
};

/**
 * Sanitizes rich-text HTML for safe display and storage.
 * Strips styles, classes, scripts, event handlers, and unknown tags.
 */
@Injectable({ providedIn: 'root' })
export class HtmlSanitizerService {
  sanitize(html: string): string {
    if (!html?.trim()) {
      return '';
    }

    const doc = new DOMParser().parseFromString(html, 'text/html');
    this.cleanNode(doc.body);
    return this.normalizeEmpty(doc.body.innerHTML.trim());
  }

  private cleanNode(root: ParentNode): void {
    const walker = Array.from(root.childNodes);

    for (const node of walker) {
      if (node.nodeType === Node.COMMENT_NODE) {
        node.parentNode?.removeChild(node);
        continue;
      }

      if (node.nodeType === Node.TEXT_NODE) {
        continue;
      }

      if (node.nodeType !== Node.ELEMENT_NODE) {
        node.parentNode?.removeChild(node);
        continue;
      }

      const el = node as HTMLElement;
      const tag = el.tagName.toLowerCase();

      if (TAG_ALIASES[tag]) {
        const replacement = el.ownerDocument.createElement(TAG_ALIASES[tag]);
        while (el.firstChild) {
          replacement.appendChild(el.firstChild);
        }
        el.replaceWith(replacement);
        this.cleanNode(replacement);
        continue;
      }

      if (!ALLOWED.has(tag)) {
        const parent = el.parentNode;
        if (!parent) {
          continue;
        }
        while (el.firstChild) {
          parent.insertBefore(el.firstChild, el);
        }
        parent.removeChild(el);
        this.cleanNode(parent);
        continue;
      }

      this.stripAttributes(el, tag);
      this.cleanNode(el);
    }
  }

  private stripAttributes(el: HTMLElement, tag: string): void {
    const keep = new Set<string>();

    if (tag === 'a') {
      const href = el.getAttribute('href')?.trim() ?? '';
      if (this.isSafeHref(href)) {
        el.setAttribute('href', href);
        keep.add('href');
      }
      const target = el.getAttribute('target');
      if (target === '_blank') {
        el.setAttribute('target', '_blank');
        el.setAttribute('rel', 'noopener noreferrer');
        keep.add('target');
        keep.add('rel');
      }
    }

    for (const attr of Array.from(el.attributes)) {
      const name = attr.name.toLowerCase();
      if (keep.has(name)) {
        continue;
      }
      el.removeAttribute(attr.name);
    }
  }

  private isSafeHref(href: string): boolean {
    if (!href) {
      return false;
    }
    const lower = href.toLowerCase();
    if (
      lower.startsWith('javascript:') ||
      lower.startsWith('data:') ||
      lower.startsWith('vbscript:')
    ) {
      return false;
    }
    return (
      lower.startsWith('http://') ||
      lower.startsWith('https://') ||
      lower.startsWith('mailto:') ||
      lower.startsWith('tel:') ||
      lower.startsWith('/') ||
      lower.startsWith('#')
    );
  }

  private normalizeEmpty(html: string): string {
    if (!html || html === '<p><br></p>' || html === '<p></p>') {
      return '';
    }
    return html;
  }
}
