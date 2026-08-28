import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { AUTH_API, IAuthApi } from './application/contracts/auth-api.contract';
import { AppShell } from './presentation/layouts/app-shell/app-shell';

function createAuthApiStub(): IAuthApi {
  return {
    user: signal(null).asReadonly(),
    sessionResolved: signal(true).asReadonly(),
    isAuthenticated: signal(false).asReadonly(),
    onAppBootstrap: () => Promise.resolve(),
    checkSession: () => of(null),
    login: () => undefined,
    logout: () => of(undefined),
  };
}

describe('AppShell', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppShell],
      providers: [
        provideRouter([]),
        { provide: AUTH_API, useValue: createAuthApiStub() },
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppShell);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render shell', async () => {
    const fixture = TestBed.createComponent(AppShell);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.app-shell')).toBeTruthy();
    expect(compiled.querySelector('app-hub-header')).toBeTruthy();
  });
});
