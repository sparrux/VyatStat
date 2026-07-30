import {
  ApplicationConfig,
  importProvidersFrom,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter } from '@angular/router';
import { DialogModule } from '@angular/cdk/dialog';

import { AuthenticationService } from './application/services/authentication.service';
import { provideHubAuthInfrastructure } from './infrastructure/auth/provide-auth';
import { provideHubGroupsInfrastructure } from './infrastructure/groups/provide-groups';
import { provideHubUsersInfrastructure } from './infrastructure/users/provide-users';
import { routes } from './presentation/routing/app.routes';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideAnimationsAsync(),
    importProvidersFrom(DialogModule),
    provideHubAuthInfrastructure({
      bffBaseUrl: environment.hubApiUrl,
    }),
    provideHubGroupsInfrastructure(),
    provideHubUsersInfrastructure(),
    provideAppInitializer(() => {
      return inject(AuthenticationService).onAppBootstrap();
    }),
  ],
};
