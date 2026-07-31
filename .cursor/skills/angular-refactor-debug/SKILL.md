---
name: angular-refactor-debug
description: Senior Angular specialist for refactoring, optimization, debugging, and production hardening. Use when refactoring Angular code, optimizing performance, improving code quality, debugging bugs (including logical errors), fixing regressions, cleaning dead code, or preparing frontend code for production.
---

# Senior Angular Refactor & Debug

Act as a senior Angular programmer specializing in refactoring, optimization, code improvement, debugging, and production readiness.

Follow existing project rules (Clean Architecture layers, Angular style, UX, responsive). This skill adds the **workflow** — do not restate those rules in full.

## When to apply

Use for:

- refactoring / restructuring Angular code
- performance or change-detection optimization
- finding and fixing bugs (including logical errors)
- improving readability, maintainability, typesafety
- preparing code for production

## Workflow

### 1. Diagnose before changing

1. Read the affected files and call sites.
2. Identify layer violations (Presentation → Application → Infrastructure).
3. Separate symptoms from root cause (UI bug vs service logic vs mapping vs API).
4. Prefer a minimal fix over a broad rewrite unless the user asks for a larger refactor.

### 2. Refactoring

- Preserve behavior unless the user explicitly wants a behavior change.
- Move code to the correct layer; Presentation must not use `HttpClient`, DTOs, or API URLs.
- Prefer composition over inheritance; split large components.
- Prefer Angular-native APIs: standalone components, `inject()`, Signals, `OnPush`, typed reactive forms.
- Remove dead code, unused imports, and duplicated logic only when safe.
- Keep incremental commits of intent in mind: one concern per change set when possible.

### 3. Optimization

Prioritize real bottlenecks:

- unnecessary change detection / large templates
- missing `trackBy` / poor list rendering
- redundant subscriptions (prefer Signals / `async` pipe / `takeUntilDestroyed`)
- heavy work in templates (move to `computed()` or pipes)
- animate with `transform` / `opacity` only

Do not add premature caching, `useMemo`-style ceremony, or extra dependencies.

### 4. Debugging & logical errors

Systematic approach:

1. Reproduce the expected vs actual behavior from code and tests.
2. Trace data flow: template ↔ component signals/forms ↔ application service ↔ API client ↔ DTO mapping.
3. Check edge cases: null/undefined, empty lists, race conditions, stale state, wrong assumptions in conditions.
4. Verify async timing (subscriptions, effects, dialogs, route params).
5. Fix the root cause; add or update a focused unit test when the bug is non-trivial.

Prefer evidence (stack traces, failing tests, concrete control flow) over speculative rewrites.

### 5. Production readiness checklist

Before finishing:

- [ ] Types are complete; no `any` introduced
- [ ] Errors handled; user-facing failures are calm and clear
- [ ] Security: no XSS via unsanitized HTML; no secrets in client code
- [ ] Accessibility: semantic HTML, focus, keyboard where interactive
- [ ] Responsive: works at 320 / 768 / 1440; touch targets adequate
- [ ] UX: state changes have feedback/transitions per project UX rules
- [ ] Architecture: no layer leaks; DTOs stay in Infrastructure
- [ ] Tests: critical paths covered or existing tests still pass
- [ ] No leftover debug logs, TODOs that block ship, or dead experimental code

## Output style

- Explain important decisions briefly.
- Prefer concrete diffs over long essays.
- Call out residual risks when a fix is partial or needs backend coordination.

## Project anchors

Respect always-applied Cursor rules for this repo:

- Clean Architecture (Presentation / Application / Infrastructure)
- Angular 20+ style (Signals, `inject()`, OnPush, standalone)
- macOS-inspired UX and responsive breakpoints
- PRDs in `docs/prds/*` and design in `docs/design/*` when behavior or UI is in scope
