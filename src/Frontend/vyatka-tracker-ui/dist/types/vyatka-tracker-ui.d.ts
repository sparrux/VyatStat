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

export { VYATKA_DESIGN_TOKENS };
export type { VyatkaColorTokens, VyatkaDesignTokens, VyatkaFontColorTokens };
