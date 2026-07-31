import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

import { RichTextEditor } from './rich-text-editor';
import { applyHeading, readToolbarState } from './services/editor-toolbar.commands';
import { richTextToSafeHtmlString } from './utils/rich-text-html.utils';
import { TextFormat } from '../../../../application/models/rich-text.model';

@Component({
  standalone: true,
  imports: [RichTextEditor, ReactiveFormsModule],
  template: `<app-rich-text-editor [formControl]="control" />`,
})
class HostWithCva {
  readonly control = new FormControl('<p>Hello</p>', { nonNullable: true });
}

describe('RichTextEditor', () => {
  it('creates the component with TipTap surface', async () => {
    await TestBed.configureTestingModule({
      imports: [RichTextEditor],
    }).compileComponents();

    const fixture = TestBed.createComponent(RichTextEditor);
    fixture.componentRef.setInput('placeholder', 'Type here');
    fixture.detectChanges();
    await fixture.whenStable();

    expect(fixture.componentInstance).toBeTruthy();
    expect(fixture.componentInstance.editor.isEditable).toBe(true);
    expect(
      (fixture.nativeElement as HTMLElement).querySelector('.rte__chrome'),
    ).toBeTruthy();

    fixture.destroy();
  });

  it('implements ControlValueAccessor via formControl', async () => {
    await TestBed.configureTestingModule({
      imports: [HostWithCva],
    }).compileComponents();

    const fixture: ComponentFixture<HostWithCva> =
      TestBed.createComponent(HostWithCva);
    fixture.detectChanges();
    await fixture.whenStable();

    expect(fixture.componentInstance.control.value).toContain('Hello');

    fixture.componentInstance.control.setValue('<p><strong>Updated</strong></p>');
    fixture.detectChanges();
    await fixture.whenStable();

    expect(fixture.componentInstance.control.value).toContain('Updated');
    fixture.destroy();
  });

  it('toggles readonly / disabled editable state', async () => {
    await TestBed.configureTestingModule({
      imports: [RichTextEditor],
    }).compileComponents();

    const fixture = TestBed.createComponent(RichTextEditor);
    fixture.detectChanges();
    await fixture.whenStable();

    fixture.componentRef.setInput('readonly', true);
    fixture.detectChanges();
    expect(fixture.componentInstance.editor.isEditable).toBe(false);

    fixture.componentRef.setInput('readonly', false);
    fixture.componentInstance.setDisabledState(true);
    fixture.detectChanges();
    expect(fixture.componentInstance.editor.isEditable).toBe(false);

    fixture.destroy();
  });
});

describe('editor-toolbar.commands', () => {
  it('reads empty toolbar state from a fresh editor', async () => {
    await TestBed.configureTestingModule({
      imports: [RichTextEditor],
    }).compileComponents();

    const fixture = TestBed.createComponent(RichTextEditor);
    fixture.detectChanges();
    await fixture.whenStable();

    const state = readToolbarState(fixture.componentInstance.editor);
    expect(state.bold).toBe(false);
    expect(state.heading).toBe('');

    applyHeading(fixture.componentInstance.editor, '2');
    expect(
      fixture.componentInstance.editor.isActive('heading', { level: 2 }),
    ).toBe(true);

    fixture.destroy();
  });
});

describe('richTextToSafeHtmlString', () => {
  it('sanitizes html format and escapes plain text', () => {
    const sanitize = (html: string): string =>
      html.replaceAll('<script>', '').replaceAll('</script>', '');

    expect(
      richTextToSafeHtmlString(
        { text: '<p>Hi</p><script>x</script>', format: TextFormat.Html },
        sanitize,
      ),
    ).toBe('<p>Hi</p>x');

    expect(
      richTextToSafeHtmlString(
        { text: 'a < b', format: TextFormat.PlainText },
        sanitize,
      ),
    ).toBe('<p>a &lt; b</p>');
  });
});
