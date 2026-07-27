import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import {
  AuthService,
  provideAuthConfig,
  resourceApiInterceptor,
} from '@vyatka-tracker/auth';

import { routes } from './app.routes';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideAuthConfig({
      authServerUrl: environment.authServerUrl,
      clientId: environment.clientId,
      apiAudience: environment.apiAudience,
      resourceApiUrl: environment.hubApiUrl,
      postLogoutRedirectUrl: environment.identityAppUrl,
    }),
    provideHttpClient(withInterceptors([resourceApiInterceptor])),
    provideAppInitializer(() => {
      inject(AuthService).onAppBootstrap();
    }),
  ],
};
