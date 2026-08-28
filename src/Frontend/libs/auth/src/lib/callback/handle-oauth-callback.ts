import { AuthService } from '../services/auth.service';

export interface OAuthCallbackParams {
  error?: string;
  code?: string;
  state?: string;
}

/**
 * Shared OAuth callback handler used by SPA callback routes.
 */
export function handleOAuthCallback(
  auth: AuthService,
  params: OAuthCallbackParams,
  onSuccess: () => void,
): void {
  if (params.error) {
    auth.clearOAuthTransientState();
    void auth.startAuthorizationFlow();
    return;
  }

  const code = params.code;
  const state = params.state;
  if (!code || !auth.validateAndConsumeOAuthState(state)) {
    void auth.startAuthorizationFlow();
    return;
  }

  auth.exchangeCodeForToken(code).subscribe({
    next: (tokens) => {
      auth.applyOAuthTokens(tokens);
      onSuccess();
    },
    error: () => {
      void auth.startAuthorizationFlow();
    },
  });
}
