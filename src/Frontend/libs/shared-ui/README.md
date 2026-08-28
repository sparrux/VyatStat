# VyatkaTracker UI

Shared Angular UI library for Identity Server and the main VyatkaTracker frontend.

The library is intentionally not connected to any application yet. It contains:

- Figma-based design tokens in TypeScript and SCSS.
- Global style entry point for shared page defaults.
- Standalone Angular components for auth cards, buttons, and text fields.

## Structure

```text
src/
  public-api.ts
  lib/
    components/
      auth-card/
      button/
      text-field/
    styles/
      _base.scss
      _tokens.scss
      index.scss
    tokens/
      design-tokens.ts
```

## Design Tokens

The token values come from `docs/design/.variables/Firgma_Variables.json`.

Apps import via path mapping (`@vyatka-tracker/ui` → `libs/shared-ui/src/public-api.ts`).

Use SCSS tokens by importing the style entry point from an app:

```scss
@use '@vyatka-tracker/ui/styles';
```

Use TypeScript tokens when code needs exact design values:

```ts
import { VYATKA_DESIGN_TOKENS } from '@vyatka-tracker/ui';
```

## Build

Install dependencies in this package first, then build:

```bash
npm install
npm run build
```
