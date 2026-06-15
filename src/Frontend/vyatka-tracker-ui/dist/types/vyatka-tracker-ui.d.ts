import * as _angular_core from '@angular/core';
import { ControlValueAccessor } from '@angular/forms';

declare class VtAuthCardComponent {
    readonly description: _angular_core.InputSignal<string | null>;
    readonly eyebrow: _angular_core.InputSignal<string | null>;
    readonly title: _angular_core.InputSignal<string>;
    static ɵfac: _angular_core.ɵɵFactoryDeclaration<VtAuthCardComponent, never>;
    static ɵcmp: _angular_core.ɵɵComponentDeclaration<VtAuthCardComponent, "vt-auth-card", never, { "description": { "alias": "description"; "required": false; "isSignal": true; }; "eyebrow": { "alias": "eyebrow"; "required": false; "isSignal": true; }; "title": { "alias": "title"; "required": true; "isSignal": true; }; }, {}, never, ["*", "[vt-card-actions]"], true, never>;
}

type VtButtonType = 'button' | 'submit' | 'reset';
type VtButtonVariant = 'primary' | 'secondary' | 'ghost';
declare class VtButtonComponent {
    readonly disabled: _angular_core.InputSignalWithTransform<boolean, unknown>;
    readonly type: _angular_core.InputSignal<VtButtonType>;
    readonly variant: _angular_core.InputSignal<VtButtonVariant>;
    static ɵfac: _angular_core.ɵɵFactoryDeclaration<VtButtonComponent, never>;
    static ɵcmp: _angular_core.ɵɵComponentDeclaration<VtButtonComponent, "vt-button", never, { "disabled": { "alias": "disabled"; "required": false; "isSignal": true; }; "type": { "alias": "type"; "required": false; "isSignal": true; }; "variant": { "alias": "variant"; "required": false; "isSignal": true; }; }, {}, never, ["*"], true, never>;
}

type VtTextFieldType = 'email' | 'password' | 'search' | 'tel' | 'text' | 'url';
declare class VtTextFieldComponent implements ControlValueAccessor {
    private static nextId;
    readonly autocomplete: _angular_core.InputSignal<string | null>;
    readonly controlId: _angular_core.InputSignal<string>;
    readonly disabled: _angular_core.InputSignalWithTransform<boolean, unknown>;
    readonly error: _angular_core.InputSignal<string | null>;
    readonly hint: _angular_core.InputSignal<string | null>;
    readonly label: _angular_core.InputSignal<string | null>;
    readonly name: _angular_core.InputSignal<string | null>;
    readonly placeholder: _angular_core.InputSignal<string | null>;
    readonly required: _angular_core.InputSignalWithTransform<boolean, unknown>;
    readonly type: _angular_core.InputSignal<VtTextFieldType>;
    protected readonly formDisabled: _angular_core.WritableSignal<boolean>;
    protected readonly isDisabled: _angular_core.Signal<boolean>;
    protected readonly messageId: _angular_core.Signal<string>;
    protected readonly describedBy: _angular_core.Signal<string | null>;
    protected readonly value: _angular_core.WritableSignal<string>;
    private onChange;
    private onTouched;
    writeValue(value: string | null): void;
    registerOnChange(onChange: (value: string) => void): void;
    registerOnTouched(onTouched: () => void): void;
    setDisabledState(isDisabled: boolean): void;
    protected handleInput(event: Event): void;
    protected handleTouched(): void;
    static ɵfac: _angular_core.ɵɵFactoryDeclaration<VtTextFieldComponent, never>;
    static ɵcmp: _angular_core.ɵɵComponentDeclaration<VtTextFieldComponent, "vt-text-field", never, { "autocomplete": { "alias": "autocomplete"; "required": false; "isSignal": true; }; "controlId": { "alias": "controlId"; "required": false; "isSignal": true; }; "disabled": { "alias": "disabled"; "required": false; "isSignal": true; }; "error": { "alias": "error"; "required": false; "isSignal": true; }; "hint": { "alias": "hint"; "required": false; "isSignal": true; }; "label": { "alias": "label"; "required": false; "isSignal": true; }; "name": { "alias": "name"; "required": false; "isSignal": true; }; "placeholder": { "alias": "placeholder"; "required": false; "isSignal": true; }; "required": { "alias": "required"; "required": false; "isSignal": true; }; "type": { "alias": "type"; "required": false; "isSignal": true; }; }, {}, never, never, true, never>;
}

interface VyatkaColorTokens {
    readonly primary: string;
    readonly neutral: string;
    readonly warmGray: string;
    readonly green: string;
    readonly grayGreen: string;
    readonly mutedSage: string;
    readonly mutedOlive: string;
    readonly error: string;
}
interface VyatkaFontColorTokens {
    readonly primary: string;
    readonly secondary: string;
}
interface VyatkaDesignTokens {
    readonly color: VyatkaColorTokens;
    readonly fontColor: VyatkaFontColorTokens;
    readonly fontFamily: {
        readonly default: string;
    };
    readonly corner: {
        readonly default: number;
    };
    readonly size: {
        readonly cardActionsFrameHeight: number;
    };
}
declare const VYATKA_DESIGN_TOKENS: VyatkaDesignTokens;

export { VYATKA_DESIGN_TOKENS, VtAuthCardComponent, VtButtonComponent, VtTextFieldComponent };
export type { VtButtonType, VtButtonVariant, VtTextFieldType, VyatkaColorTokens, VyatkaDesignTokens, VyatkaFontColorTokens };
