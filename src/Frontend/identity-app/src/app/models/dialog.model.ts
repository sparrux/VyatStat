import { UserClaims } from './auth.model';
import { DashboardUser } from './user.model';

export interface MessageDialogData {
  message: string;
  title?: string;
}

export interface ConfirmDialogData {
  message: string;
  title?: string;
  confirmLabel?: string;
  cancelLabel?: string;
}

export interface UserPermissionsDialogData {
  user: DashboardUser;
}

export interface UserPermissionsDialogResult {
  userId: string;
  claims: UserClaims;
}
