import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import {
  BffAuthService,
  bffCredentialsInterceptor,
  provideBffAuthConfig,
} from '@vyatka-tracker/auth';

import { routes } from './app.routes';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideBffAuthConfig({
      bffBaseUrl: environment.hubApiUrl,
    }),
    provideHttpClient(withInterceptors([bffCredentialsInterceptor])),
    provideAppInitializer(() => {
      return inject(BffAuthService).onAppBootstrap();
    }),
  ],
};
