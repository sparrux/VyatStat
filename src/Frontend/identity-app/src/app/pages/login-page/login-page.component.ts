import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '@vyatka-tracker/auth';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss',
})
export class LoginPageComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  protected username = '';
  protected password = '';
  protected readonly isSubmitting = signal(false);
  protected readonly submitError = signal<string | null>(null);
  protected readonly isExternalOAuth = signal(false);

  ngOnInit(): void {
    void this.initialize();
  }

  protected async onSubmit(event: Event): Promise<void> {
    event.preventDefault();
    this.submitError.set(null);

    const login = this.username.trim();
    if (!login || !this.password) {
      this.submitError.set('Enter your login and password');
      return;
    }

    this.isSubmitting.set(true);
    try {
      const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
      const safeReturnUrl =
        returnUrl && this.auth.isValidAuthorizeReturnUrl(returnUrl) ? returnUrl : null;
      await this.auth.login(login, this.password, safeReturnUrl);
    } catch {
      this.isSubmitting.set(false);
      this.submitError.set('Failed to login. Please try again.');
    }
  }

  private async initialize(): Promise<void> {
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');

    if (returnUrl && this.auth.isValidAuthorizeReturnUrl(returnUrl)) {
      this.isExternalOAuth.set(true);

      if (await this.auth.hasIdpCookieSession()) {
        window.location.href = returnUrl;
        return;
      }

      return;
    }

    if (this.auth.isAuthenticated()) {
      void this.router.navigate(['/account']);
      return;
    }

    void this.auth.startAuthorizationFlow();
  }
}
