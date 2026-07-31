import { TestBed } from '@angular/core/testing';

import { HtmlSanitizerService } from './html-sanitizer.service';

describe('HtmlSanitizerService', () => {
  let sanitizer: HtmlSanitizerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    sanitizer = TestBed.inject(HtmlSanitizerService);
  });

  it('returns empty string for blank input', () => {
    expect(sanitizer.sanitize('')).toBe('');
    expect(sanitizer.sanitize('   ')).toBe('');
  });

  it('keeps allowed semantic tags', () => {
    const html = '<h1>Hello</h1><p>This is <strong>formatted</strong> text.</p>';
    expect(sanitizer.sanitize(html)).toContain('<h1>Hello</h1>');
    expect(sanitizer.sanitize(html)).toContain('<strong>formatted</strong>');
  });

  it('strips scripts and event handlers', () => {
    const dirty =
      '<p onclick="alert(1)">Hi</p><script>alert(2)</script><img src=x onerror=alert(3)>';
    const clean = sanitizer.sanitize(dirty);
    expect(clean).not.toContain('script');
    expect(clean).not.toContain('onclick');
    expect(clean).not.toContain('onerror');
    expect(clean).not.toContain('img');
  });

  it('removes style and class attributes', () => {
    const dirty = '<p class="x" style="color:red">Text</p>';
    const clean = sanitizer.sanitize(dirty);
    expect(clean).not.toContain('style');
    expect(clean).not.toContain('class');
    expect(clean).toContain('Text');
  });

  it('blocks javascript urls', () => {
    const dirty = '<a href="javascript:alert(1)">link</a>';
    const clean = sanitizer.sanitize(dirty);
    expect(clean).not.toContain('javascript:');
  });

  it('keeps safe https links', () => {
    const dirty = '<a href="https://example.com">link</a>';
    const clean = sanitizer.sanitize(dirty);
    expect(clean).toContain('href="https://example.com"');
  });

  it('normalizes b/i aliases', () => {
    const dirty = '<p><b>Bold</b> <i>Italic</i></p>';
    const clean = sanitizer.sanitize(dirty);
    expect(clean).toContain('<strong>Bold</strong>');
    expect(clean).toContain('<em>Italic</em>');
  });
});
