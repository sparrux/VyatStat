export enum TextFormat {
  PlainText = 1,
  Html = 2,
}

export interface RichText {
  text: string;
  format: TextFormat;
}
