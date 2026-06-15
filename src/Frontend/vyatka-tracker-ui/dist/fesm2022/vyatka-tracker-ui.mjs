import * as i0 from '@angular/core';
import { input, ChangeDetectionStrategy, Component, booleanAttribute, signal, computed, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';

class VtAuthCardComponent {
    description = input(null, /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "description" }] : /* istanbul ignore next */ []));
    eyebrow = input(null, /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "eyebrow" }] : /* istanbul ignore next */ []));
    title = input.required(/* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "title" }] : /* istanbul ignore next */ []));
    static ɵfac = i0.ɵɵngDeclareFactory({ minVersion: "12.0.0", version: "22.0.1", ngImport: i0, type: VtAuthCardComponent, deps: [], target: i0.ɵɵFactoryTarget.Component });
    static ɵcmp = i0.ɵɵngDeclareComponent({ minVersion: "17.0.0", version: "22.0.1", type: VtAuthCardComponent, isStandalone: true, selector: "vt-auth-card", inputs: { description: { classPropertyName: "description", publicName: "description", isSignal: true, isRequired: false, transformFunction: null }, eyebrow: { classPropertyName: "eyebrow", publicName: "eyebrow", isSignal: true, isRequired: false, transformFunction: null }, title: { classPropertyName: "title", publicName: "title", isSignal: true, isRequired: true, transformFunction: null } }, ngImport: i0, template: `
    <section class="vt-auth-card">
      <header class="vt-auth-card__header">
        @if (eyebrow()) {
          <p class="vt-auth-card__eyebrow">{{ eyebrow() }}</p>
        }

        <h1 class="vt-auth-card__title">{{ title() }}</h1>

        @if (description()) {
          <p class="vt-auth-card__description">{{ description() }}</p>
        }
      </header>

      <div class="vt-auth-card__content">
        <ng-content />
      </div>

      <footer class="vt-auth-card__actions">
        <ng-content select="[vt-card-actions]" />
      </footer>
    </section>
  `, isInline: true, styles: [":host{display:block;width:min(100%,420px)}.vt-auth-card{background:var(--vt-surface-card);border:1px solid var(--vt-border-subtle);border-radius:calc(var(--vt-corner-default) * 2);box-shadow:0 20px 50px #31313114;color:var(--vt-font-color-primary);overflow:hidden}.vt-auth-card__header,.vt-auth-card__content{padding:28px 32px 0}.vt-auth-card__eyebrow{color:var(--vt-color-gray-green);font-size:13px;font-weight:700;letter-spacing:.08em;margin:0 0 10px;text-transform:uppercase}.vt-auth-card__title{color:var(--vt-font-color-secondary);font-size:28px;line-height:1.15;margin:0}.vt-auth-card__description{color:var(--vt-font-color-primary);line-height:1.5;margin:12px 0 0}.vt-auth-card__content{display:grid;gap:16px;padding-bottom:24px}.vt-auth-card__actions{align-items:center;background:var(--vt-surface-muted);border-top:1px solid var(--vt-border-subtle);display:flex;gap:12px;justify-content:flex-end;min-height:var(--vt-card-actions-frame-height);padding:12px 32px}\n"], changeDetection: i0.ChangeDetectionStrategy.OnPush });
}
i0.ɵɵngDeclareClassMetadata({ minVersion: "12.0.0", version: "22.0.1", ngImport: i0, type: VtAuthCardComponent, decorators: [{
            type: Component,
            args: [{ selector: 'vt-auth-card', standalone: true, template: `
    <section class="vt-auth-card">
      <header class="vt-auth-card__header">
        @if (eyebrow()) {
          <p class="vt-auth-card__eyebrow">{{ eyebrow() }}</p>
        }

        <h1 class="vt-auth-card__title">{{ title() }}</h1>

        @if (description()) {
          <p class="vt-auth-card__description">{{ description() }}</p>
        }
      </header>

      <div class="vt-auth-card__content">
        <ng-content />
      </div>

      <footer class="vt-auth-card__actions">
        <ng-content select="[vt-card-actions]" />
      </footer>
    </section>
  `, changeDetection: ChangeDetectionStrategy.OnPush, styles: [":host{display:block;width:min(100%,420px)}.vt-auth-card{background:var(--vt-surface-card);border:1px solid var(--vt-border-subtle);border-radius:calc(var(--vt-corner-default) * 2);box-shadow:0 20px 50px #31313114;color:var(--vt-font-color-primary);overflow:hidden}.vt-auth-card__header,.vt-auth-card__content{padding:28px 32px 0}.vt-auth-card__eyebrow{color:var(--vt-color-gray-green);font-size:13px;font-weight:700;letter-spacing:.08em;margin:0 0 10px;text-transform:uppercase}.vt-auth-card__title{color:var(--vt-font-color-secondary);font-size:28px;line-height:1.15;margin:0}.vt-auth-card__description{color:var(--vt-font-color-primary);line-height:1.5;margin:12px 0 0}.vt-auth-card__content{display:grid;gap:16px;padding-bottom:24px}.vt-auth-card__actions{align-items:center;background:var(--vt-surface-muted);border-top:1px solid var(--vt-border-subtle);display:flex;gap:12px;justify-content:flex-end;min-height:var(--vt-card-actions-frame-height);padding:12px 32px}\n"] }]
        }], propDecorators: { description: [{ type: i0.Input, args: [{ isSignal: true, alias: "description", required: false }] }], eyebrow: [{ type: i0.Input, args: [{ isSignal: true, alias: "eyebrow", required: false }] }], title: [{ type: i0.Input, args: [{ isSignal: true, alias: "title", required: true }] }] } });

class VtButtonComponent {
    disabled = input(false, { ...(ngDevMode ? { debugName: "disabled" } : /* istanbul ignore next */ {}), transform: booleanAttribute });
    type = input('button', /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "type" }] : /* istanbul ignore next */ []));
    variant = input('primary', /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "variant" }] : /* istanbul ignore next */ []));
    static ɵfac = i0.ɵɵngDeclareFactory({ minVersion: "12.0.0", version: "22.0.1", ngImport: i0, type: VtButtonComponent, deps: [], target: i0.ɵɵFactoryTarget.Component });
    static ɵcmp = i0.ɵɵngDeclareComponent({ minVersion: "17.1.0", version: "22.0.1", type: VtButtonComponent, isStandalone: true, selector: "vt-button", inputs: { disabled: { classPropertyName: "disabled", publicName: "disabled", isSignal: true, isRequired: false, transformFunction: null }, type: { classPropertyName: "type", publicName: "type", isSignal: true, isRequired: false, transformFunction: null }, variant: { classPropertyName: "variant", publicName: "variant", isSignal: true, isRequired: false, transformFunction: null } }, ngImport: i0, template: `
    <button
      class="vt-button"
      [class.vt-button--ghost]="variant() === 'ghost'"
      [class.vt-button--primary]="variant() === 'primary'"
      [class.vt-button--secondary]="variant() === 'secondary'"
      [disabled]="disabled()"
      [type]="type()"
    >
      <ng-content />
    </button>
  `, isInline: true, styles: [":host{display:inline-flex}.vt-button{align-items:center;border:1px solid transparent;border-radius:var(--vt-corner-default);cursor:pointer;display:inline-flex;font-weight:600;gap:8px;justify-content:center;min-height:44px;padding:0 20px;transition:background-color .16s ease,border-color .16s ease,color .16s ease,opacity .16s ease}.vt-button:disabled{cursor:not-allowed;opacity:.55}.vt-button--primary{background:var(--vt-action-primary);color:var(--vt-action-primary-contrast)}.vt-button--primary:not(:disabled):hover{background:var(--vt-action-primary-hover)}.vt-button--secondary{background:var(--vt-color-green);color:var(--vt-font-color-secondary)}.vt-button--secondary:not(:disabled):hover{background:var(--vt-color-gray-green);color:var(--vt-action-primary-contrast)}.vt-button--ghost{background:transparent;border-color:var(--vt-border-subtle);color:var(--vt-font-color-primary)}.vt-button--ghost:not(:disabled):hover{background:var(--vt-surface-muted)}\n"], changeDetection: i0.ChangeDetectionStrategy.OnPush });
}
i0.ɵɵngDeclareClassMetadata({ minVersion: "12.0.0", version: "22.0.1", ngImport: i0, type: VtButtonComponent, decorators: [{
            type: Component,
            args: [{ selector: 'vt-button', standalone: true, template: `
    <button
      class="vt-button"
      [class.vt-button--ghost]="variant() === 'ghost'"
      [class.vt-button--primary]="variant() === 'primary'"
      [class.vt-button--secondary]="variant() === 'secondary'"
      [disabled]="disabled()"
      [type]="type()"
    >
      <ng-content />
    </button>
  `, changeDetection: ChangeDetectionStrategy.OnPush, styles: [":host{display:inline-flex}.vt-button{align-items:center;border:1px solid transparent;border-radius:var(--vt-corner-default);cursor:pointer;display:inline-flex;font-weight:600;gap:8px;justify-content:center;min-height:44px;padding:0 20px;transition:background-color .16s ease,border-color .16s ease,color .16s ease,opacity .16s ease}.vt-button:disabled{cursor:not-allowed;opacity:.55}.vt-button--primary{background:var(--vt-action-primary);color:var(--vt-action-primary-contrast)}.vt-button--primary:not(:disabled):hover{background:var(--vt-action-primary-hover)}.vt-button--secondary{background:var(--vt-color-green);color:var(--vt-font-color-secondary)}.vt-button--secondary:not(:disabled):hover{background:var(--vt-color-gray-green);color:var(--vt-action-primary-contrast)}.vt-button--ghost{background:transparent;border-color:var(--vt-border-subtle);color:var(--vt-font-color-primary)}.vt-button--ghost:not(:disabled):hover{background:var(--vt-surface-muted)}\n"] }]
        }], propDecorators: { disabled: [{ type: i0.Input, args: [{ isSignal: true, alias: "disabled", required: false }] }], type: [{ type: i0.Input, args: [{ isSignal: true, alias: "type", required: false }] }], variant: [{ type: i0.Input, args: [{ isSignal: true, alias: "variant", required: false }] }] } });

class VtTextFieldComponent {
    static nextId = 0;
    autocomplete = input(null, /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "autocomplete" }] : /* istanbul ignore next */ []));
    controlId = input(`vt-text-field-${VtTextFieldComponent.nextId++}`, /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "controlId" }] : /* istanbul ignore next */ []));
    disabled = input(false, { ...(ngDevMode ? { debugName: "disabled" } : /* istanbul ignore next */ {}), transform: booleanAttribute });
    error = input(null, /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "error" }] : /* istanbul ignore next */ []));
    hint = input(null, /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "hint" }] : /* istanbul ignore next */ []));
    label = input(null, /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "label" }] : /* istanbul ignore next */ []));
    name = input(null, /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "name" }] : /* istanbul ignore next */ []));
    placeholder = input(null, /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "placeholder" }] : /* istanbul ignore next */ []));
    required = input(false, { ...(ngDevMode ? { debugName: "required" } : /* istanbul ignore next */ {}), transform: booleanAttribute });
    type = input('text', /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "type" }] : /* istanbul ignore next */ []));
    formDisabled = signal(false, /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "formDisabled" }] : /* istanbul ignore next */ []));
    isDisabled = computed(() => this.disabled() || this.formDisabled(), /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "isDisabled" }] : /* istanbul ignore next */ []));
    messageId = computed(() => `${this.controlId()}-message`, /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "messageId" }] : /* istanbul ignore next */ []));
    describedBy = computed(() => (this.hint() || this.error() ? this.messageId() : null), /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "describedBy" }] : /* istanbul ignore next */ []));
    value = signal('', /* @ts-ignore */
    ...(ngDevMode ? [{ debugName: "value" }] : /* istanbul ignore next */ []));
    onChange = () => undefined;
    onTouched = () => undefined;
    writeValue(value) {
        this.value.set(value ?? '');
    }
    registerOnChange(onChange) {
        this.onChange = onChange;
    }
    registerOnTouched(onTouched) {
        this.onTouched = onTouched;
    }
    setDisabledState(isDisabled) {
        this.formDisabled.set(isDisabled);
    }
    handleInput(event) {
        const nextValue = event.target.value;
        this.value.set(nextValue);
        this.onChange(nextValue);
    }
    handleTouched() {
        this.onTouched();
    }
    static ɵfac = i0.ɵɵngDeclareFactory({ minVersion: "12.0.0", version: "22.0.1", ngImport: i0, type: VtTextFieldComponent, deps: [], target: i0.ɵɵFactoryTarget.Component });
    static ɵcmp = i0.ɵɵngDeclareComponent({ minVersion: "17.0.0", version: "22.0.1", type: VtTextFieldComponent, isStandalone: true, selector: "vt-text-field", inputs: { autocomplete: { classPropertyName: "autocomplete", publicName: "autocomplete", isSignal: true, isRequired: false, transformFunction: null }, controlId: { classPropertyName: "controlId", publicName: "controlId", isSignal: true, isRequired: false, transformFunction: null }, disabled: { classPropertyName: "disabled", publicName: "disabled", isSignal: true, isRequired: false, transformFunction: null }, error: { classPropertyName: "error", publicName: "error", isSignal: true, isRequired: false, transformFunction: null }, hint: { classPropertyName: "hint", publicName: "hint", isSignal: true, isRequired: false, transformFunction: null }, label: { classPropertyName: "label", publicName: "label", isSignal: true, isRequired: false, transformFunction: null }, name: { classPropertyName: "name", publicName: "name", isSignal: true, isRequired: false, transformFunction: null }, placeholder: { classPropertyName: "placeholder", publicName: "placeholder", isSignal: true, isRequired: false, transformFunction: null }, required: { classPropertyName: "required", publicName: "required", isSignal: true, isRequired: false, transformFunction: null }, type: { classPropertyName: "type", publicName: "type", isSignal: true, isRequired: false, transformFunction: null } }, providers: [
            {
                provide: NG_VALUE_ACCESSOR,
                useExisting: forwardRef(() => VtTextFieldComponent),
                multi: true,
            },
        ], ngImport: i0, template: `
    <div class="vt-text-field" [class.vt-text-field--invalid]="error()">
      @if (label()) {
        <label class="vt-text-field__label" [for]="controlId()">
          {{ label() }}
        </label>
      }

      <input
        class="vt-text-field__control"
        [attr.aria-describedby]="describedBy()"
        [attr.aria-invalid]="error() ? 'true' : null"
        [attr.autocomplete]="autocomplete()"
        [attr.placeholder]="placeholder()"
        [disabled]="isDisabled()"
        [id]="controlId()"
        [name]="name() || controlId()"
        [required]="required()"
        [type]="type()"
        [value]="value()"
        (blur)="handleTouched()"
        (input)="handleInput($event)"
      />

      @if (hint() || error()) {
        <p class="vt-text-field__message" [id]="messageId()">
          {{ error() || hint() }}
        </p>
      }
    </div>
  `, isInline: true, styles: [":host{display:block}.vt-text-field{display:grid;gap:8px}.vt-text-field__label{color:var(--vt-font-color-primary);font-size:14px;font-weight:600}.vt-text-field__control{background:var(--vt-surface-card);border:1px solid var(--vt-border-subtle);border-radius:var(--vt-corner-default);color:var(--vt-font-color-secondary);min-height:44px;outline:none;padding:0 14px;transition:border-color .16s ease,box-shadow .16s ease;width:100%}.vt-text-field__control::placeholder{color:var(--vt-color-gray-green)}.vt-text-field__control:focus{border-color:var(--vt-color-muted-olive);box-shadow:0 0 0 3px #595e5024}.vt-text-field__control:disabled{background:var(--vt-surface-muted);cursor:not-allowed}.vt-text-field__message{color:var(--vt-color-gray-green);font-size:13px;line-height:1.35;margin:0}.vt-text-field--invalid .vt-text-field__control{border-color:var(--vt-color-error)}.vt-text-field--invalid .vt-text-field__message{color:var(--vt-color-error)}\n"], changeDetection: i0.ChangeDetectionStrategy.OnPush });
}
i0.ɵɵngDeclareClassMetadata({ minVersion: "12.0.0", version: "22.0.1", ngImport: i0, type: VtTextFieldComponent, decorators: [{
            type: Component,
            args: [{ selector: 'vt-text-field', standalone: true, template: `
    <div class="vt-text-field" [class.vt-text-field--invalid]="error()">
      @if (label()) {
        <label class="vt-text-field__label" [for]="controlId()">
          {{ label() }}
        </label>
      }

      <input
        class="vt-text-field__control"
        [attr.aria-describedby]="describedBy()"
        [attr.aria-invalid]="error() ? 'true' : null"
        [attr.autocomplete]="autocomplete()"
        [attr.placeholder]="placeholder()"
        [disabled]="isDisabled()"
        [id]="controlId()"
        [name]="name() || controlId()"
        [required]="required()"
        [type]="type()"
        [value]="value()"
        (blur)="handleTouched()"
        (input)="handleInput($event)"
      />

      @if (hint() || error()) {
        <p class="vt-text-field__message" [id]="messageId()">
          {{ error() || hint() }}
        </p>
      }
    </div>
  `, providers: [
                        {
                            provide: NG_VALUE_ACCESSOR,
                            useExisting: forwardRef(() => VtTextFieldComponent),
                            multi: true,
                        },
                    ], changeDetection: ChangeDetectionStrategy.OnPush, styles: [":host{display:block}.vt-text-field{display:grid;gap:8px}.vt-text-field__label{color:var(--vt-font-color-primary);font-size:14px;font-weight:600}.vt-text-field__control{background:var(--vt-surface-card);border:1px solid var(--vt-border-subtle);border-radius:var(--vt-corner-default);color:var(--vt-font-color-secondary);min-height:44px;outline:none;padding:0 14px;transition:border-color .16s ease,box-shadow .16s ease;width:100%}.vt-text-field__control::placeholder{color:var(--vt-color-gray-green)}.vt-text-field__control:focus{border-color:var(--vt-color-muted-olive);box-shadow:0 0 0 3px #595e5024}.vt-text-field__control:disabled{background:var(--vt-surface-muted);cursor:not-allowed}.vt-text-field__message{color:var(--vt-color-gray-green);font-size:13px;line-height:1.35;margin:0}.vt-text-field--invalid .vt-text-field__control{border-color:var(--vt-color-error)}.vt-text-field--invalid .vt-text-field__message{color:var(--vt-color-error)}\n"] }]
        }], propDecorators: { autocomplete: [{ type: i0.Input, args: [{ isSignal: true, alias: "autocomplete", required: false }] }], controlId: [{ type: i0.Input, args: [{ isSignal: true, alias: "controlId", required: false }] }], disabled: [{ type: i0.Input, args: [{ isSignal: true, alias: "disabled", required: false }] }], error: [{ type: i0.Input, args: [{ isSignal: true, alias: "error", required: false }] }], hint: [{ type: i0.Input, args: [{ isSignal: true, alias: "hint", required: false }] }], label: [{ type: i0.Input, args: [{ isSignal: true, alias: "label", required: false }] }], name: [{ type: i0.Input, args: [{ isSignal: true, alias: "name", required: false }] }], placeholder: [{ type: i0.Input, args: [{ isSignal: true, alias: "placeholder", required: false }] }], required: [{ type: i0.Input, args: [{ isSignal: true, alias: "required", required: false }] }], type: [{ type: i0.Input, args: [{ isSignal: true, alias: "type", required: false }] }] } });

const VYATKA_DESIGN_TOKENS = {
    color: {
        primary: '#ECEBE9',
        neutral: '#F3F3F0',
        warmGray: '#DDD9D3',
        green: '#B1D686',
        grayGreen: '#949E82',
        mutedSage: '#B1B69C',
        mutedOlive: '#595E50',
        error: '#D26C6C',
    },
    fontColor: {
        primary: '#4E5247',
        secondary: '#313131',
    },
    fontFamily: {
        default: 'Albert Sans',
    },
    corner: {
        default: 5,
    },
    size: {
        cardActionsFrameHeight: 65,
    },
};

/**
 * Generated bundle index. Do not edit.
 */

export { VYATKA_DESIGN_TOKENS, VtAuthCardComponent, VtButtonComponent, VtTextFieldComponent };
//# sourceMappingURL=vyatka-tracker-ui.mjs.map
