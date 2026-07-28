import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { AuthenticationService } from './application/services/authentication.service';
import { provideHubAuthInfrastructure } from './infrastructure/auth/provide-auth';
import { routes } from './presentation/routing/app.routes';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHubAuthInfrastructure({
      bffBaseUrl: environment.hubApiUrl,
    }),
    provideAppInitializer(() => {
      return inject(AuthenticationService).onAppBootstrap();
    }),
  ],
};
