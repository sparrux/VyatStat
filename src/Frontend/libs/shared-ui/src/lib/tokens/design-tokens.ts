export interface VyatkaColorTokens {
  readonly primary: string;
  readonly neutral: string;
  readonly warmGray: string;
  readonly green: string;
  readonly grayGreen: string;
  readonly mutedSage: string;
  readonly mutedOlive: string;
  readonly error: string;
}

export interface VyatkaFontColorTokens {
  readonly primary: string;
  readonly secondary: string;
}

export interface VyatkaDesignTokens {
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

export const VYATKA_DESIGN_TOKENS: VyatkaDesignTokens = {
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
