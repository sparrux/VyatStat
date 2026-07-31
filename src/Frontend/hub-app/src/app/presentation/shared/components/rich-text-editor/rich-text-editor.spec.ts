import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

import { RichTextEditor } from './rich-text-editor';

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
});
