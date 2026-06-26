import {
  ApplicationConfig,
  importProvidersFrom,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { DialogModule } from '@angular/cdk/dialog';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {
  AuthService,
  authServerInterceptor,
  provideAuthConfig,
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
    }),
    provideHttpClient(withInterceptors([authServerInterceptor])),
    provideAnimationsAsync(),
    importProvidersFrom(DialogModule),
    provideAppInitializer(() => {
      inject(AuthService).onAppBootstrap();
    }),
  ],
};
