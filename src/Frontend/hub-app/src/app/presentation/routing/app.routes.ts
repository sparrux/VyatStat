import { Routes } from '@angular/router';

import { authGuard } from '../guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('../pages/main-page/main-page').then((m) => m.MainPage),
    canActivate: [authGuard],
  },
  {
    path: 'account',
    loadComponent: () =>
      import('../pages/account-page/account-page').then((m) => m.AccountPage),
    canActivate: [authGuard],
  },
  {
    path: 'groups',
    loadComponent: () =>
      import('../pages/groups/my-groups-page/my-groups-page').then(
        (m) => m.MyGroupsPage,
      ),
    canActivate: [authGuard],
  },
  {
    path: 'groups/:groupId',
    loadComponent: () =>
      import('../pages/groups/group-page/group-page').then((m) => m.GroupPage),
    canActivate: [authGuard],
  },
  { path: '**', redirectTo: '' },
];
